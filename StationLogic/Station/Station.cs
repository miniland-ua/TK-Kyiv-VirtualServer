using System.Net.Sockets;
using static ConsoleLog;

public partial class Station {
    UdpClient socket = new(AddressFamily.InterNetwork);
    public Z21Server server; // Объект сервера Z21
    Dictionary<ushort, byte> sw = new(); // Состояние стрелок: адрес + позиция

    public List<Switch> switchList = new(); // Список всех свитчей
    public List<Contact> contactList = new(); // Список всех контактов

    public List<Section> sectionList = new(); // Список всех секций
    public List<TurnSolo> turnSoloList = new(); // Список всех одиночных стрелок
    public List<TurnPair> turnPairList = new(); // Список всех комплексных стрелок
    public List<TrafficLight> tlList = new(); // Список всех светофоров
    public List<Route> routeList = new(); // Список всех маршрутов
    public List<Bridge> bridgeList = new(); // Список всех мостов
    public List<Separate> separateList = new(); // Список всех разъединителей
    public List<RouteButton> routeButtonList = new(); // Список всех кнопок управления маршрутами
    
    public SeparateControl separateControl; // Панель управления разъединителями
    public RouteControl routeControl; // Панель управления маршрутами
    
    public Switch buttonEnaFB; // Кнопка разрешения обратной связи
    public Switch buttonInitTurn; // Кнопка инициализации стрелок
    public Switch buttonInitStation; // Кнопка инициализации системы
    public Switch buttonTurnControl; // Кнопка контроля стрелок

    public List<Rule> ruleList = new(); // Список всех правил

    // Конструктор
    public Station(Z21Server s) {
        this.server = s; // Создаем объект сервера Z21
        server.switchRead += readSwitches; // Подписываемся на событие чтения состояния стрелок
        server.switchRequest += readSwitchesRequest; // Подписываемся на событие запроса состояния свитча
        server.contactRequest += connectedTC; // Подписываемся на событие запроса состояния контакта

        // Загрузка базы данных
        initLoader("sta2");

        // Создание системных кнопок
        buttonEnaFB = createSwitch(Switch.Type.Virt_Toggle, 1448); // Кнопка запрета обратной связи
        buttonEnaFB.setState(true); // Включаем обратную связь по умолчанию
        buttonInitTurn = createSwitch(Switch.Type.Virt_Toggle, 1449); // Кнопка инициализации стрелок
        buttonInitStation = createSwitch(Switch.Type.Virt_Toggle, 1450); // Кнопка инициализации системы
        buttonTurnControl = createSwitch(Switch.Type.Virt_Push, 1400); // Кнопка контроля стрелок
        // Создание панелей управления
        separateControl = new SeparateControl(separateList);
        routeControl = new RouteControl(routeButtonList);
        
        // Подписки одиночных стрелок
        foreach (TurnSolo turn in turnSoloList) {
            turn.actionSendContact += sendContact;
        }
        // Подписки парных стрелок
        foreach (TurnPair turnPair in turnPairList) {
            turnPair.actionSendContact += sendContact;
            turnPair.actionSendSwitch += sendSwitch;
        }
        // Подписки светофоров
        foreach (TrafficLight tl in tlList) {
            tl.actionSendSwitch += sendSwitch;
            tl.actionSendContact += sendContact;
            tl.actionUpdateTL += updateTL;
        }
        // Подписки маршрутов
        foreach (Route route in routeList) {
            route.actionSendSwitch += sendSwitch;
        }
        // Подписки секций
        foreach (Section section in sectionList) {
            section.actionSendContact += sendContact;
            section.actionSendSwitch += sendSwitch;
        }
        // Подписки разъединителей
        separateControl.actionSendContact += sendContact;
        separateControl.actionSendSwitch += sendSwitch;
        // Подписка панели управления маршрутами
        routeControl.actionSendSwitch += sendSwitch;

        // Добавление связи между секциями и мостами
        foreach (Section section in sectionList) {
            foreach (Route? route in section.route) {
                if (route == null) {
                    continue;
                }

                Bridge leftBridge = route.bridgeLeft;
                Bridge rightBridge = route.bridgeRight;

                // Добавление мостов в секцию
                if (!section.bridge.Contains(leftBridge)) {
                    section.bridge.Add(leftBridge);
                }

                if (!section.bridge.Contains(rightBridge)) {
                    section.bridge.Add(rightBridge);
                }

                // Секция находится справа от левого моста
                if (!leftBridge.sectionList.Contains(section)) {
                    leftBridge.sectionList.Add(section);
                }

                // Секция находится слева от правого моста
                if (!rightBridge.sectionList.Contains(section)) {
                    rightBridge.sectionList.Insert(0, section);
                }
            }
        }
        // Добавление маршрута к мостам и секциям
        foreach (Route route in routeList) {
            // Добавление маршрута к мостам
            if (!route.bridgeLeft.routeList.Contains(route)) {
                route.bridgeLeft.routeList.Add(route);
            }
            if (!route.bridgeRight.routeList.Contains(route)) {
                route.bridgeRight.routeList.Add(route);
            }
            // Добавление маршрута к секциям
            foreach (Section section in sectionList){
                if (section.route.Contains(route)) {
                    route.section = section;
                    break;
                }
            }
        }
        // Добавление парных стрелок к секциям
        foreach (Section section in sectionList) {
            foreach (Route? route in section.route) {
                if (route == null) continue;

                foreach ((TurnPair turnPair, _) in route.turn) {
                    if (!section.turnPair.Contains(turnPair)) {
                        section.turnPair.Add(turnPair);
                    }
                }
            }
        }
        // Добавление мостов к светофорам
        foreach (TrafficLight tl in tlList) {
            foreach (Bridge bridge in bridgeList) {
                if (bridge.name == tl.Bridge?.name) {
                    tl.Bridge = bridge;
                    break;
                }
            }
        }
    }

