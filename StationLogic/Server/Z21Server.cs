using System.Net;
using System.Net.Sockets;
using static ConsoleLog;

public class Z21Server {
    // Класс для хранения состояния клиента: endpoint + флаги рассылки.
    private class ClientState {
        public ClientState(IPEndPoint endPoint) {
            EndPoint = endPoint;
        }
        public IPEndPoint EndPoint; // Endpoint клиента
        public uint BroadcastFlags; // Флаги рассылки
    }

    // Данные сервера
    public const int Z21Port = 21105; // UDP-порт (По умолчанию для TrainController: 21105)
    private const int SerialNumber = 20260001; // Серийный номер сервера (уникальный для каждого экземпляра)
    public string Z21Host = "127.0.0.1"; // Адрес сервера (локальный)
    // Сервер
    private UdpClient? serverSocket; // UDP-сокет сервера
    private static IPEndPoint server_endPoint = new(IPAddress.Loopback, Z21Port); // Адрес сервера
    private CancellationTokenSource? cancellation; // Токен отмены для фоновой задачи сервера
    private Task? serverTask; // Фоновая задача сервера
    private CancellationTokenSource? cancelToken; // Токен отмены для остановки сервера
    // Клиенты
    private Dictionary<string, ClientState> clients = new(StringComparer.Ordinal); // Ключи всех клиентов
    private UdpClient socketUI; // UDP-сокет для отправки команд Z21 в UI (MainForm/ConsoleCmd.cs)
    public IPEndPoint? TC_endPoint; // Адрес TrainController
    // Обьект синхронизации
    private object sync = new();

    public event Action<bool>? status; //Статус сервера: true = запущен, false = остановлен.
    public event Action<ushort, byte, bool>? switchRead; // Событие чтения состояния свитча
    public event Action<ushort>? switchRequest; // Событие запроса состояния свитча
    public event Action? contactRequest; // Событие запроса состояния контакта
    public event Action<ushort, byte, byte>? extAccessoryRead; // Событие чтения состояния extended accessory

    // Данные
    private bool statePower; // Напряжение на рельсах: true = включено, false = выключено.
    private Dictionary<int, byte> sw = new(); // Состояние всех свитчей: адрес + позиции
    private Dictionary<int, (byte State, byte Data)> extAccessories = new(); // Состояние extended accessories: адрес + state/data

    // Цвета для сообщений в консоли
    private Color hideColor = Color.FromArgb(60, 60, 60); // Цвет для скрытых сообщений
    private Color hideColorRx = Color.FromArgb(60, 120, 60); // Цвет для скрытых сообщений чтения
    private Color hideColorRxHex = Color.FromArgb(60, 80, 60); // Цвет для скрытых сообщений чтения
    private Color hideColorTx = Color.FromArgb(60, 60, 120); // Цвет для скрытых сообщений отправки
    private Color hideColorTxHex = Color.FromArgb(60, 60, 80); // Цвет для скрытых сообщений отправки

    public bool showDebug = false; // Флаг для отображения отладочных сообщений в консоли
    private void printDebug(string message, Color color = default) {
        if (showDebug) {
            print(message, color);
        }
    }

    // Конструктор сервера
    public Z21Server(UdpClient newSocket) {
        socketUI = newSocket;
        stopUDPConnReset(socketUI.Client); // Отключаем ConnReset
    }

    // Освобождение ресурсов сокетов и токена отмены
    public void Dispose() {
        cancelToken?.Cancel(); // Отмена текущей задачи сервера
        serverSocket?.Dispose(); // Освобождение ресурсов сокета сервера
        cancelToken?.Dispose(); // Освобождение ресурсов токена отмены
        socketUI.Dispose(); // Освобождение ресурсов сокета Z21
    }

    // Запуск сервера
    public void start() {
        lock (sync) { // Синхронизация доступа к полям сервера
            // Если сервер уже запущен, выходим
            if (serverTask is { IsCompleted: false }) {
                printDebug("Сервер уже запущен.", Color.Yellow);
                status?.Invoke(true); // Уведомляем UI, что сервер запущен
                return;
            }

            try {
                // Создаём UDP-сокет
                UdpClient newServerSocket = new UdpClient(AddressFamily.InterNetwork);
                newServerSocket.Client.Bind(server_endPoint); // Привязываем сокет к адресу
                stopUDPConnReset(newServerSocket.Client); // Отключаем ConnReset, чтобы не получать исключения при ICMP Port Unreachable.
                // При новом запуске очищаем старые клиенты, endpoint TC и состояние стрелок.
                clients.Clear(); // Очищаем список клиентов
                sw.Clear(); // Очищаем состояние всех стрелок
                extAccessories.Clear(); // Очищаем состояние extended accessories
                TC_endPoint = null; // Сбрасываем endpoint TrainController
                statePower = false; // Сбрасываем состояние питания пути

                cancellation = new CancellationTokenSource(); // Создаём токен отмены для фоновой задачи сервера
                serverSocket = newServerSocket; // Сохраняем сокет сервера
                serverTask = process(serverSocket, cancellation.Token); // Запускаем фоновую задачу сервера
            // Если произошла ошибка сокета
            } catch (SocketException ex) {
                printDebug($"Не удалось запустить Z21 server: {printErrorSocket(ex)}", Color.Red);
                status?.Invoke(false); // Уведомляем UI, что сервер отключён
                return;
            // Если произошла другая ошибка
            } catch (Exception ex) {
                printDebug($"Не удалось запустить Z21 server: {ex.Message}", Color.Red);
                status?.Invoke(false); // Уведомляем UI, что сервер отключён
                return;
            }
        }

        printDebug($"Z21 server слушает на udp://{server_endPoint.Address}:{server_endPoint.Port}", hideColor);
        status?.Invoke(true); // Уведомляем UI, что сервер запущен
    }

