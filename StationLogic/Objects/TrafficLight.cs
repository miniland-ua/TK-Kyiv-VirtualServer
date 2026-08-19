// Светофоры
public class TrafficLight {
    public string name; // Имя светофора
    public int baseAddr; // Базовый адрес сигналов светофора
    public Contact ind; // Индикатор светофора

    // Тип светофора
    public enum Type {
        PreInPAB, // Передвхідний ПАБ
        OutPAB, // Вихідний ПАБ
        PassAB, // Прохідний АБ
        PreInAB, // Передвхідний АБ
        OutAB, // Вихідний АБ
        In, // Вхідний
        InAdd, // Вхідний додатковий
        Shunting, // Маневровий
    }
    public Type type;

    public Direct dir; // Направление светофора
    public Bridge? Bridge; // Мост

    // Сигналы светофора
    private List<Signal> signals = new List<Signal>();
    private Signal? curSignal; // Текущий сигнал светофора

    // Связь с предыдущим светофором
    public TrafficLight? prevTL;

    // События отправки команд для сервера
    public event Action<Switch>? actionSendSwitch;
    public event Action<Contact>? actionSendContact;
    // Событие обновления состояния светофора
    public event Action<TrafficLight>? actionUpdateTL;
    
    // Конструктор светофора
    public TrafficLight(string name, int baseAddr, Type type, Contact ind, Direct dir,
                        List<Switch>? switchList = null) {
        this.name = name;
        this.baseAddr = baseAddr;
        this.type = type;
        this.ind = ind;
        this.dir = dir;

        switch (type) {
            // Передвхідний ПАБ
            case Type.PreInPAB:
                addSignal(baseAddr, Signal.Color.G, switchList);
                addSignal(baseAddr + 1, Signal.Color.Y, switchList);
                addSignal(baseAddr + 2, Signal.Color.Y1Blink, switchList);
                break;
            // Вихідний ПАБ
            case Type.OutPAB:
                addSignal(baseAddr, Signal.Color.R, switchList);
                addSignal(baseAddr + 1, Signal.Color.G, switchList);
                addSignal(baseAddr + 2, Signal.Color.W, switchList);
                break;
            // Прохідний АБ
            case Type.PassAB:
                addSignal(baseAddr, Signal.Color.R, switchList);
                addSignal(baseAddr + 1, Signal.Color.G, switchList);
                addSignal(baseAddr + 2, Signal.Color.Y, switchList);
                break;
            // Передвхідний АБ
            case Type.PreInAB:
                addSignal(baseAddr, Signal.Color.R, switchList);
                addSignal(baseAddr + 1, Signal.Color.G, switchList);
                addSignal(baseAddr + 2, Signal.Color.Y, switchList);
                addSignal(baseAddr + 3, Signal.Color.Y1Blink, switchList);
                break;
            // Вихідний АБ
            case Type.OutAB:
                addSignal(baseAddr, Signal.Color.R, switchList);
                addSignal(baseAddr + 1, Signal.Color.G, switchList);
                addSignal(baseAddr + 2, Signal.Color.Y, switchList);
                addSignal(baseAddr + 3, Signal.Color.Y1Blink, switchList);
                addSignal(baseAddr + 4, Signal.Color.W, switchList);
                break;
            // Вхідний
            case Type.In:
                addSignal(baseAddr, Signal.Color.R, switchList);
                addSignal(baseAddr + 1, Signal.Color.G, switchList);
                addSignal(baseAddr + 2, Signal.Color.Y, switchList);
                addSignal(baseAddr + 3, Signal.Color.Y1Blink, switchList);
                addSignal(baseAddr + 4, Signal.Color.Y1_Y2, switchList);
                addSignal(baseAddr + 5, Signal.Color.Y1Blink_Y2, switchList);
                addSignal(baseAddr + 6, Signal.Color.WBlink, switchList);
                break;
            // Вхідний додатковий
            case Type.InAdd:
                addSignal(baseAddr, Signal.Color.R, switchList);
                addSignal(baseAddr + 1, Signal.Color.Y1_Y2, switchList);
                break;
            // Маневровий
            case Type.Shunting:
                addSignal(baseAddr, Signal.Color.R, switchList);
                addSignal(baseAddr + 1, Signal.Color.W, switchList);
                break;
            default:
                break;
        }
        // Подписки для отправки команд на сервер
        foreach (Signal signal in signals) {
            signal.actionSend += (sw) => actionSendSwitch?.Invoke(sw);
        }

        // Инициализация текущего сигнала светофора
        curSignal = signals.FirstOrDefault();
    }

    // Создание сигнала + добавление его свитча в общий список
    private void addSignal(int addr, Signal.Color color, List<Switch>? switchList) {
        Switch sw = new(Switch.Type.Virt_Toggle, addr);
        signals.Add(new Signal(sw, color));
        switchList?.Add(sw);
    }

    // Инициализация светофора
    public void init() {
        for (int i = signals.Count - 1; i >= 0; i--) {
            setState(i);
            sendState();
        }
    }

    // Состояния светофора
    public void setState(int state) {
        // Если состояние светофора за пределами - выходим
        if (signals.Count <= state || state < 0) {
            return;
        }
        Signal? newSignal = signals[state];
        // Если состояние светофора не изменилось - выходим
        if (curSignal == newSignal) {
            return;
        }
        // Отключение текущего сигнала светофора
        curSignal?.setState(false);
        // Включение нового сигнала светофора
        curSignal = newSignal;
        curSignal?.setState(true);

        if (prevTL != null) {
            prevTL.actionUpdateTL?.Invoke(this);
        }

    }
    public void setState(Signal.Color color) {
        int state = signals.FindIndex(signal => signal.getColor() == color);
        setState(state);
    }
    public bool getState() => ind.getState();
    public void sendState() => curSignal?.sendState();

    // Состояние индикатора
    public void setIndState(bool state) => ind.setState(state);
    public bool getIndState() => ind.getState();
    public void sendIndState() => actionSendContact?.Invoke(ind);

    // Сигнал светофора
    public class Signal {
        private Switch control; // Переключатель светофора

        // Тип сигнала
        public enum Type {
            red, // Красный
            train, // Поездной
            shunt, // Маневровый
        }
        Type type;

        // Цвет сигнала
        public enum Color {
            R, // Красный
            G, // Зеленый
            Y, // Желтый
            Y1Blink,// Желтый (вверхний) мигающий
            Y1_Y2, // 2 желтых
            Y1Blink_Y2, // 2 желтых (верхний мигающий + нижний)
            Y1Blink_W, // Желтый мигающий + белый
            W, // Белый
            WBlink // Белый мигающий
        }
        Color color;

        // Событие изменения состояния сигнала светофора
        public event Action<Switch>? actionSend;

        // Конструктор сигнала светофора
        public Signal(Switch control, Color color) {
            this.control = control;
            this.color = color;
        }

        // Команды состояния
        public void setState(bool state) => control.setState(state);
        public bool getState() => control.getState();
        public void sendState() => actionSend?.Invoke(control);

        public Color getColor() => color;
    }
}
