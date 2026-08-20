using static ConsoleLog;

public partial class Station {
    // Отправка свитча
    public async void sendSwitch(Switch sw) {
        // Высчитываем сколько нужно еще ждать до отправки команды
        long curTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
        // print($"Send Switch[{sw.addr}] state: {sw.getState()}", Color.SaddleBrown);
        await server.setSwitch(sw.addr, sw.getState());
    }
    
    // Отправка контакта
    public async void sendContact(Contact ci) {
        // print($"Send Contact[{ci.addr}][{ci.input}] state: {ci.getState()}", Color.SaddleBrown);
        switch (ci.type) {
            case Contact.Type.RBus:
                await server.setRBusInput(ci.addr, ci.input, ci.getState());
                break;
            case Contact.Type.CANBus:
                await server.setCanInput(ci.addr, ci.input, ci.getState());
                break;
        }
    }

    // Запрос состояния свитчей с TC
    public async void readSwitchesRequest(ushort addr) {
        if (atSwitchVirt(addr) is Switch sw) {
            if (sw.type == Switch.Type.Virt_Toggle 
            || sw.type == Switch.Type.Virt_Push) {
                await server.setSwitch(addr, sw.getState());
            }
        } else {
            await server.setSwitch(addr, false);
        }
    }

    // Чтение свитчей с TrainController
    private void readSwitches(ushort addr, byte position, bool activate) {
        // print($"Чтение свитча: {addr}, позиция: {position}, активность: {activate}", Color.DodgerBlue);
        Switch? swToggle = atSwitchToggle(addr);
        Switch? swPush = atSwitchPush(addr);
        if (swPush != null) {
            pressPushButton(swPush, activate);
        } else if (swToggle != null && activate) {
            pressToggleSwitch(swToggle, position == 0x01);
        }
    }

    // Обработка нажатия Push Button
    private void pressPushButton(Switch sw, bool state) {
        // print($"Нажатие Push Button: {sw.addr}, состояние: {state}", Color.Cyan);
        sw.setState(state);
        // Кнопка контроля стрелок
        if (sw == buttonTurnControl) {
            foreach (TurnPair turnPair in turnPairList) {
                turnPair.setIndVisible(state);
            }
            return;
        }
        if (state) {
            // Кнопки управления маршрутом
            if (routeControl.pressRouteButton(sw)) {
                return;
            // Кнопка відміни набору
            } else if (sw == routeControl.buttonCancelSet) {
                print($"Кнопка отмены набора: {true}", Color.Green);
                routeControl.clearSet();
                return;
            }
        }
    }

    // Обработка нажатия Toggle Switch
    private void pressToggleSwitch(Switch sw, bool state) {
        // print($"Нажатие Toggle Switch: {sw.addr}, состояние: {state}", Color.Cyan);
        sw.setState(state);
        // Кнопка инициализации станции
        if (sw == buttonInitStation) {
            if (state) {
                initStation();
                buttonInitStation.setState(false);
                buttonInitStation.sendState();
            }
            return;

        // Кнопка инициализации стрелок
        } else if (sw == buttonInitTurn) {
            if (state) {
                initTurn();
                buttonInitTurn.setState(false);
                buttonInitTurn.sendState();
            }
            return;

        // Кнопка разрешения обратной связи
        } else if (sw == buttonEnaFB) {
            foreach (TurnPair turnPair in turnPairList) {
                turnPair.setEnaFB(state);
            }
            return;
        
        // Кнопка відміна маршруту
        } else if (sw == routeControl.buttonCancelRoute) {
            print($"Кнопка отмены маршрута: {state}", Color.Green);

            // Индикатор отмены маршрута
            bool newStateInd;
            // Если кнопку включили
            if (state) {
                newStateInd = true;
                routeControl.clearSet(); // Очистка набора маршрута
            // Если кнопку отключили
            } else {
                // Если отменяемых нету
                if (routeControl.cancelRuleList.Count == 0) {
                    newStateInd = false;
                // Если есть отменяемые правила 
                } else {
                    newStateInd = true;
                }
            }
            if (routeControl.indCancel.getState() != newStateInd) {
                routeControl.indCancel.setState(newStateInd);
                routeControl.indCancel.sendState();
            }
            return;
        }

        // Кнопка занятости секции
        foreach (Section section in sectionList) {
            if (sw == section.control) {
                print($"Секция: {section.name}, состояние: {state}", Color.Green);
                return;
            }
        }

        // Ручное управление парными стрелками
        foreach (TurnPair turnPair in turnPairList) {
            TurnControl tc = turnPair.tc;
            if (sw == tc.buttonC
            || sw == tc.buttonT
            || sw == tc.buttonAuto) {
                turnPair.pressButtonControl(sw);
                print($"C: {tc.buttonC.getState()}, Auto: {tc.buttonAuto.getState()}, T: {tc.buttonT.getState()}");
                return;
            }
        }

        return;
    }

}