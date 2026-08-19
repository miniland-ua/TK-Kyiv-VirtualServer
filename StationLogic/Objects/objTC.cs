// Индикатор занятости в TC
public class Contact {
    public enum Type {
        RBus, // R-Bus
        CANBus, // CAN-Bus
        Disp // Диспетчерский
    }
    public Type type;
    public int addr; // Адресс изменения состояния
    public int input; // Номер входа
    private bool state = false; // Состояние индикатора занятости

    // Событие изменения состояния индикатора занятости
    public event Action<Contact>? actionSend;

    // Конструктор
    public Contact(Type type, int addr, int input) {
        this.type = type;
        this.addr = addr;
        this.input = input;
    }

    // Команды для состояния
    public void setState(bool state) => this.state = state;
    public bool getState() => this.state;
    public void sendState() => actionSend?.Invoke(this);
}

// Toggle switch (переключатель)
public class Switch {
    public enum Type {
        Virt_Toggle, // Виртуальный Toggle Switch
        Virt_Push, // Виртуальный Push Button
        Real // Реальный
    }

    public int addr; // Адресс изменения состояния переключателя
    public Type type; // Тип переключателя
    private bool state = false; // Состояние переключателя

    // Событие изменения состояния переключателя
    public event Action<Switch>? actionSend;

    // Конструктор
    public Switch(Type type, int addr) {
        this.type = type;
        this.addr = addr;
    }

    // Команды для состояния
    public void setState(bool state) => this.state = state;
    public bool getState() => this.state;
    public void sendState() => actionSend?.Invoke(this);
}

// Комплексный переключатель
public class SwitchControl {
    public int baseAddr; // Базовый адресс переключателя
    public List<Switch> switches = new(); // Список переключателей

    // Событие изменения состояния переключателя
    public event Action<SwitchControl>? actionSend;

    // Конструктор
    public SwitchControl(int baseAddr, int size) {
        this.baseAddr = baseAddr;
        for (int i = 0; i < size; i++) {
            switches.Add(new Switch(Switch.Type.Virt_Toggle, baseAddr + i));
            // Подписка для отправки команд на сервер
            switches[i].actionSend += (sw) => actionSend?.Invoke(this);
        }
    }

    // Команды для состояния
    public void setState(int state) {
        for (int i = 0; i < switches.Count; i++) {
            if (i == state) {
                switches[i].setState(true);
            } else {
                switches[i].setState(false);
            }
        }
    }
    public int getState() {
        for (int i = 0; i < switches.Count; i++) {
            if (switches[i].getState()) {
                return i;
            }
        }
        return -1; // Если ни один переключатель не активен
    }
    public void sendState() => switches.FirstOrDefault(sw => sw.getState())?.sendState();
}
