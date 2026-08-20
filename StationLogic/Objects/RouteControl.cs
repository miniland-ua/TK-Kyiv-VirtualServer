using static ConsoleLog;

// Направление
public enum Direct {
    Left, // Влево
    Right // Вправо
}

// Панель управления маршрутами
public class RouteControl {
    // Список кнопок маршрутов
    public List<RouteButton> routeButton = new();
    // Кнопка отмены набора
    public Switch buttonCancelSet = new Switch(Switch.Type.Virt_Push, 1406);
    // Кнопка отмены маршрута
    public Switch buttonCancelRoute = new Switch(Switch.Type.Virt_Toggle, 1405);

    // Индикатор процесса отмены
    public Switch indCancel = new Switch(Switch.Type.Virt_Toggle, 1407);
    // Индикатор отмены поездного маршрута
    public Switch indCancelTrain = new Switch(Switch.Type.Virt_Toggle, 1409);
    // Индикатор отмены маневрового маршрута
    public Switch indCancelShunt = new Switch(Switch.Type.Virt_Toggle, 1410);
    // Индикатор отмены свободного маршрута
    public Switch indCancelFree = new Switch(Switch.Type.Virt_Toggle, 1408);

    // Индикатор направления маршрута
    public SwitchControl indDirection = new SwitchControl(1411, 5);

    RouteButton? startButton = null; // Стартовая кнопка маршрута
    RouteButton? finishButton = null; // Конечная кнопка маршрута
    TypeRoute typeRoute; // Тип маршрута
    Direct direct; // Направление маршрута

    // Текущий список создаваемых правил
    public List<Rule> curRuleList = new();

    // Событие отправки команд для сервера
    public event Action<Switch>? actionSendSwitch;
    public event Action<Rule>? actionCreateRule;
    public event Action<Bridge>? actionRemoveRule;


    // Конструктор
    public RouteControl(List<RouteButton>? routeButton = null) {
        if (routeButton != null) {
            this.routeButton = routeButton;
        }

        // Подписки для отправки команд на сервер
        this.buttonCancelSet.actionSend += (sw) => actionSendSwitch?.Invoke(sw);
        this.buttonCancelRoute.actionSend += (sw) => actionSendSwitch?.Invoke(sw);
        this.indCancel.actionSend += (sw) => actionSendSwitch?.Invoke(sw);
        this.indCancelTrain.actionSend += (sw) => actionSendSwitch?.Invoke(sw);
        this.indCancelShunt.actionSend += (sw) => actionSendSwitch?.Invoke(sw);
        this.indCancelFree.actionSend += (sw) => actionSendSwitch?.Invoke(sw);

        foreach (Switch sw in this.indDirection.switches) {
            sw.actionSend += (control) => actionSendSwitch?.Invoke(control);
        }

        foreach (RouteButton button in this.routeButton) {
            button.actionSend += (sw) => actionSendSwitch?.Invoke(sw);
        }

    }

    // Кнопка для отмены маршрута
    public void setButtonCancelRoute(bool state) => buttonCancelRoute.setState(state);
    public bool getButtonCancelRoute() => buttonCancelRoute.getState();
    public void sendButtonCancelRoute() => actionSendSwitch?.Invoke(buttonCancelRoute);

    // Поиск кнопки маршрута
    public RouteButton? atRouteButton(Switch sw) {
        return routeButton.FirstOrDefault(button => button.control == sw);
    }

