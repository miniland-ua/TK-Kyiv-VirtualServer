// Routes
public class Route {
    public string name; // Имя маршрута
    public Switch control; // Переключатель маршрута
    public List<(TurnPair, bool)> turn = new(); // Список стрелок маршрута
    public Bridge bridgeLeft; // Список мостов маршрута
    public Bridge bridgeRight; // Список мостов маршрута
    public Section? section; // Секция маршрута

    public Direct direct; // Направление маршрута
    public TypeRoute type; // Тип маршрута

    // События отправки команд для сервера
    public event Action<Switch>? actionSendSwitch; 

    // Конструктор
    public Route(string name, Switch control, List<(TurnPair, bool)> turn, Bridge bridgeLeft, Bridge bridgeRight) {
        this.name = name;
        this.control = control;
        this.turn = turn;
        this.bridgeLeft = bridgeLeft;
        this.bridgeRight = bridgeRight;
        // Подписка для отправки команд на сервер
        this.control.actionSend += (sw) => actionSendSwitch?.Invoke(this.control);
    }

    // Состояния маршрута
    public void setState(bool state) {
        control.setState(state);
    }
    public bool getState() => control.getState();
    public void sendState() => actionSendSwitch?.Invoke(this.control);

    // Постройка маршрута
    public void build() {
        setState(true);
        sendState();
        // Назначение состояния стрелок
        foreach (var (turnPair, state) in turn) {
            turnPair.setState(state);
        }
    }

    // Очистка маршрута
    public void clear() {
        setState(false);
        sendState();
    }
}

// // Rule
// public class Rule {
//     public List<Route> routes = new(); // Список маршрутов правила
// }