    // Остановка сервера
    public async Task stop() {
        UdpClient? socket;
        CancellationTokenSource? token;
        Task? task;

        lock (sync) { // Синхронизация доступа к полям сервера
            socket = serverSocket; // Сохраняем текущий сокет сервера
            token = cancelToken; // Сохраняем текущий токен отмены
            task = serverTask; // Сохраняем текущую задачу сервера

            serverSocket = null; // Сбрасываем сокет сервера
            cancelToken = null; // Сбрасываем токен отмены
            serverTask = null; // Сбрасываем задачу сервера
        }

        // Если сервер уже остановлен, выходим
        if (socket is null) {
            return;
        }

        status?.Invoke(false); // Уведомляем UI, что сервер остановлен
        printDebug("Z21 server остановлен...");

        token?.Cancel(); // Отмена текущей задачи сервера
        socket.Dispose(); // Освобождение ресурсов сокета

        // Если задача сервера ещё не завершена
        if (task is not null) {
            try {
                await task; // Ожидание завершения задачи сервера
            // Игнорируем исключения, если задача уже завершена
            } catch (OperationCanceledException) {
            }
        }

        token?.Dispose(); // Освобождение ресурсов токена отмены

        printDebug(new string('-', 80));
        printDebug("Z21 server остановлен.");
    }

    // Главный цикл сервера: получает UDP-пакеты, регистрирует клиента и передаёт payload в парсер.
    private async Task process(UdpClient serverSocket, CancellationToken cancellationToken) {
        try {
            // Пока токен отмены не был вызван
            while (!cancellationToken.IsCancellationRequested) {
                UdpReceiveResult received; // Результат получения UDP-пакета

                try {
                    // Ожидаем следующий UDP datagram от TC или другого клиента.
                    received = await serverSocket.ReceiveAsync(cancellationToken);
                // Если токен отмены был вызван
                } catch (OperationCanceledException) {
                    break;
                // Если сокет был закрыт
                } catch (ObjectDisposedException) {
                    break;
                // Если токен отмены был вызван во время ожидания сокета
                } catch (SocketException ex) when (cancellationToken.IsCancellationRequested) {
                    printDebug($"Z21 server остановлен во время ожидания сокета: {printErrorSocket(ex)}", Color.Red);
                    break;
                // Если произошла ошибка сокета
                } catch (SocketException ex) {
                    printDebug($"recvfrom failed: {printErrorSocket(ex)}", Color.Red);
                    continue;
                }

                // Клонируем endpoint для многопоточной безопасности
                IPEndPoint remoteEndPoint = cloneEndpoint(received.RemoteEndPoint);

                updateClient(remoteEndPoint); // Обновляем/создаём клиента в списке
                saveTCendpoint(remoteEndPoint, received.Buffer); // Сохраняем endpoint TrainController
                await searchPacket(remoteEndPoint, received.Buffer); // Поиск пакетов пока они есть
            }
        // Обработка всех исключений, кроме отмены токена
        } catch (Exception ex) when (ex is not OperationCanceledException) { 
            printDebug($"Z21 server завершился с ошибкой: {ex.Message}", Color.Red);
        // Если токен отмены был вызван - выходим без ошибок
        } finally {
            finish(); // Завершаем сервер и уведомляем UI
        }
    }

    // Процесс завершения сервера при отмене сокета
    private void finish() {
        lock (sync) { // Синхронизация доступа к полям сервера
            serverSocket = null; // Сбрасываем сокет сервера
            cancelToken?.Dispose(); // Освобождаем токен отмены
            cancelToken = null; // Сбрасываем токен отмены
            serverTask = null; // Сбрасываем задачу сервера
        }

        printDebug(new string('-', 80));
        printDebug("Z21 server остановлен.");
        status?.Invoke(false); // Уведомляем UI, что сервер остановлен
    }

