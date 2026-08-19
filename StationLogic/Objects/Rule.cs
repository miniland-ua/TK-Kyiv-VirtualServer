// Правило создания маршрута (набор маршрутов + светофоры)
public class Rule {
    public List<Route> routes = new(); // Список маршрутов правила
    public Bridge? startBridge; // Стартовый мост правила
    public Bridge? finishBridge; // Конечный мост правила
    public List<TrafficLight> tlList = new(); // Список светофоров правила
    public TypeRoute type; // Тип правила
    Direct direct; // Направление правила

    public Rule(List<Route> routes, TypeRoute type, Direct direct) {
        if (direct == Direct.Left) {
            this.startBridge = routes.FirstOrDefault()?.bridgeLeft;
            this.finishBridge = routes.LastOrDefault()?.bridgeLeft;
        } else {
            this.startBridge = routes.FirstOrDefault()?.bridgeRight;
            this.finishBridge = routes.LastOrDefault()?.bridgeRight;
        }
        this.routes = routes;
        this.type = type;
        this.direct = direct;
        // Список светофоров в нужно направлении
        for (int i = 0; i < routes.Count - 1; i++) {
            Route route = routes[i];
            TrafficLight? tlLeft = route.bridgeLeft?.trafficLight;
            // Фильтруем маневровые светофоры для поездных маршрутов
            if (tlLeft?.dir == direct
            && (type == TypeRoute.Shunt || tlLeft.type != TrafficLight.Type.Shunting)) {
                tlList.Add(tlLeft);
            }
            TrafficLight? tlRight = route.bridgeRight?.trafficLight;
            // Фильтруем маневровые светофоры для поездных маршрутов
            if (tlRight?.dir == direct
            && (type == TypeRoute.Shunt || tlRight.type != TrafficLight.Type.Shunting)) {
                tlList.Add(tlRight);
            }

        }

    }

    // Постройка маршрута
    public void build() {
        // Постройка всех маршрутов
        foreach (Route route in routes) {
            route.build();
        }
        // Включение светофоров
        foreach (TrafficLight tl in tlList) {
            switch (tl.type) {
            // Передвхідний ПАБ
            case TrafficLight.Type.PreInPAB:
                // -
                break;
            // Вихідний ПАБ
            case TrafficLight.Type.OutPAB:
                if (type == TypeRoute.Shunt) {
                    // Доп условие (для поездного маршрута): 
                    // Если едет на перегон по неправильному пути
                    tl.setState(TrafficLight.Signal.Color.W);
                } else {
                    tl.setState(TrafficLight.Signal.Color.G);
                }
                break;
            // Прохідний АБ
            case TrafficLight.Type.PassAB:
                // -
                break;
            // Передвхідний АБ
            case TrafficLight.Type.PreInAB:
                // -
                break;
            // Вихідний АБ
            case TrafficLight.Type.OutAB:
                if (type == TypeRoute.Shunt) {
                    tl.setState(TrafficLight.Signal.Color.W);
                } else {
                    // Когда з / ж или б(ж)???????????????????

                    // Зеленый:
                    // Отправка по правильному пути: Если след. светофор(переген) зелен/желт

                    // Желтый:
                    // Отправка по правильному пути: Если след. светофор(перегон) красн

                    // Белый(Желт):
                    // Отправка по неправильному пути
                }

                break;
            // Вхідний
            case TrafficLight.Type.In:
                // Определяем есть ли впереди активная стрелка (до следущего светофора)
                bool isActiveTurn = false;
                foreach (Route route in routes) {
                    foreach ((TurnPair turnPair, bool state) in route.turn) {
                        if (state == true) {
                            isActiveTurn = true;
                        }
                    }
                }

                // Желтый:
                // Когда впереди все стрелки выставлены в прямом положении
                // Если впереди светофор: красный / белый 

                // Зеленый:
                // Когда впереди все стрелки выставлены в прямом положении
                // Если впереди светофор НЕ: красный / белый (ПОЕЗДНОЙ)


                // 2 Желтых:
                // Когда впередели есть хотябы одна стрелка боковом положении
                // Если впереди светофор: красный / белый 

                // Желтый + желтый мигающий:
                // Когда впередели есть хотябы одна стрелка боковом положении
                // Если впереди светофор НЕ: красный / белый (ПОЕЗДНОЙ)

                if (!isActiveTurn) {
                    tl.setState(TrafficLight.Signal.Color.G);
                } else {
                    tl.setState(TrafficLight.Signal.Color.Y1_Y2);
                }

                break;
            // Вхідний додатковий
            case TrafficLight.Type.InAdd:
                tl.setState(TrafficLight.Signal.Color.Y1_Y2);
                break;
            // Маневровий
            case TrafficLight.Type.Shunting:
                tl.setState(TrafficLight.Signal.Color.W);
                break;
            }
            tl.sendState();
        }
    }
}