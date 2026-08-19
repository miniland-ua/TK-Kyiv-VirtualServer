using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Timers;

public struct TCP_packet_type   // Структура пакета данных: [PacketLen][Header][ Data[DB0][DB1]...[DBn][XOR] ]
{
    public byte     PacketLen;
    public byte     Header;
    public byte[]   Data;
    public byte     xor;        

    public TCP_packet_type(byte header, params byte[] args)
    {
        if (args != null && args.Length > 0)
        {
            PacketLen = (byte)(args.Length + 3); // [DataLen]+[Header]+[...]+[XOR]
            Header = header;
            Data = new byte[args.Length + 1];

            Data[0] = args[0];
            xor = args[0];
            for (int i = 1; i < args.Length; i++)
            {
                Data[i] = args[i];
                xor ^= args[i];
            }
            Data[args.Length] = xor;
        }
        else
        {
            PacketLen = 3;  // [DataLen]+[Header]+[XOR]
            Header = header;
            Data = new byte[1] { Header };  // XOR = Header если данных нету
        }
    }

    public bool Validate()
    {
        if (Data == null || Data.Length == 0) return false;

        xor = Data[0];

        for (int i = 1; i < Data.Length - 1; i++)
        {
            xor ^= Data[i];
        }

        if (xor == Data[Data.Length - 1])
            return true;
        else
            return false;
    }
}

internal class TcpClientManager
{
    private TcpClient? _client;
    private NetworkStream? _stream;
    private readonly CancellationTokenSource _cts = new();
    private readonly BlockingCollection<TCP_packet_type> _messageQueue = new(128); // очередь из сообщений на отправку. максимум 128
    private readonly ushort _deviceID;      // Уникальный ID клиента (например, серийный номер прибора)
    private readonly string _serverIP;      // IP удаленного сервера
    private readonly int _serverPort;       // Порт удаленного сервера
    private string _connectionId;           // "IP:Port" для сетевой идентификации (LocalEndPoint)
    private const int _retryCount = 3;      // Количество попыток для отправки пакета
    private static Task? _timerTask;        // Таймер для отсчета времени после последнего полученого пакета

    public bool isConnected;
    public event Action<string>? OnLogMessage;  // Событие для вывода логов в UI
    public event Action<TCP_packet_type>? NewPacketReceived; // Событие когда завершен прием пакета от сервера

    public TcpClientManager(string ip, int port, ushort id)
    {
        _serverIP = ip;
        _serverPort = port;
        _deviceID = id;
        _connectionId = string.Empty;
    }

    private void CloseConnection()
    {
        try { _stream?.Close(); } catch { }
        try { _client?.Close(); } catch { }
    }

    public async Task ConnectAsync()
    {
        while (!_cts.IsCancellationRequested)
        {
            try
            {
                if (_client == null || _client.Connected == false)
                {
                    isConnected = false;
                    CloseConnection();

                    Log($"Попытка подключения к {_serverIP}:{_serverPort}...");
                    _client = new TcpClient();
                    await _client.ConnectAsync(_serverIP, _serverPort, _cts.Token);
                    _stream = _client.GetStream();
                    isConnected = true;
                    Log("Успешно подключено!");
                    _connectionId = _client.Client.LocalEndPoint?.ToString() ?? "";

                    // Запускаем параллельные задачи на чтение и отправку
                    _ = Task.Run(() => ProcessReceivingAsync(_cts.Token));
                    _ = Task.Run(() => ProcessSendingAsync(_cts.Token));
                    _ = Task.Run(() => ProcessHeartbeatAsync(_cts.Token));

                    // Сразу отправляем пакет регистрации на сервере
                    byte id_lsb = (byte)(_deviceID & 0xFF);
                    byte id_msb = (byte)(_deviceID >> 8);
                    QueueMessage(new TCP_packet_type(0x91, id_msb, id_lsb));
                }
            }
            catch (Exception ex)
            {
                isConnected = false;
                Log($"[Ошибка подключения]: {ex.Message}. Повтор через 3 сек...");
                CloseConnection();
                await Task.Delay(3000, _cts.Token);
            }
            await Task.Delay(1000, _cts.Token);
        }
    }

    /* ================ Отправка пакетов ================*/

    public bool QueueMessage(TCP_packet_type message)
    {
        if (_messageQueue.IsAddingCompleted) 
            return false;
        else
            return _messageQueue.TryAdd(message); // Добавляем новый пакет в очередь на отправку
    }