    // Отключение сброса соединения UDP ConnReset-ошибки
    private static void stopUDPConnReset(Socket socket) {
        try {
            // Отключение ConnReset
            socket.IOControl(
                (IOControlCode)unchecked((int)0x9800000C), // SIO_UDP_CONNRESET
                BitConverter.GetBytes(0), // Отключаем ConnReset
                null // Не используем выходной буфер
            );
        // Если платформа не поддерживает IOControl, выходим без ошибок
        } catch (PlatformNotSupportedException) {
        // Если произошла ошибка сокета, выходим без ошибок
        } catch (SocketException) {
        // Если сокет был закрыт, выходим без ошибок
        } catch (ObjectDisposedException) {
        }
    }
    // Регистрация/обновление UDP-клиента
    private void updateClient(IPEndPoint endPoint) {
        string key = endPoint.ToString(); // Получаем ключ клиента
        lock (sync) { // Синхронизация доступа к словарю клиентов
            // Если клиент уже есть, обновляем его endpoint
            if (clients.TryGetValue(key, out ClientState? clientState)) {
                clientState.EndPoint = cloneEndpoint(endPoint);
            // Если клиента нет, создаём нового
            } else {
                clients[key] = new ClientState(cloneEndpoint(endPoint));
            }
        }
    }

    // Клонирование IPEndPoint (независимая)
    private static IPEndPoint cloneEndpoint(IPEndPoint endPoint) {
        return new IPEndPoint(endPoint.Address, endPoint.Port);
    }

    // Удаление клиента
    private void removeClient(IPEndPoint endPoint) {
        string key = endPoint.ToString(); // Получаем ключ клиента
        lock (sync) { // Синхронизация доступа к словарю клиентов
            clients.Remove(key); // Удаляем клиента из словаря
            // Если удаляемый клиент был TrainController, сбрасываем endpoint для него тоже.
            if (TC_endPoint?.ToString() == key) {
                TC_endPoint = null;
            }
        }
    }

    // Сохранение endpoint TrainController
    private void saveTCendpoint(IPEndPoint endPoint, byte[] data) {
        int offset = 0; // Начальный offset для чтения

        // Выполняем пока есть хотя бы 4 байта для чтения
        while (offset + 4 <= data.Length) {
            int frameLength = data[offset]; // Длина пакета
            // Если длина пакета меньше 4 байт или превышает оставшуюся длину payload - выходим
            if ( (frameLength < 4) || (offset + frameLength > data.Length) ) {
                return;
            }
            byte header = data[offset + 2];
            if (header == 0x10 // LAN_GET_SERIAL_NUMBER
            || header == 0x1A // LAN_GET_HWINFO
            || header == 0x50 // LAN_SET_BROADCASTFLAGS
            || header == 0x51 // LAN_GET_BROADCASTFLAGS
            || header == 0xC4 // LAN_CAN_DETECTOR
            || (header == 0x40 // LAN_X
            && frameLength >= 0x07 // Длина пакета должна быть не меньше 7 байт
            && data[offset + 4] == 0x21 // Системные X-команды
            && data[offset + 5] == 0x24) ) { // LAN_X_GET_STATUS
                lock (sync) { // Синхронизация доступа к полям сервера
                    TC_endPoint = cloneEndpoint(endPoint); // Сохраняем endpoint TrainController
                }
                return;
            }
            offset += frameLength;
        }
    }

    // Сохранение подписок клиента (какие события он хочет получать автоматически)
    private void saveBroadcastFlags(IPEndPoint remoteEndPoint, uint flags) {
        string key = remoteEndPoint.ToString(); // Получаем ключ клиента
        lock (sync) { // Синхронизация доступа к словарю клиентов
            // Если клиента нет, создаём нового
            if (!clients.TryGetValue(key, out ClientState? clientState)) {
                clientState = new ClientState(cloneEndpoint(remoteEndPoint));
                clients[key] = clientState;
            }
            clientState.BroadcastFlags = flags; // Сохраняем флаги рассылки для клиента
        }
    }

    // Вычисление XOR для пакета
    private static byte calcXOR(byte[] packet, int startByte, int endByte) {
        byte value = 0;
        for (int i = startByte; i < endByte; i++) {
            value ^= packet[i];
        }
        return value;
    }

    // Преобразование IPEndPoint в строку
    private static string FormatEndpoint(IPEndPoint endPoint) {
        return $"{endPoint.Address}:{endPoint.Port}";
    }

    // Преобразование байт в hex-строку
    private static string hex(byte[] bytes) {
        return BitConverter.ToString(bytes).Replace('-', ' ');
    }

    // Декодирование 2 байт в 11-битный адрес (адрес аксессуара/стрелки/свитча).
    private static ushort DecodeAccessoryAddress(byte high, byte low) {
        return (ushort)(((high << 8) | low) & 0x07FF);
    }

    // Вывод сообщения с ошибкой для сокета
    private string printErrorSocket(SocketException ex) {
        return ex.SocketErrorCode == SocketError.AddressAlreadyInUse
            ? $"порт {Z21Host}:{Z21Port} уже занят"
            : ex.Message;
    }

