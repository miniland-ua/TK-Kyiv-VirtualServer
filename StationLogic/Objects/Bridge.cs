// Мост между секциями
public class Bridge {
    public string name = ""; // Имя
    public TrafficLight? trafficLight; // Светофор
    public List<Section> sectionList = new(); // Список секций
    public List<Route> routeList = new(); // Список маршрутов

    // Конструктор
    public Bridge(string name, TrafficLight? trafficLight = null) {
        this.name = name;
        this.trafficLight = trafficLight;
    }

    // Добавление секции и маршрута
    public void addSection(Section section) {
        sectionList.Add(section);
    }
    public void addRoute(Route route) {
        routeList.Add(route);
    }
}