    // Поиск кратчайшего пути по начальному и конечному мосту
    public List<Route>? findPath(Bridge start, Bridge finish) {
        // Если стартовый мост совпадает с финишным - путь пустой
        if (start == finish) {
            return new List<Route>();
        }

        // Множество посещенных мостов для предотвращения зацикливания
        HashSet<Bridge> visited = new HashSet<Bridge> { start };
        // Очередь мостов для обхода в ширину
        Queue<Bridge> queue = new Queue<Bridge>();
        queue.Enqueue(start);
        // Для каждого моста запоминаем, из какого моста и каким маршрутом в него пришли
        Dictionary<Bridge, (Bridge prev, Route route)> cameFrom = new();

        // Пока есть мосты для обхода
        while (queue.Count > 0) {
            // Берем следующий мост из очереди
            Bridge cur = queue.Dequeue();
            // Перебираем все маршруты текущего моста
            foreach (Route route in cur.routeList) {
                // Определяем следующий мост маршрута по направлению
                Bridge? next = (direct == Direct.Left) ? route.bridgeLeft : route.bridgeRight;
                // Если следующего моста нет, он ведет назад или уже посещен - пропускаем
                if (next == null || next == cur || visited.Contains(next)) {
                    continue;
                }
                // Помечаем мост посещенным и запоминаем, откуда пришли
                visited.Add(next);
                cameFrom[next] = (cur, route);
                // Если дошли до финишного моста
                if (next == finish) {
                    // Восстанавливаем путь от финишного моста к стартовому
                    List<Route> path = recoverPath(cameFrom, start, finish);
                    // Корректируем путь, если нужно
                    correctPath(path);
                    // Фильтруем создание маршрута по найденному пути
                    return filterCreateRule(path) ? path : null;
                }
                // Иначе добавляем мост в очередь для дальнейшего обхода
                queue.Enqueue(next);
            }
        }
        // Путь не найден
        return null;
    }
    // Восстановление пути от финишного моста к стартовому
    private List<Route> recoverPath(Dictionary<Bridge, (Bridge prev, Route route)> cameFrom, Bridge start, Bridge finish) {
        // Список маршрутов пути
        List<Route> path = new List<Route>();
        // Идем назад от финиша к старту
        Bridge node = finish;
        while (node != start) {
            // Берем мост и маршрут, из которого пришли в текущий
            (Bridge prev, Route route) = cameFrom[node];
            // Добавляем маршрут в путь
            path.Add(route);
            // Переходим к предыдущему мосту
            node = prev;
        }
        // Шли от финиша - переворачиваем путь в правильный порядок
        path.Reverse();
        return path;
    }
    // Корректировка маршрута при поиске пути
    private void correctPath(List<Route> routeList) {
        // Если следущая секция после конечного моста имеет лишь 1 маршрут - добавляем его
        Section? nextSection = finishButton?.bridge.sectionList
            .ElementAtOrDefault(direct == Direct.Left ? 0 : 1);
        if (nextSection != null && nextSection.route.Count == 1) {
            Route? nextRoute = nextSection.route[0];
            if (nextRoute != null) {
                routeList.Add(nextRoute);
            }
        }
    }
    // Фильтрация при поиске пути
    private bool filterCreateRule(List<Route> routeList) {
        // Для маневровых маршрутов
        if (typeRoute == TypeRoute.Shunt) {
            // Если последняя секция имеет стрелки + конечный светофор в противоположном направлении
            Section? lastSection = routeList.LastOrDefault()?.section;
            if (lastSection != null && lastSection.turnPair.Count > 0) {
                TrafficLight? trafficLight = finishButton?.bridge.trafficLight;
                if (trafficLight != null && trafficLight.dir != direct) {
                    return false;
                }
            }
        }

        foreach (Route route in routeList) {
            // Если один из маршрутов уже активный
            if (route.getState()) {
                return false;
            }
            // Если одна из стрелок по пути имеет ошибку обратной связи
            foreach ((TurnPair turnPair, bool state) in route.turn) {
                if (state == true && !turnPair.isNormalState) {
                    return false;
                }
            }
            // Если на секция с маршрутом есть занятый участок
            Section? section = route.section;
            if (section?.getState() == true) {
                return false;
            }
        }

        return true;
    }

    // Направления маршрута
    public void updateDirection() {
        if (startButton == null) {
            indDirection.setState(0);
            return;
        }
        Bridge bridge = startButton.bridge;
        if (bridge == null) {
            indDirection.setState(0);
            return;
        }
        TrafficLight? trafficLight = bridge.trafficLight;
        if (trafficLight == null) {
            return;
        }
        // Обновление направления маршрута
        direct = trafficLight.dir;
        // Обновление типа маршрута
        if (startButton.type == RouteButton.Type.Shunt) {
            typeRoute = TypeRoute.Shunt;
        } else {
            typeRoute = TypeRoute.Train;
        }
        // Left + train
        if (direct == Direct.Left && typeRoute == TypeRoute.Train) {
            indDirection.setState(2);
        // Left + shunt
        } else if (direct == Direct.Left && typeRoute == TypeRoute.Shunt) {
            indDirection.setState(4);
        }
        // Right + train
        if (direct == Direct.Right && typeRoute == TypeRoute.Train) {
            indDirection.setState(1);
        // Right + shunt
        } else if (direct == Direct.Right && typeRoute == TypeRoute.Shunt) {
            indDirection.setState(3);
        }
    }
    public int getDirection() => indDirection.getState();
    public void sendDirection() {
        indDirection.sendState();
    }