    //////// Чтение состояния стрелки и отправка его к TrainController ////////

    // Поиск пакетов
    private async Task searchPacket(IPEndPoint remoteEndPoint, byte[] data) {
        int n = 0;
        // Пока есть хотя бы 4 байта для чтения
        while (n + 4 <= data.Length) {
            int size = data[n]; // Длина пакета
            // Если длина пакета меньше 4 байт или превышает оставшуюся длину payload - выходим
            if (size < 4 || n + size > data.Length) {
                return;
            }

            byte[] frame = data[n..(n + size)]; // Вырезание части массива для одного пакета
            await readPacket(remoteEndPoint, frame);
            n += size;
        }
    }

    // Обработка принятых пакетов
    // serverSocket - сокет сервера, remoteEndPoint - endpoint клиента, frame - принятый пакет, cancellationToken - токен отмены.
    private async Task readPacket(IPEndPoint remoteEndPoint, byte[] frame) {
        switch (frame[2]) {
            // LAN_GET_SERIAL_NUMBER: опрос серийного номера станции
            case 0x10 when frame.Length == 0x04:{
                printDebug($"[Rx][{FormatEndpoint(remoteEndPoint)}][LAN_GET_SERIAL_NUMBER]", hideColorRx);
                await sendSerialNumber(remoteEndPoint);
                break;
            }
            // LAN_GET_HWINFO: опрос типа станции и версии прошивки
            case 0x1A when frame.Length == 0x04: {
                printDebug($"[Rx][{FormatEndpoint(remoteEndPoint)}][LAN_GET_HWINFO]", hideColorRx);
                await sendHwInfo(remoteEndPoint);
                break;
            }
            // LAN_LOGOFF: клиент отключается от сервера
            case 0x30 when frame.Length == 0x04: {
                printDebug($"[Rx][{FormatEndpoint(remoteEndPoint)}][LAN_LOGOFF]", hideColorRx);
                removeClient(remoteEndPoint); // Удаляем клиента из списка
                break;
            }
            // LAN_SET_BROADCASTFLAGS: подписка клиента на рассылку (какие события ему отправлять автоматически)
            case 0x50 when frame.Length == 0x08: {
                uint flags =
                    (uint)frame[4] |
                    ((uint)frame[5] << 8) |
                    ((uint)frame[6] << 16) |
                    ((uint)frame[7] << 24);

                string flagsText = "";

                if ((flags & 0x00000001) != 0) flagsText += "Driving/Switching, ";
                if ((flags & 0x00000002) != 0) flagsText += "R-BUS, ";
                if ((flags & 0x00000004) != 0) flagsText += "RailCom, ";
                if ((flags & 0x00000010) != 0) flagsText += "FastClock, ";
                if ((flags & 0x00000100) != 0) flagsText += "SystemState, ";
                if ((flags & 0x00010000) != 0) flagsText += "All loco info, ";
                if ((flags & 0x00020000) != 0) flagsText += "CAN booster, ";
                if ((flags & 0x00040000) != 0) flagsText += "RailCom all, ";
                if ((flags & 0x00080000) != 0) flagsText += "CAN detector, ";
                if ((flags & 0x01000000) != 0) flagsText += "LocoNet general, ";
                if ((flags & 0x02000000) != 0) flagsText += "LocoNet locos, ";
                if ((flags & 0x04000000) != 0) flagsText += "LocoNet switches, ";
                if ((flags & 0x08000000) != 0) flagsText += "LocoNet detectors, ";

                if (flagsText.Length >= 2) {
                    flagsText = flagsText[..^2];
                } else {
                    flagsText = "none";
                }

                printDebug(
                    $"[Rx][{FormatEndpoint(remoteEndPoint)}][LAN_SET_BROADCASTFLAGS]"
                    + $"[flags=0x{flags:X8}][{flagsText}]",
                    hideColorRx
                );

                saveBroadcastFlags(remoteEndPoint, flags);
                break;
            }
            // LAN_GET_BROADCASTFLAGS: опрос флагов рассылки от клиента
            case 0x51 when frame.Length == 0x04: {
                printDebug($"[Rx][{FormatEndpoint(remoteEndPoint)}][LAN_GET_BROADCASTFLAGS]", hideColorRx);
                await sendBroadcastFlags(remoteEndPoint);
                break;
            }

            
            // LAN_SYSTEMSTATE_GETDATA: опрос состояния станции (ток, напряжение, температура, central state)
            case 0x85 when frame.Length == 0x04: {
                printDebug($"[Rx][{FormatEndpoint(remoteEndPoint)}][LAN_SYSTEMSTATE_GETDATA]", hideColorRx);
                await sendSystemData(remoteEndPoint);
                break;
            }
            // LAN_CAN_DETECTOR: чтение состояния входа CAN
            case 0xC4: {
                // TC запрашивает все CAN-контакты
                if (frame.Length == 0x07) {
                    byte type = frame[4];
                    ushort networkId =
                        (ushort)(frame[5] | (frame[6] << 8));

                    if (type == 0x00 &&
                        (networkId == 0xD000 || networkId == 0xD001)) {
                        contactRequest?.Invoke();
                    }
                }
                // Сообщение состояния одного CAN-контакта
                else if (frame.Length == 0x0E) {
                    int addr = (frame[6] | (frame[7] << 8)) + 1;
                    int input = frame[8] + 1;
                    bool state =
                        ((ushort)(frame[10] | (frame[11] << 8)) & 0x1000) != 0;

                    printDebug(
                        $"[Rx][{FormatEndpoint(remoteEndPoint)}][LAN_CAN_DETECTOR]"
                        + $"[addr={addr}][input={input}]"
                        + $"[state={state}]",
                        hideColorRx
                    );
                }

                break;
            }
            // LAN_X: X-BUS/XpressNet-команды
            case 0x40: {
                // Если длина пакета меньше 7 байт - выходим
                if (frame.Length < 0x07) {
                    return;
                }
                switch (frame[4]) {
                    // Системные X-команды
                    case 0x21 when frame.Length == 0x07:
                        switch (frame[5]) {
                            // LAN_X_GET_VERSION: опрос версии протокола X-BUS
                            case 0x21: {
                                printDebug($"[Rx][{FormatEndpoint(remoteEndPoint)}][LAN_X_GET_VERSION]", hideColorRx);
                                await sendXBusVersion(remoteEndPoint);
                                break;
                            }
                            // LAN_X_GET_STATUS: опрос состояния станции (питание, напряжение, аварийные события)
                            case 0x24: {
                                // printDebug($"[Rx][{FormatEndpoint(remoteEndPoint)}][LAN_X_GET_STATUS]", hideColorRx);
                                await sendSystemStatus(remoteEndPoint);
                                return;
                            }
                            // LAN_X_SET_TRACK_POWER_OFF: прием об отключении питания пути
                            case 0x80:
                                printDebug($"[Rx][{FormatEndpoint(remoteEndPoint)}][LAN_X_SET_TRACK_POWER_OFF]", hideColorRx);
                                statePower = false;
                                break;
                            // LAN_X_SET_TRACK_POWER_ON: прием об включении питания пути
                            case 0x81:
                                printDebug($"[Rx][{FormatEndpoint(remoteEndPoint)}][LAN_X_SET_TRACK_POWER_ON]", hideColorRx);
                                statePower = true;
                                break;
                            default: // неизвестная команда
                                printDebug($"[Rx][{FormatEndpoint(remoteEndPoint)}][LAN_X_UNKNOWN]: {hex(frame)}", hideColorRx);
                                await sendUnknownCmdReply(remoteEndPoint);
                                break;
                        }
                        break;

                    // LAN_X_GET_TURNOUT_INFO: TC спрашивает текущее положение стрелки.
                    case 0x43 when frame.Length == 0x08: {
                        ushort address = (ushort)(DecodeAccessoryAddress(frame[5], frame[6]) + 1);
                        switchRequest?.Invoke(address);
                        break;
                    }

                    // LAN_X_SET_TURNOUT: чтение состояния стрелки
                    case 0x53 when frame.Length == 0x09: {
                        ushort address = DecodeAccessoryAddress(frame[5], frame[6]);
                        address++; // В протоколе Z21 адреса стрелок начинаются с 0, а в TrainController с 1.
                        byte position = (byte)((frame[7] & 0x01) != 0 ? 0x02 : 0x01);
                        bool activate = (frame[7] & 0x08) != 0;
                        sw[address] = position;
                        printDebug($"[Rx][{FormatEndpoint(remoteEndPoint)}][LAN_X_SET_TURNOUT]"
                            + $"[addr={address}][pos={(position==0x01 ? "THROWN" : "CLOSED")}][Activate={activate}]", hideColorRx);
                        switchRead?.Invoke(address, position, activate);
                        break;
                    }

                    // LAN_X_SET_EXT_ACCESSORY: расширенный аксессуар/сигнал
                    case 0x54 when frame.Length == 0x0A: {
                        int address = DecodeAccessoryAddress(frame[5], frame[6]) - 3;
                        byte state = frame[7]; // состояние/aspect сигнала

                        extAccessories[address] = (State: state, Data: (byte)0x00);

                        printDebug($"[Rx][{FormatEndpoint(remoteEndPoint)}][LAN_X_SET_EXT_ACCESSORY]"
                            + $"[{address}][state={(state == 1 ? "ON" : "OFF")}]",
                            hideColorRx
                        );

                        extAccessoryRead?.Invoke((ushort)address, state, (byte)0x00);
                        await sendExtAccessoryInfo(remoteEndPoint, address, state);
                        break;
                    }

                    default: // неизвестная команда
                        await sendUnknownCmdReply(remoteEndPoint);
                        break;
                }
                break;
            }
        }
        printDebug($"[Rx][{FormatEndpoint(remoteEndPoint)}][{hex(frame)}]", hideColorRxHex);
    }

