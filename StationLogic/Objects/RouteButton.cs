
// Кнопка для управления маршрутом
public class RouteButton {
    public string name; // Имя
    public Switch control; // Переключатель
    public Bridge bridge; // Мост
    // Тип кнопки
    public enum Type {
        Train, // Поездной
        Shunt, // Маневровый
        End // Свободный
    }
    public Type type;

    // События отправки команд для сервера
    public event Action<Switch>? actionSend;

    // Конструктор
    public RouteButton(string name, Type type, Switch control, Bridge bridge) {
        this.name = name;
        this.type = type;
        this.control = control;
        this.bridge = bridge;
        // Подписка для отправки команд на сервер
        this.control.actionSend += (sw) => actionSend?.Invoke(this.control);
    }

    // Команды состояния
    public void setState(bool state) => control?.setState(state);
    public bool getState() => control?.getState() ?? false;
    public void sendState() => actionSend?.Invoke(control);
}