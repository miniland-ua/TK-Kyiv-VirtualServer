using static ConsoleLog;

// Правило создания маршрута (набор маршрутов + светофоры)
public class Rule {
    public List<Route> routes = new(); // Список маршрутов правила
    public Bridge? startBridge; // Стартовый мост правила
    public Bridge? finishBridge; // Конечный мост правила
    public TrafficLight? tl; // Стартовый светофор
    public TypeRoute type; // Тип правила
    public Direct direct; // Направление правила
    public bool isCorrectWay; // Правильный путь (для поездного маршрута)

    public Rule(List<Route> routes, TypeRoute type, Direct direct, bool isCorrectWay = true) {
        if (direct == Direct.Left) {
            this.startBridge = routes.FirstOrDefault()?.bridgeRight;
            this.finishBridge = routes.LastOrDefault()?.bridgeLeft;
        } else {
            this.startBridge = routes.FirstOrDefault()?.bridgeLeft;
            this.finishBridge = routes.LastOrDefault()?.bridgeRight;
        }
        this.routes = routes;
        this.type = type;
        this.direct = direct;
        this.isCorrectWay = isCorrectWay;
        // Находим стартовый светофор правила
        tl = startBridge?.trafficLight;
    }


    // Постройка маршрута
    public void build() {
        // Если нету стартового светофора - выходим
        if (tl == null) {
            return;
        }

        // Постройка всех маршрутов
        foreach (Route route in routes) {
            route.build();
        }

        print($"Стартовый светофор правила: {tl.name}, тип: {tl.type}, направление: {tl.dir}");
        
        // Определение правильности пути перегона (для поездного маршрута)
        bool isCorrectWayPeregon = true;
        // Определение что следующий светофор красный или отключенный (для поездного маршрута)
        TrafficLight? nextTL = finishBridge?.trafficLight;
        bool isNextTLRedOrOff = nextTL?.getColor() == TrafficLight.Signal.Color.R 
                            || nextTL?.getColor() == TrafficLight.Signal.Color.Off;
        bool isNextTLRedOrW = nextTL?.getColor() == TrafficLight.Signal.Color.R 
                            || nextTL?.getColor() == TrafficLight.Signal.Color.W;


        // Включение светофоров
        // Маневровый маршрут
        if (type == TypeRoute.Shunt) {
            tl.setState(TrafficLight.Signal.Color.W);
            print($"Светофор {tl.name} установлен в белый", Color.HotPink);
        // Поездной маршрут
        } else {
            switch (tl.type) {
            // Вхідний додатковий
            case TrafficLight.Type.InAdd:
                tl.setState(TrafficLight.Signal.Color.Y1_Y2);
                print($"Светофор {tl.name} установлен в 2 желтых", Color.HotPink);
                break;

            // Вихідний ПАБ
            case TrafficLight.Type.OutPAB:
                // Зеленый - для поездного маршрута + правильный путь перегона
                if (type == TypeRoute.Train && isCorrectWayPeregon) {
                    tl.setState(TrafficLight.Signal.Color.G);
                    print($"Светофор {tl.name} установлен в зеленый", Color.HotPink);
                // Белый - для маневрового или поездного маршрута по неправильному пути
                } else {
                    tl.setState(TrafficLight.Signal.Color.W);
                    print($"Светофор {tl.name} установлен в белый", Color.HotPink);
                }
                break;
                
            // Вихідний АБ
            case TrafficLight.Type.OutAB:
                if (type == TypeRoute.Shunt) {
                    tl.setState(TrafficLight.Signal.Color.W);
                } else {
                    // Зеленый - неправильный путь перегона + след. светофор зеленый/желтый
                    if (!isCorrectWayPeregon && !isNextTLRedOrOff) {
                        tl.setState(TrafficLight.Signal.Color.G);
                        print($"Светофор {tl.name} установлен в зеленый", Color.HotPink);
                    // Желтый - неправильный путь перегона + след. светофор красный/отключен
                    } else if (!isCorrectWayPeregon && isNextTLRedOrOff) {
                        tl.setState(TrafficLight.Signal.Color.Y);
                        print($"Светофор {tl.name} установлен в желтый", Color.HotPink);
                    // Желт.миг.+белый - правильный путь перегона
                    } else if (isCorrectWayPeregon) {
                        tl.setState(TrafficLight.Signal.Color.Y1Blink_W);
                        print($"Светофор {tl.name} установлен в желт.миг.+белый", Color.HotPink);
                    // Белый 
                    } else {
                        tl.setState(TrafficLight.Signal.Color.W);
                        print($"Светофор {tl.name} установлен в белый", Color.HotPink);
                    }

                }
                break;

            // Вхідний
            case TrafficLight.Type.In:
                // Желтый - правильный путь правила + впереди светофор красный/белый
                if (isCorrectWay && isNextTLRedOrW) {
                    tl.setState(TrafficLight.Signal.Color.Y);
                    print($"Светофор {tl.name} установлен в желтый", Color.HotPink);
                // Зеленый - правильный путь правила + впереди светофор НЕ красный/белый
                } else if (isCorrectWay && !isNextTLRedOrW) {
                    tl.setState(TrafficLight.Signal.Color.G);
                    print($"Светофор {tl.name} установлен в зеленый", Color.HotPink);
                // 2 Желтых - неправильный путь правила + впереди светофор красный/белый
                } else if (!isCorrectWay && isNextTLRedOrW) {
                    tl.setState(TrafficLight.Signal.Color.Y1_Y2);
                    print($"Светофор {tl.name} установлен в 2 желтых", Color.HotPink);
                // Желтый + желтый мигающий - неправильный путь правила + впереди светофор НЕ красный/белый
                } else if (!isCorrectWay && !isNextTLRedOrW) {
                    tl.setState(TrafficLight.Signal.Color.Y1Blink_Y2);
                    print($"Светофор {tl.name} установлен в желтый + желтый мигающий", Color.HotPink);
                }

                break;
            }
        }
        tl.sendState();
    }

    // Очистка правила
    public void clear() {
        // Очистка всех маршрутов
        foreach (Route route in routes) {
            route.clear();
        }
        // Очистка светофора
        tl?.clear();
    }
}