    //////// Работа с отправкой пакетов клиенту ////////

    // Отправка пакета клиенту
    private async Task sendPacket(IPEndPoint target, byte[] packet, bool enaShow = true) {
        UdpClient? socket = serverSocket;
        if (socket is null) {
            return;
        }

        CancellationToken token = cancellation?.Token ?? CancellationToken.None;
        token.ThrowIfCancellationRequested();

        await socket.SendAsync(packet, target);
        if (enaShow) {
            printDebug($"[Tx][{FormatEndpoint(target)}][{hex(packet)}]", hideColorTxHex);
        }
    }

    // Отправка пакета на неизвестную команду
    public async Task sendUnknownCmdReply(IPEndPoint target) {
        byte[] packet = [0x07, 0x00, 0x40, 0x00, 0x61, 0x82, 0xE3];
        await sendPacket(target, packet);
    }

    // Отправка серийного номера станции клиенту.
    public async Task sendSerialNumber(IPEndPoint target) {
        byte[] packet = [
                    0x08, 0x00, // DataLen
                    0x10, 0x00, //Header
                    // Data: 4 байта серийного номера, little-endian
                    unchecked((byte)(SerialNumber >> 0)),
                    unchecked((byte)(SerialNumber >> 8)),
                    unchecked((byte)(SerialNumber >> 16)),
                    unchecked((byte)(SerialNumber >> 24))
                ];
        await sendPacket(target, packet);
        // printDebug($"[Tx][{FormatEndpoint(target)}][LAN_GET_SERIAL_NUMBER][SerialNumber={SerialNumber}]", hideColorTx);
    }

