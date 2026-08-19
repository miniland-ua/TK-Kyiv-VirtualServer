using System.Net.Sockets;

// Главный файл формы: поля, конструктор и обработчики событий.
partial class MainForm : Form
{
    public Z21Server server = new Z21Server(new(AddressFamily.InterNetwork)); // Объект сервера Z21
    public Station sta; // Объект станции.
    bool _closing; // Флаг, чтобы закрытие формы не запустилось повторно.
    bool _updatingServerCheckBox;


    static  TcpClientManager? tcp_client;
    static ushort clientID = 10101;
    static ushort port = 21201;
    
    public MainForm()
    {
        InitializeComponent(); // Создает элементы формы из Designer.
        sta = new Station(server); // Создаем объект станции с сервером Z21.
        sta.server.status += SetZ21ServerStatus; // Меняем текст статуса при запуске/остановке.

        tcp_client = new TcpClientManager("127.0.0.1", port, clientID);
        tcp_client.OnLogMessage += message => print(message);
        tcp_client.NewPacketReceived += sta.readTCP;
        _= tcp_client.ConnectAsync();

        helpButton.Click += helpButton_Click; // Кнопка Help.
        clearButton.Click += clearButton_Click; // Очистка окна лога.
        sendButton.Click += sendButton_Click; // Отправка команды из нижнего поля.
        serverCheckBox.CheckedChanged += serverCheckBox_CheckedChanged;
        serverCheckBox.Checked = true;
        debugCheckBox.CheckedChanged += debugCheckBox_CheckedChanged;
        debugCheckBox.Checked = server.showDebug;
        button_powerOn.Click += button_powerOn_Click; // Включить питание пути.
        button_powerOff.Click += button_powerOff_Click; // Выключить питание пути.

        buttonTP1_T_On.Click += buttonTP1_T_On_Click;
        buttonTP1_T_Off.Click += buttonTP1_T_Off_Click;
        buttonTP1_C_On.Click += buttonTP1_C_On_Click;
        buttonTP1_C_Off.Click += buttonTP1_C_Off_Click;
        buttonTP2_4_T_On.Click += buttonTP2_4_T_On_Click;
        buttonTP2_4_T_Off.Click += buttonTP2_4_T_Off_Click;
        buttonTP2_4_C_On.Click += buttonTP2_4_C_On_Click;
        buttonTP2_4_C_Off.Click += buttonTP2_4_C_Off_Click;
        buttonTP3_T_On.Click += buttonTP3_T_On_Click;
        buttonTP3_T_Off.Click += buttonTP3_T_Off_Click;
        buttonTP3_C_On.Click += buttonTP3_C_On_Click;
        buttonTP3_C_Off.Click += buttonTP3_C_Off_Click;
        buttonTP5_7_T_On.Click += buttonTP5_7_T_On_Click;
        buttonTP5_7_T_Off.Click += buttonTP5_7_T_Off_Click;
        buttonTP5_7_C_On.Click += buttonTP5_7_C_On_Click;
        buttonTP5_7_C_Off.Click += buttonTP5_7_C_Off_Click;
        buttonTP6_8_T_On.Click += buttonTP6_8_T_On_Click;
        buttonTP6_8_T_Off.Click += buttonTP6_8_T_Off_Click;
        buttonTP6_8_C_On.Click += buttonTP6_8_C_On_Click;
        buttonTP6_8_C_Off.Click += buttonTP6_8_C_Off_Click;
        buttonTP9_T_On.Click += buttonTP9_T_On_Click;
        buttonTP9_T_Off.Click += buttonTP9_T_Off_Click;
        buttonTP9_C_On.Click += buttonTP9_C_On_Click;
        buttonTP9_C_Off.Click += buttonTP9_C_Off_Click;
        buttonTP10_T_On.Click += buttonTP10_T_On_Click;
        buttonTP10_T_Off.Click += buttonTP10_T_Off_Click;
        buttonTP10_C_On.Click += buttonTP10_C_On_Click;
        buttonTP10_C_Off.Click += buttonTP10_C_Off_Click;
        buttonTP12_T_On.Click += buttonTP12_T_On_Click;
        buttonTP12_T_Off.Click += buttonTP12_T_Off_Click;
        buttonTP12_C_On.Click += buttonTP12_C_On_Click;
        buttonTP12_C_Off.Click += buttonTP12_C_Off_Click;

        textBox_cmd.KeyDown += textBox_cmd_KeyDown; // Отправка команды по Enter.

        FormClosing += mainFormClosingAsync; // Остановка native server перед закрытием формы.


    }