    // Обработка нажатия кнопки маршрута
    public bool pressRouteButton(Switch sw) {
        // Находим кнопку маршрута по переключателю
        RouteButton? button = atRouteButton(sw);
        if (button == null) {
            return false;
        }
        // Если нажата кнопка отмены маршрута - отменяем маршрут
        if (buttonCancelRoute.getState()) {
            // Отменяем маршрут за мостом
            actionRemoveRule?.Invoke(button.bridge);
            // Отключаем кнопку отмены маршрута
            setButtonCancelRoute(false);
            sendButtonCancelRoute();
            return true;
        }

        if (startButton == null) {
            // print($"Стартовая кнопка маршрута: {button.bridge.name}", Color.Gold);
            startButton = button;
            // Обновление направления маршрута
            updateDirection();
            sendDirection();
            // Включаем индикатор светофора
            TrafficLight? trafficLight = startButton.bridge.trafficLight;
            if (trafficLight != null) {
                trafficLight.setIndState(true);
                trafficLight.sendIndState();
            }
            
        } else if (finishButton == null) {
            // print($"Конечная кнопка маршрута: {button.bridge.name}", Color.Gold);
            finishButton = button;
            // Создание маршрута
            if (create()) {
                print($"Создан маршрут: {startButton.bridge.name} -> {finishButton.bridge.name}", Color.Green);
                // Постройка маршрута
                foreach (Rule rule in curRuleList) {
                    rule.build();
                    //Задержка для построения маршрута
                    // System.Threading.Thread.Sleep(1000);
                }
            } else {
                print($"Ошибка создания маршрута: {startButton.bridge.name} -> {finishButton.bridge.name}", Color.Red);
            }
            clearSet(); // Очистка набора маршрута
        }
        return true;
    }

    // Создание набора
    public bool create() {
        // Если стартовая и финишная кнопка имеют разные типы - выходим
        if (startButton?.type != finishButton?.type) {
            return false;
        }
        // Если стартовая и финишная кнопка совпадают - выходим
        if (startButton == finishButton) {
            return false;
        }

        // Ищем кратчайший путь от стартового моста до финишного моста
        List<Route>? routeList = findPath(startButton!.bridge, finishButton!.bridge);
        // Если путь не найден - выходим
        if (routeList == null) {
            return false;
        }

        // Разделенные маршруты на несколько правил
        int startIndex = 0;
        for (int i = 0; i < routeList.Count; i++) {
            Route route = routeList[i];
            // Если маршрут заканчивается на светофоре в нужном направлении - создаем правило
            Bridge? endBridge = (direct == Direct.Left) ? route.bridgeLeft : route.bridgeRight;
            if (endBridge?.trafficLight?.dir == direct) {
                // Если маневровый маршрут или поездной маршрут + конечный светофор не маневровый
                if (typeRoute == TypeRoute.Shunt
                || (typeRoute == TypeRoute.Train && endBridge?.trafficLight?.type != TrafficLight.Type.Shunting)) {
                    // Добавляем правило (добавляем все предыдущие маршруты до текущего)
                    addRule(routeList.GetRange(startIndex, i + 1 - startIndex));
                    startIndex = i + 1;
                }
            }
        }
        // Остаток маршрутов добавляем в правило
        if (startIndex < routeList.Count) {
            addRule(routeList.GetRange(startIndex, routeList.Count - startIndex));
        }
        return true;
    }

    // Добавление правила в список
    public void addRule(List<Route> routeList) {
        // Определяем правильность пути (для поездного маршрута)
        bool isCorrectWay = true;
        if (typeRoute == TypeRoute.Train) {
            foreach (Route route in routeList) {
                foreach ((TurnPair turnPair, bool state) in route.turn) {
                    if (state == true) {
                        isCorrectWay = false;
                        break;
                    }
                }
            }

        }
        print($"Создано правило маршрута: {string.Join(" -> ", routeList.Select(r => r.name))}, " +
                $"Тип: {typeRoute}, " +
                $"Направление: {direct}, " +
                $"Тип правила: {typeRoute}, " +
                $"Правильный путь: {isCorrectWay}", 
            Color.Green);
        Rule rule = new Rule(routeList, typeRoute, direct, isCorrectWay);
        curRuleList.Add(rule);
        actionCreateRule?.Invoke(rule);
    }

    // Очистка набора маршрута
    public void clearSet() {
        // Отключаем индикатор светофора
        TrafficLight? trafficLight = startButton?.bridge.trafficLight;
        if (trafficLight != null) {
            trafficLight.setIndState(false);
            trafficLight.sendIndState();
        }
        // Очистка набора маршрута
        startButton = null;
        finishButton = null;
        curRuleList.Clear();
        updateDirection();
        sendDirection();
    }
        
}