    // Отправка типа станции и версии прошивки клиенту.
    public async Task sendHwInfo(IPEndPoint target) {
        const uint HwTypeZ21New = 0x00000201; // Тип станции (201 = новая Z21)
        byte[] packet = [
            0x0C, 0x00, //DataLen
            0x1A, 0x00, //Header
            //Data: 4 байта типа станции, 2 байта версии прошивки, 2 байта reserved
            unchecked((byte)(HwTypeZ21New >> 0)),
            unchecked((byte)(HwTypeZ21New >> 8)),
            unchecked((byte)(HwTypeZ21New >> 16)),
            unchecked((byte)(HwTypeZ21New >> 24)),
            0x43, 0x01, 0x00, 0x00 // Version: 0x01 0x43 = 1.43, reserved: 0x00 0x00
        ];
        await sendPacket(target, packet);
        printDebug($"[Tx][{FormatEndpoint(target)}][LAN_GET_HWINFO][HwType=0x{HwTypeZ21New:X8}][Version=1.43]", hideColorTx);
    }

    // Отправка флагов рассылки клиента
    public async Task sendBroadcastFlags(IPEndPoint target) {
        uint flags; // Флаги рассылки клиента

        lock (sync) { // Синхронизация доступа к словарю клиентов
            // Получаем текущие флаги рассылки для клиента
            flags = clients.TryGetValue(target.ToString(), out ClientState? clientState)
                ? clientState.BroadcastFlags
                : 0u;
        }

        byte[] packet = [
            0x08, 0x00, // DataLen
            0x51, 0x00, // Header
            // Data: 4 байта флагов рассылки
            (byte)(flags >> 0),
            (byte)(flags >> 8),
            (byte)(flags >> 16),
            (byte)(flags >> 24)
        ];
        await sendPacket(target, packet);
        printDebug($"[Tx][{FormatEndpoint(target)}][LAN_GET_BROADCASTFLAGS][Flags=0x{flags:X8}]", hideColorTx);
    }

    // Отправка состояния текущей станции (ток, напряжение, температура, central state)
    public async Task sendSystemData(IPEndPoint target) {
        ushort mainCurrent = statePower ? (ushort)250 : (ushort)0; 
        ushort supplyVoltage = statePower ? (ushort)18000 : (ushort)0;
        byte[] packet = [
            0x14, 0x00, // DataLen
            0x84, 0x00, // Header
            // MainCurrent: 2 байта тока главного пути, мА
            (byte)(mainCurrent >> 0), (byte)(mainCurrent >> 8),
            // ProgCurrent: 2 байта тока программного пути, мА
            0x00, 0x00,
            // FilteredMainCurrent: 2 байта сглаженного тока главного пути, мА
            (byte)(mainCurrent >> 0), (byte)(mainCurrent >> 8),
            // Temperature: 2 байта внутренней температуры, градусы Цельсия
            30, 0x00,
            // SupplyVoltage: 2 байта напряжения питания станции, мВ
            (byte)(supplyVoltage >> 0), (byte)(supplyVoltage >> 8),
            // VCCVoltage: 2 байта внутреннего напряжения станции, мВ
            0x88, 0x13,
            // CentralState: основное состояние станции (Напряжение на рельсах отключено = 0x02)
            (byte)(statePower ? 0x00 : 0x02),
            // CentralStateEx: расширенное состояние станции
            0x00,
            // reserved: резервный байт
            0x00,
            // Capabilities: возможности станции (0х21 = поддержка DCC + прием LAN-команд для декодеров/стрелок)
            0x21
        ];
        await sendPacket(target, packet);
        string message =
            $"[Tx][{FormatEndpoint(target)}]"
            + $"[LAN_SYSTEMSTATE_GETDATA]"
            + $"[{(statePower ? "ON" : "OFF")}]"
            + $"[MainCurrent={mainCurrent}mA]"
            + $"[SupplyVoltage={supplyVoltage}mV]"
            + $"[CentralState={(statePower ? "ON" : "OFF")}]";
        printDebug(message, hideColorTx);
    }