    // Поиск одиночной стрелки
    public TurnSolo? atTurnSolo(string name) =>
        turnSoloList.FirstOrDefault(x =>
            string.Equals(x.name, name, StringComparison.Ordinal));
    // Поиск комплексной стрелки
    public TurnPair? atTurnPair(string name) =>
        turnPairList.FirstOrDefault(x =>
            string.Equals(x.name, name, StringComparison.Ordinal));
    // Поиск светофора
    public TrafficLight? atTL(string name) =>
        tlList.FirstOrDefault(x =>
            string.Equals(x.name, name, StringComparison.Ordinal));
    // Поиск моста
    public Bridge? atBridge(string name) =>
        bridgeList.FirstOrDefault(x =>
            string.Equals(x.name, name, StringComparison.Ordinal));
    // Поиск секции
    public Section? atSect(string name) =>
        sectionList.FirstOrDefault(x =>
            string.Equals(x.name, name, StringComparison.Ordinal));
    // Поиск Route
    public Route? atRoute(string name) =>
        routeList.FirstOrDefault(x =>
            string.Equals(x.name, name, StringComparison.Ordinal));
    // Поиск свитча по адресу
    public Switch? atSwitchPush(int addr) =>
        switchList.FirstOrDefault(x => x.addr == addr 
                                && x.type == Switch.Type.Virt_Push);
    public Switch? atSwitchToggle(int addr) =>
        switchList.FirstOrDefault(x => x.addr == addr 
                                && x.type == Switch.Type.Virt_Toggle);
    public Switch? atSwitchVirt(int addr) =>
        switchList.FirstOrDefault(x => x.addr == addr 
                                && (x.type == Switch.Type.Virt_Toggle 
                                || x.type == Switch.Type.Virt_Push));
    public Switch? atSwitchDisp(int addr) =>
        switchList.FirstOrDefault(x => x.addr == addr && x.type == Switch.Type.Real);
    // Поиск контакта по адресу
    public Contact? atContactVirt(int addr, int input) =>
        contactList.FirstOrDefault(x => x.addr == addr 
                                && x.input == input 
                                && x.type == Contact.Type.CANBus);
    public Contact? atContactDisp(int addr, int input) =>
        contactList.FirstOrDefault(x => x.addr == addr 
                                && x.input == input 
                                && x.type == Contact.Type.Disp);
    // Поиск правила
    public Rule? atRule(TrafficLight tl) {
        foreach (Rule rule in ruleList) {
            foreach (Route route in rule.routes) {
                if (route.bridgeLeft == tl.Bridge || route.bridgeRight == tl.Bridge) {
                    return rule;
                }
            }
        }
        return null;
    }
        