    private async Task ProcessSendingAsync(CancellationToken ct)
    {
        try
        {
            foreach (var message in _messageQueue.GetConsumingEnumerable(ct))
            {
                byte[] sendBuffer = new byte[message.PacketLen];

                sendBuffer[0] = message.PacketLen;
                sendBuffer[1] = message.Header;

                if (message.Data != null && message.Data.Length > 0)
                {
                    Buffer.BlockCopy(message.Data, 0, sendBuffer, 2, message.Data.Length);
                }

                int retries = 0;
                bool isSent = false;

                Log($"Отправка пакета 0x{sendBuffer[1]:X2} на сервер");

                while (retries < _retryCount && isSent == false && !ct.IsCancellationRequested)
                {
                    try
                    {
                        if (_client != null && _client.Connected)
                        {
                            var stream = _client.GetStream();
                            await stream.WriteAsync(sendBuffer, 0, sendBuffer.Length, ct);  // Отправляем весь пакет целиком
                            isSent = true;
                        }
                        else { break; }
                    }
                    catch
                    {
                        retries++;
                        if (retries < _retryCount) await Task.Delay(500, ct);
                    }
                }

                if (!isSent)
                {
                    Log($"Не удалось отправить пакет с {retries} попыток");
                    Disconnect();   // Отключаемся если сообщение не удалось отправить _retryCount раз
                    break;
                }
            }
        }
        catch (OperationCanceledException) { }
    }

    /* ================ Прием пакетов ================*/

    private async Task ProcessReceivingAsync(CancellationToken ct)
    {
        if (_client == null) return;

        _stream = _client?.GetStream();

        const int dataLenSize = 1; // Длина пакета описывается 1 байтом
        byte[] receiveBuffer = ArrayPool<byte>.Shared.Rent(1024); // Выделяем буфер под входящие байты
        using var packetBuffer = new MemoryStream();

        while (!ct.IsCancellationRequested)
        {
            if (_stream == null) break;

            int bytesRead = await _stream.ReadAsync(receiveBuffer, ct); // Асинхронно читаем входящие байты из TCP-сокета

            if (bytesRead == 0) break; // Сервер закрыл сокет

            _timerTask = Task.Delay(10000, ct); // Перезапускаем таймер на 10 сек после которых будет происходить периодическая проверка подключения

            packetBuffer.Write(receiveBuffer, 0, bytesRead); // сохраняем принятое в буфер

            while (true) // Разбор накопившихся байтов
            {
                // Получаем span на все текущие байты в packetBuffer
                ReadOnlySpan<byte> bufferSpan = packetBuffer.GetBuffer().AsSpan(0, (int)packetBuffer.Length);

                // Если нет даже 1 байта — выходим из цикла до следующего ReadAsync
                if (bufferSpan.Length < dataLenSize) break;

                // Читаем длину пакета (первый байт)
                byte packetLen = bufferSpan[0];

                // Если все тело пакета еще не доехало — выходим из цикла до следующего ReadAsync
                if (bufferSpan.Length < packetLen) break;

                // Вырезаем тело пакета без копирования в новый массив
                ReadOnlySpan<byte> packetBody = bufferSpan[..packetLen];

                // Обработка пакета
                ProcessPayload(packetBody);

                // Сдвигаем буфер, удаляя обработанный пакет
                int remainingBytes = bufferSpan.Length - packetLen;
                if (remainingBytes > 0)
                {
                    packetBuffer.Position = 0;
                    packetBuffer.Write(bufferSpan.Slice(packetLen));
                    packetBuffer.SetLength(remainingBytes);
                }
                else
                {
                    packetBuffer.SetLength(0);
                    break; // Все данные обработаны, выходим в ожидание ReadAsync
                }
            }
        }
    }

    private void ProcessPayload(ReadOnlySpan<byte> payload)
    {
        TCP_packet_type new_packet = new TCP_packet_type
        {
            PacketLen = payload[0],
            Header = payload[1],
            Data = payload.Slice(2).ToArray()   // Делаем срез начиная с индекса 2 и до самого конца payload, а зтем копируем в массив Data
        };

        if (new_packet.Validate())
            NewPacketReceived?.Invoke(new_packet);  // После проверки целосности извещаем всех подписчиков про новый пакет
    }

    /* ============= Проверка соединения =============*/

    private async Task ProcessHeartbeatAsync(CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                await Task.Delay(10000, ct);
                // Периодически проверяем подключения только тогда, когда после последнего приема пакета прошло больше 10 секунд 
                // И если очередь на отправку пустая
                if ((_timerTask == null || (_timerTask != null && _timerTask.IsCompleted)) && _messageQueue.Count == 0)  
                {
                    byte id_lsb = (byte)(_deviceID & 0xFF);
                    byte id_msb = (byte)(_deviceID >> 8);
                    QueueMessage(new TCP_packet_type(0x91, id_msb, id_lsb));
                }
            }
        }
        catch (OperationCanceledException) {  }
    }

    public void Disconnect() // Вызывается при закрытии приложения
    {
        Log($"Вызван метод Disconnect()");

        if (_cts.IsCancellationRequested) return;
        _cts.Cancel();
        _messageQueue.CompleteAdding();
        _client?.Close();
    }

    public void Log(string message)
    {
        OnLogMessage?.Invoke($"[{DateTime.Now:mm:ss}] {message}");
    }
}
