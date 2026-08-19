// Разъединитель секций
public class Separate {
    public string name; // Имя разъединителя
    private Switch control; // Переключатель разъединителя
    public Contact ind; // Индикатор разъединителя
    public Section section; // Секция разъединителя

    // События отправки команд для сервера
    public event Action<Contact>? actionSendContact;
    public event Action<Switch>? actionSendSwitch;

    // Конструктор
    public Separate(string name, Switch control, Contact ind, Section section) {
        this.name = name;
        this.control = control;
        this.ind = ind;
        this.section = section;
        // Подписки для отправки команд на сервер
        this.control.actionSend += (sw) => actionSendSwitch?.Invoke(this.control);
        this.ind.actionSend += (contact) => actionSendContact?.Invoke(this.ind);
    }

    // Состояния разъединителя
    public void setState(bool state) => control.setState(state);
    public bool getState() => control.getState();
    public void sendState() => actionSendSwitch?.Invoke(control);

    // Состояние индикатора
    public void setIndState(bool state) => ind.setState(state);
    public bool getIndState() => ind.getState();
    public void sendIndState() => actionSendContact?.Invoke(ind);
}

// Панель разъединителя
public class SeparateControl {
    public List<Separate> separateList = new(); // Список разъединителей
    public Switch indProcess = new Switch(Switch.Type.Virt_Toggle, 1416); // Индикатор процесса
    public Switch buttonStart = new Switch(Switch.Type.Virt_Push, 1417); // Кнопка запуска процесса

    // События отправки команд для сервера
    public event Action<Contact>? actionSendContact;
    public event Action<Switch>? actionSendSwitch;

    // Конструктор
    public SeparateControl(List<Separate>? separateList = null) {
        if (separateList != null) {
            this.separateList = separateList;
        }

        // Подписки для отправки команд на сервер
        this.indProcess.actionSend += (sw) => actionSendSwitch?.Invoke(sw);
        this.buttonStart.actionSend += (sw) => actionSendSwitch?.Invoke(sw);
        foreach (Separate separate in this.separateList) {
            separate.actionSendSwitch += (sw) => actionSendSwitch?.Invoke(sw);
            separate.actionSendContact += (contact) => actionSendContact?.Invoke(contact);
        }
    }
}
