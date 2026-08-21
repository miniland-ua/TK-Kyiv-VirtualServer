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
        // Создание панели управления маршрутами
        routeControl = new RouteControl(routeButtonList);
        // Подписки
        routeControl.actionCreateRule += addRule; // Фиксация созданного правила
        routeControl.actionCancelRule += cancelRule; // Отмена правила за мостом
        switchList.Add(routeControl.buttonCancelSet);
        switchList.Add(routeControl.buttonCancelRoute);

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

                // Добавление левого моста маршрута к секции
                Bridge leftBridge = route.bridgeLeft;
                if (!section.bridge.Contains(leftBridge)) {
                    section.bridge.Add(leftBridge);
                }
                // Добавление правого моста маршрута к секции
                Bridge rightBridge = route.bridgeRight;
                if (!section.bridge.Contains(rightBridge)) {
                    section.bridge.Add(rightBridge);
                }

                // Добавление секции к мостам маршрута
                leftBridge.sectionLeft = section;
                rightBridge.sectionRight = section;
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
        // Добавление секций к одиночным стрелкам
        foreach (Section section in sectionList) {
            foreach (TurnSolo? turn in section.turnSolo) {
                if (turn == null) {
                    continue;
                }

                if (!turn.sections.Contains(section)) {
                    turn.sections.Add(section);
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
    public async Task connectedTC() {
        print("Запуск TrainController");
        // Включение кнопки инициализации системы
        buttonInitStation.setState(true);
        buttonInitStation.sendState();

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

        int delay = 0;
        // Подсчет задержки для светофоров
        foreach (TrafficLight tl in tlList) {
            delay += 100 * tl.signals.Count;
        }
        await Task.Delay(delay);

        // Отключение кнопки инициализации системы
        buttonInitStation.setState(false);
        buttonInitStation.sendState();
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
            tl.setState(0);
            tl.sendState();
        }
        // Инициализация панели управления маршрутами
        routeControl.init();
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
        ruleList.Clear();
    }

    // Очистка всех включенных светофоров
    public void clearAllTrafficLights() {
        foreach (TrafficLight tl in tlList) {
            tl.setState(0);
            tl.sendState();
        }
    }

    // Фиксация созданного правила
    public void addRule(Rule rule) {
        print($"Фиксация правила {rule}", Color.OrangeRed);
        ruleList.Add(rule);
    }

    // Отмена правила
    public async Task cancelRule(Bridge bridge, TypeRoute typeRoute) {
        print($"Отмена правила {bridge.name}", Color.OrangeRed);
        foreach (Rule rule in ruleList) {
            if (rule.startBridge == bridge && rule.type == typeRoute) {
                print($"Найдено правило {rule}", Color.OrangeRed);
                // Отключаем кнопку отмены маршрута
                routeControl.setButtonCancelRoute(false);
                routeControl.sendButtonCancelRoute();

                // Добавляем в список отменяемых правил
                routeControl.cancelRuleList.Add(rule);

                TypeCancel typeCancel = TypeCancel.None;
                Switch? switchCancel = null;

                bool isRuleFree = true;
                // Проверка что все маршруты правила свободны
                foreach (Route route in rule.routes) {
                    if (route.section?.getState() == true) {
                        isRuleFree = false;
                        break;
                    }
                }

                // Если путь не имеет занятей
                if (isRuleFree) {
                    print($"Отмена: свободной секции", Color.OrangeRed);
                    typeCancel = TypeCancel.Free;
                    switchCancel = routeControl.indCancelFree;
                // Если путь занят + маневровый маршрут
                } else if (!isRuleFree && rule.type == TypeRoute.Shunt) {
                    print($"Отмена: маневрового правила", Color.OrangeRed);
                    typeCancel = TypeCancel.Shunt;
                    switchCancel = routeControl.indCancelShunt;
                // Если путь занят + поездной маршрут
                } else if (!isRuleFree && rule.type == TypeRoute.Train) {
                    print($"Отмена: поездного правила", Color.OrangeRed);
                    typeCancel = TypeCancel.Train;
                    switchCancel = routeControl.indCancelTrain;
                }

                // Включаем определенный индикатор отмены
                if (switchCancel != null) {
                    switchCancel.setState(true);
                    switchCancel.sendState();
                }

                // Задержка отключения маршрута
                switch (typeCancel) {
                    case TypeCancel.Free:
                        await Task.Delay(1000);
                        break;
                    case TypeCancel.Shunt:
                        await Task.Delay(3000);
                        break;
                    case TypeCancel.Train:
                        await Task.Delay(5000);
                        break;
                }

                // Удаляем правило из списка отменяемых правил
                if (typeCancel != TypeCancel.None) {
                    // Отключаем индикатор отмены определенного типа
                    if (switchCancel != null) {
                        switchCancel.setState(false);
                        switchCancel.sendState();
                    }
                    // Отключаем маршрут
                    rule.clear();
                    // Удаляем правило из списка отменяемых правил
                    routeControl.cancelRuleList.Remove(rule);
                    // Удаление правила из общего списка
                    ruleList.Remove(rule);

                    // Если нету отменяемых правил - отключаем индикатор
                    if (routeControl.cancelRuleList.Count == 0) {
                        if (routeControl.indCancel.getState()) {
                            routeControl.indCancel.setState(false);
                            routeControl.indCancel.sendState();
                        }
                    }
                }
                break;
            }
        }

    }
    private enum TypeCancel {
        None, // Нет
        Free, // Свободная секция
        Shunt, // Маневровый маршрут
        Train // Поездной маршрут
    }

}