    private void helpButton_Click(object? sender, EventArgs e)
    {
        PrintAvailableCommands();
    }

    private void PrintAvailableCommands()
    {
        string[] str = new string[] {
            "Доступные команды:",
            "  help - показать список команд",
            "  power on/off - включить/выключить питание пути",
            "  sw <addr> on/off - включить/выключить переключатель (Toggle Switch)",
            "  turn <name> on/off - установить состояние комплексной стрелки",
            "  turnIndC <name> on/off - установить индикатор C одиночной стрелки",
            "  turnIndT <name> on/off - установить индикатор T одиночной стрелки",
            "  turnFBC <name> on/off - установить обратную связь C комплексной стрелки",
            "  turnFBLost <name> on/off - установить ошибку обратной связи комплексной стрелки",
            "  turnFBT <name> on/off - установить обратную связь T комплексной стрелки",
            "  turnButtonC <name> on/off - установить кнопку C комплексной стрелки",
            "  turnButtonAuto <name> on/off - установить кнопку Auto комплексной стрелки",
            "  turnButtonT <name> on/off - установить кнопку T комплексной стрелки",
            
            "  tl <name> <state> - установить состояние светофора",
            "  tlInd <name> on/off - установить состояние индикатора светофора",
            "  route <name> on/off - установить состояние маршрута",
            "  routeBuild <name> - построить маршрут",
            "  routeClear <name> - очистить маршрут",
            "  bus <addr> <input> on/off - установить вход R-Bus",
            "  can <addr> <input> on/off - установить вход CAN",
            "  access <addr> <state> - установить состояние внешнего аксессуара",
            "  sect <name> on/off - установить состояние секции",
            "  printBridgeSections - показать мосты секций",
            "  printBridgeRoutes - показать мосты маршрутов",
            "  infoTurnSolo - показать состояния одиночных стрелок",
            "  infoTurnPair - показать состояния комплексных стрелок",
            "  infoSwitch - показать состояния переключателей",
        };
        foreach (var line in str)
        {
            print(line);
        }
    }

    private void clearButton_Click(object? sender, EventArgs e)
    {
        textBox_log.Clear();
    }

    private async void sendButton_Click(object? sender, EventArgs e)
    {
        await SendCommandFromUiAsync();
    }

    private async void serverCheckBox_CheckedChanged(object? sender, EventArgs e)
    {
        if (_updatingServerCheckBox)
        {
            return;
        }

        if (serverCheckBox.Checked)
        {
            sta.server.start();
        }
        else
        {
            await sta.server.stop();
        }
    }

    private void debugCheckBox_CheckedChanged(object? sender, EventArgs e)
    {
        server.showDebug = debugCheckBox.Checked;
    }

    private async void textBox_cmd_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter)
        {
            return;
        }

        e.SuppressKeyPress = true; // Не добавляем перенос строки в поле команды.
        await SendCommandFromUiAsync(); // Отправляем команду.
    }

    private async void mainFormClosingAsync(object? sender, FormClosingEventArgs e)
    {
        // При закрытии формы сначала останавливаем запущенный native server.
        if (_closing)
        {
            return;
        }

        _closing = true;
        e.Cancel = true;
        await sta.server.stop();
        Close();
    }

    // Кнопка включения питания пути.
    private async void button_powerOn_Click(object? sender, EventArgs e)
    {
        await sta.server.sendPowerStatus(true);
    }

    // Кнопка выключения питания пути.
    private async void button_powerOff_Click(object? sender, EventArgs e)
    {
        await sta.server.sendPowerStatus(false);
    }

    private void button_powerOn_Click_1(object sender, EventArgs e)
    {
    }

    private void SetZ21ServerStatus(bool running)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action(delegate
            {
                SetZ21ServerStatus(running);
            }));
            return;
        }

        _updatingServerCheckBox = true;
        serverCheckBox.Checked = running;
        _updatingServerCheckBox = false;
    }

}