    // Отправка версии протокола X-BUS клиенту
    public async Task sendXBusVersion(IPEndPoint target) {
        byte[] packet = [
            0x09, 0x00, // DataLen
            0x40, 0x00, // Header
            // Data: X-Header, DB0, DB1, DB2, XOR
            0x63, 0x21,
            0x40, // XBUS_VERSION: 0x30 = V3.0, 0x36 = V3.6, 0x40 = V4.0, … 
            0x12, //CMDST_ID: Command station ID (0x12 = Z21 device family)
            0x00 // reserved
        ];
        packet[^1] = calcXOR(packet, 4, packet.Length - 1); // XOR пересчитывается последним байтом
        await sendPacket(target, packet); // Отправка ответа
        printDebug($"[Tx][{FormatEndpoint(target)}][LAN_X_GET_VERSION][XBUS_VERSION={packet[6]:X2}][CMDST_ID={packet[7]:X2}]", hideColorTx);
    }

    // Отправка состояния текущей станции (питание, напряжение, аварийные события)
    public async Task sendSystemStatus(IPEndPoint target) {
        byte[] packet = [
            0x08, 0x00, // DataLen
            0x40, 0x00, // Header
            // Data: X-Header, DB0, DB1, XOR
            0x62, 0x22,
            // 0x01 - аварийное отключение питания
            // 0x02 - отключено напряжение на рельсах
            // 0x04 - короткое замыкание
            // 0x20 - режим программирования активен
            statePower ? (byte)0x00 : (byte)0x02,
            0x00 // reserved
        ];
        packet[^1] = calcXOR(packet, 4, packet.Length - 1);
        await sendPacket(target, packet, false);
        // printDebug($"[Tx][{FormatEndpoint(target)}][LAN_X_GET_STATUS][{(statePower ? "ON" : "OFF")}]", hideColorTx);
    }

    // Отправка текущего положения стрелки
    public async Task sendTurnoutInfo(IPEndPoint target, int addr, byte position, bool activate = true) {
        printDebug($"[Tx][{FormatEndpoint(target)}][LAN_X_TURNOUT_INFO][addr={addr}]" 
        + $"[{(position==0x01 ? "THROWN" : "CLOSED")}]", hideColorTx);
        addr--; // Преобразуем адрес в Z21-адрес (отнимаем 1)
        byte[] packet = [
            0x09, 0x00, // DataLen
            0x40, 0x00, // Header = LAN_X

            0x43, // X-Header = LAN_X_TURNOUT_INFO
            (byte)((addr >> 8) & 0x07),
            (byte)(addr & 0xFF),
            position,
            0x00 // XOR
        ];

        packet[^1] = calcXOR(packet, 4, packet.Length - 1);
        await sendPacket(target, packet);
    }

    // Отправка состояния питания пути к TrainController
    // LAN_X_BC_TRACK_POWER_OFF / LAN_X_BC_TRACK_POWER_ON
    public async Task sendPowerStatus(bool state) {
        statePower = state;

        // Если TC ещё не зарегистрирован, выходим.
        if (TC_endPoint is null) {
            return;
        }

        byte db0 = state ? (byte)0x01 : (byte)0x00; // DB0: 0x01 = питание пути включено, 0x00 = питание пути выключено
        byte xorByte = state ? (byte)0x60 : (byte)0x61;

        byte[] packet = [
            0x07, 0x00, // DataLen  
            0x40, 0x00, // Header
            0x61, // X-Header
            db0, // DB0
            xorByte // XOR
        ];

        printDebug($"[Tx][{FormatEndpoint(TC_endPoint)}][LAN_X_BC_TRACK_POWER_{(state ? "ON" : "OFF")}]", hideColorTx);
        await sendPacket(TC_endPoint, packet);
    }

