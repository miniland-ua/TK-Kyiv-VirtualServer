// Секция
public class Section {
    public string name; // Имя
    private Switch control; // Переключатель секции
    private Contact realOccup; // Реальная занятость секции
    public List<Route?> route = new(); // Список маршрутов
    public List<Bridge?> bridge = new(); // Мосты
    public List<TurnPair?> turnPair = new(); // Список стрелок секции

    // События отправки команд для сервера
    public event Action<Contact>? actionSendContact;
    public event Action<Switch>? actionSendSwitch;

    // Конструктор
    public Section(string name, Switch control, Contact realOccup, List<Route?> route) {
        this.name = name;
        this.control = control;
        this.realOccup = realOccup;
        this.route = route;
        // Подписки для отправки команд на сервер
        this.control.actionSend += (sw) => actionSendSwitch?.Invoke(this.control);
        this.realOccup.actionSend += (contact) => actionSendContact?.Invoke(this.realOccup);
    }

    // Состояния секции
    public void setState(bool state) => control.setState(state);
    public bool getState() => control.getState();
    public void sendState() => actionSendSwitch?.Invoke(control);
}