    // Создание свитча
    private Switch createSwitch(Switch.Type type, int addr) {
        Switch sw = new(type, addr);
        switchList.Add(sw); // Добавление свитча в список
        sw.actionSend += sendSwitch; // Подписка на событие отправки состояния свитча
        return sw;
    }

    // Создание контакта
    private Contact createContact(Contact.Type type, int addr, int input) {
        Contact contact = new(type, addr, input);
        contactList.Add(contact); // Добавление контакта в список
        contact.actionSend += sendContact; // Подписка на событие отправки состояния контакта
        return contact;
    }

    // Изменение активности маршрута по имени
    // public async Task setRouteActive(string name, bool state) {
    //     if (atRoute(name) is Route route) { // Если маршрут найден по имени
    //         print("setRouteActive: найден маршрут " + route.name);
    //         route.setState(state); // Изменяем активность маршрута
    //     }
    // }
    // // Изменение занятости маршрута по имени
    // public async Task setBusy(string name, bool state) {
    //     foreach (Route route in routeList) {
    //         if (string.Equals(route.name, name, StringComparison.Ordinal)) {
    //             print("setBusy: найден маршрут " + route.name);
    //             // route.setBusy(state);
    //             return;
    //         }
    //     }
    // }

    // Инициализация стрелок
    public void initTurn() {
        print("Инициализация стрелок");
        // Нажатие кнопки управления C
        foreach (TurnPair turnPair in turnPairList) {
            turnPair.pressButtonControl(turnPair.tc.buttonT, true);
            System.Threading.Thread.Sleep(50);
        }
        // Нажатие кнопки управления T
        foreach (TurnPair turnPair in turnPairList) {
            turnPair.pressButtonControl(turnPair.tc.buttonC, true);
            System.Threading.Thread.Sleep(50);
        }
        // Нажатие кнопки управления Auto
        foreach (TurnPair turnPair in turnPairList) {
            turnPair.pressButtonControl(turnPair.tc.buttonAuto, true);
        }
        print("Инициализация стрелок завершена");

    }

    // Отправка команд по окончанию подключения к TC
    public void connectedTC() {
        print("Запуск TrainController");
        // Включение разрешение обратной связи
        buttonEnaFB.setState(true);
        buttonEnaFB.sendState();
        // Инициализация стрелок
        foreach (TurnPair turnPair in turnPairList) {
            turnPair.init();
        }
        // Очистка всех маршрутов
        clearAllRoutes();
        // Инициализация всех светофоров
        foreach (TrafficLight tl in tlList) {
            tl.init();
        }
        // Отправка всех включеных CAN-контактов
        foreach (Contact ci in contactList) {
            if (ci.type == Contact.Type.CANBus) {
                if (ci.getState()) {
                    sendContact(ci);
                }
            }
        }
        // Инициализация стрелок
        buttonInitTurn.setState(true);
        buttonInitTurn.sendState();
        initTurn();
        buttonInitTurn.setState(false);
        buttonInitTurn.sendState();
        print("Запуск TrainController завершен");
    }

    // Инициализация станции
    public async void initStation() {
        print("Инициализация подключения к TrainController");
        // Включение разрешение обратной связи
        buttonEnaFB.setState(true);
        buttonEnaFB.sendState();
        // Инициализация стрелок
        foreach (TurnPair turnPair in turnPairList) {
            turnPair.init();
        }
        // Очистка всех маршрутов
        clearAllRoutes();
        // Инициализация всех светофоров
        foreach (TrafficLight tl in tlList) {
            tl.init();
        }
        // Отправка всех CAN-контактов
        foreach (Contact ci in contactList) {
            if (ci.type == Contact.Type.CANBus) {
                sendContact(ci);
            }
        }
        // Отправка всех свитчей
        foreach (Switch sw in switchList) {
            sendSwitch(sw);
        }
        print("Инициализация подключения к TrainController завершена");
    }

    // Очистка всех маршрутов
    public void clearAllRoutes() {
        foreach (Route route in routeList) {
            route.clear();
        }
    }

    // Очистка всех включенных светофоров
    public void clearAllTrafficLights() {
        foreach (TrafficLight tl in tlList) {
            tl.setState(0);
            tl.sendState();
        }
    }

}