    // Отправка состояния extended accessory в TrainController.
    // LAN_X_EXT_ACCESSORY_INFO
    public async Task sendExtAccessoryInfo(IPEndPoint target, int addr, byte state) {
        int z21Address = addr + 3;

        byte[] packet = [
            0x0A, 0x00, // DataLen = 10
            0x40, 0x00, // Header = LAN_X

            0x44, // X-Header = LAN_X_EXT_ACCESSORY_INFO
            (byte)((z21Address >> 8) & 0x07),
            (byte)(z21Address & 0xFF),
            state, // DDDDDDDD: состояние/aspect extended accessory
            0x00,  // Status: 0x00 = Data Valid
            0x00   // XOR
        ];

        packet[^1] = calcXOR(packet, 4, packet.Length - 1);

        await sendPacket(target, packet);

        printDebug(
            $"[Tx][{FormatEndpoint(target)}][LAN_X_EXT_ACCESSORY_INFO][addr={addr}][state={state}]",
            hideColorTx
        );
    }

    // Назначение нового положения стрелки и отправка его к TrainController
    public async Task setSwitch(int addr, bool position) {
        if (addr < 0 || 2048 <= addr) {
            printDebug($"Недопустимый адрес стрелки: {addr}.", Color.Red);
            return;
        }

        // Если TC ещё не зарегистрирован, выходим.
        if (TC_endPoint is null) {
            return;
        }

        sw[addr] = position ? (byte)0x01 : (byte)0x02;

        await sendTurnoutInfo(TC_endPoint, addr, sw[addr]);
    }
    
    // Назначение нового состояния питания пути и отправка его к TrainController
    public async Task setPowerStatus(bool state) {
        // Если TC ещё не зарегистрирован, выходим.
        if (TC_endPoint is null) {
            return;
        }

        await sendPowerStatus(state);
    }

    // Назначение нового состояния extended accessory и отправка его к TrainController
    public async Task setExtAccessory(int addr, int state) {
        // Если TC ещё не зарегистрирован, выходим.
        if (TC_endPoint is null) {
            return;
        }

        extAccessories[addr] = ((byte)state, 0x00);
        await sendExtAccessoryInfo(TC_endPoint, addr, (byte)state);
    }

    // Отправка состояния входа R-BUS к TrainController
    public async Task sendRBusInput(IPEndPoint target, int addr, int input, bool active) {
        if (addr < 1 || addr > 20) {
            return;
        }

        if (input < 1 || input > 8) {
            return;
        }

        int groupIndex = (addr - 1) / 10; // 0 = модули 1..10, 1 = модули 11..20
        int moduleIndex = (addr - 1) % 10; // индекс модуля внутри группы 0..9
        byte bitMask = (byte)(1 << (input - 1));

        byte[] packet = [
            0x0F, 0x00, // DataLen = 15
            0x80, 0x00, // Header = LAN_RMBUS_DATACHANGED
            (byte)groupIndex,

            0x00, // module 1/11
            0x00, // module 2/12
            0x00, // module 3/13
            0x00, // module 4/14
            0x00, // module 5/15
            0x00, // module 6/16
            0x00, // module 7/17
            0x00, // module 8/18
            0x00, // module 9/19
            0x00  // module 10/20
        ];

        packet[5 + moduleIndex] = active ? bitMask : (byte)0x00;

        await sendPacket(target, packet);

        printDebug(
            $"[Tx][{FormatEndpoint(target)}][LAN_RMBUS_DATACHANGED]" +
            $"[module={addr}][input={input}][active={active}]",
            hideColorTx
        );
    }

    // Назначение нового состояния входа R-BUS и отправка его к TrainController
    public async Task setRBusInput(int addr, int input, bool active) {
        if (TC_endPoint is null) {
            return;
        }

        await sendRBusInput(TC_endPoint, addr, input, active);
    }

    // Отправка состояния входа CAN к TrainController
    public async Task sendCanInput(IPEndPoint target, int addr, int input, bool active) {
        if (addr < 1 || addr > 65535) {
            return;
        }

        if (input < 1 || input > 8) {
            return;
        }
        ushort networkId = 0xD001;
        printDebug(
            $"[Tx][{FormatEndpoint(target)}][LAN_CAN_DETECTOR]" +
            $"[networkId=0x{networkId:X4}][addr={addr}][input={input}][active={active}]",
            hideColorTx
        );

        addr--;


        byte port = (byte)(input - 1);
        ushort value1 = active ? (ushort)0x1100 : (ushort)0x0100;

        byte[] packet = [
            0x0E, 0x00, // DataLen = 14
            0xC4, 0x00, // Header = LAN_CAN_DETECTOR

            (byte)(networkId >> 0),
            (byte)(networkId >> 8),

            (byte)(addr >> 0),
            (byte)(addr >> 8),

            port,
            0x01, // Type = occupancy status

            (byte)(value1 >> 0),
            (byte)(value1 >> 8),

            0x00,
            0x00
        ];

        await sendPacket(target, packet);

    }

    // Назначение нового состояния входа CAN и отправка его к TrainController
    public async Task setCanInput(int addr, int input, bool active) {
        if (TC_endPoint is null) {
            return;
        }

        await sendCanInput(TC_endPoint, addr, input, active);
    }



}
