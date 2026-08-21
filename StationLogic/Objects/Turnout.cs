using static ConsoleLog;

// Комплексная стрелка (две одиночные стрелки или одна одиночная)
public class TurnPair {
    public string name; // Имя стрелки
    public List<TurnSolo> t = new(2); // Список одиночных стрелок
    private Contact control; // Переключатель стрелки
    public TurnControl tc; // Панель управления стрелкой
    public bool isError = false; // Наличие ошибки обратной связи
    private bool enaFB = true; // Разрешение обратной связи стрелки
    
    // Режим работы стрелки
    public enum Mode {
        Manual, // Ручной режим
        Auto, // Автоматический режим
    }
    public Mode mode = Mode.Manual;
    
    // События отправки команд для сервера
    public event Action<Switch>? actionSendSwitch;
    public event Action<Contact>? actionSendContact;

    // Конструктор
    public TurnPair(string name, List<TurnSolo?> t, Contact control, TurnControl turnControl) {
        this.name = name;
        this.t = t.Where(turn => turn != null).Select(turn => turn!).ToList();
        this.control = control;
        this.tc = turnControl;
        // Подписки для отправки команд на сервер
        this.tc.actionSendContact += (contact) => actionSendContact?.Invoke(contact);
        this.tc.actionSendSwitch += (sw) => actionSendSwitch?.Invoke(sw);
    }

    // Инициализация
    public void init() {
        setModeControl(false); // Режим контроля стрелки
        setEnaFB(true); // Разрешение обратной связи стрелки
        // Индикация стрелок
        foreach (TurnSolo turn in t) {
            turn.setIndC(false);
            turn.setIndT(false);
        }
        // Панель управления (обратная связь)
        tc.fbC.setState(false);
        tc.fbT.setState(false);
        // Панель управления (кнопки)
        pressButtonControl(tc.buttonAuto, true);
        updateFBLost();
    }

    // Разрешение изменения состояния стрелки
    public bool enaChange() {
        foreach (TurnSolo turn in t) {
            Section? section = turn.section;
            // Если на стрелке есть занятость секции
            if (section != null && section.getState()) {
                return false;
            }
            // Если на стрелке есть маршрут
            foreach (Route? route in section?.route ?? new List<Route?>()) {
                if (route != null && route.getState()) {
                    return false;
                }
            }
        }
        return true;
    }

    // Состояния стрелки
    public void setState(bool state) {
        // Если запрещено изменение состояния стрелки
        if (!enaChange()) {
            print($"Запрещено изменение состояния стрелки: {name}", Color.Red);
            return;
        }

        // Установка состояния стрелки
        control.setState(state);
        foreach (TurnSolo turn in t) {
            turn.setState(state);
        }
        updateFBLost(); // Обновление состояния обратной связи стрелки
    }
    public bool getState() => control.getState();
    public void sendState() => actionSendContact?.Invoke(control);

    // Обратная связь стрелки (обновление состояния fbLost)
    public void updateFBLost() {
        bool prevError = isError;
        // Проверка состояния обратной связи стрелки
        if ( (!control.getState() && tc.fbC.getState() && !tc.fbT.getState()) // Стрелка в положении C
        || (control.getState() && !tc.fbC.getState() && tc.fbT.getState()) // Стрелка в положении T
        || !enaFB) { // Запрещена обратная связь
            isError = false;
        } else {
            isError = true;
        }
        // Если состояние обратной связи стрелки изменилось
        print($"updateFBLost: {isError},"
            + $"стрелка: {name}, "
            + $"fbC: {tc.fbC.getState()}, "
            + $"fbT: {tc.fbT.getState()}, "
            + $"control: {control.getState()}, "
            + $"enaFB: {enaFB}");
        if (prevError != isError) {
            foreach (TurnSolo turn in t) {
                turn.isError = isError;
                turn.updateInd();
            }
            tc.fbLost.setState(isError); // Обновление состояния
            tc.fbLost.sendState(); // Отправка состояния на сервер
        }
    }
    public void setFBC(bool state) {
        tc.fbC.setState(state);
        updateFBLost();
    }
    public void setFBT(bool state) {
        tc.fbT.setState(state);
        tc.fbT.sendState();
        updateFBLost();
    }
    public bool getFBC() => tc.fbC.getState();
    public bool getFBT() => tc.fbT.getState();
    public void setEnaFB(bool state) {
        if (enaFB != state) {
            enaFB = state;
            updateFBLost();
        }
        print($"setEnaFB: {state}, стрелка: {name}");

    }

    // Кнопки управления стрелкой (C, Auto, T)
    public void pressButtonControl(Switch button, bool enaSend = false) {
        tc.setButtonPressed(button);
        if (enaSend) {
            tc.sendButtonPressed();
        }
        
        if (tc.buttonC == button) {
            setState(false);
            sendState();
        } else if (tc.buttonT == button) {
            setState(true);
            sendState();
        } else {
            return;
        }
    }

    // Контроль стрелки
    public void setModeControl(bool state) {
        foreach (TurnSolo turn in t) {
            // Обновление флага режима контроля стрелки
            turn.isModeControl = state;
            turn.updateInd();
        }
    }

}

// Стрелка
public class TurnSolo {
    public string name; // Имя стрелки
    private Switch control; // Реальный переключатель стрелки (диспетчерский)
    private Contact indC; // Индикатор стрелки (C)
    private Contact indT; // Индикатор стрелки (T)

    public List<Section> sections = new(); // Секции стрелки
    public Section? section => sections.FirstOrDefault(); // Первая секция стрелки для старого кода
    public bool isModeControl = false; // Контроль стрелки
    public bool isError = false; // Наличие ошибки обратной связи

    // Событие изменения состояния стрелки
    public event Action<Contact>? actionSendContact;

    // Конструктор
    public TurnSolo(string name, Switch control, Contact indC, Contact indT) {
        this.name = name;
        this.control = control;
        this.indC = indC;
        this.indT = indT;
    }

    // Состояния стрелки
    public void setState(bool state) => control.setState(state);
    public bool getState() => control.getState();

    // Команды для индикаторов стрелки
    public void setIndC(bool state) => indC.setState(state);
    public void setIndT(bool state) => indT.setState(state);
    public bool getIndC() => indC.getState();
    public bool getIndT() => indT.getState();
    public void sendIndC() => actionSendContact?.Invoke(this.indC);
    public void sendIndT() => actionSendContact?.Invoke(this.indT);

    // Обновление индикаторов стрелки
    public void updateInd() {
        bool newStateIndC = false;
        bool newStateIndT = false;
        bool isBusy = sections.Any(section => section.getState());
        // Если стрелка в режиме контроля
        if (isModeControl) {
            // Если нету ошибки обратной связи
            if (!isError) {
                newStateIndC = getState();
                newStateIndT = !getState();
            }
        // Если стрелка не в режиме контроля + имеет занятость секции
        } else if (isBusy) {
            newStateIndC = getState();
            newStateIndT = !getState();
        // Если стрелка не в режиме контроля + нету занятости секции
        } else {
            newStateIndC = false;
            newStateIndT = false;
        } 
        if (getIndC() != newStateIndC) {
            setIndC(newStateIndC);
            sendIndC();
        }
        if (getIndT() != newStateIndT) {
            setIndT(newStateIndT);
            sendIndT();
        }
    }
}

// Панель управления стрелками
public class TurnControl {
    public Contact fbC; // Обратная связь стрелки (C)
    public Contact fbLost; // Обратная связь стрелки (автоматическое)
    public Contact fbT; // Обратная связь стрелки (T)
    public Switch buttonC; // Управление стрелкой (C)
    public Switch buttonT; // Управление стрелкой (T)
    public Switch buttonAuto; // Управление стрелкой (автоматическое)

    // Нажатая кнопка управления стрелкой
    private Switch? buttonPrevPressed;
    private Switch? buttonPressed;

    // События отправки команд для сервера
    public event Action<Contact>? actionSendContact;
    public event Action<Switch>? actionSendSwitch;

    // Конструктор
    public TurnControl(Contact fbC, Contact fbLost, Contact fbT, 
                        Switch buttonC, Switch buttonAuto, Switch buttonT) {
        this.fbC = fbC;
        this.fbLost = fbLost;
        this.fbT = fbT;
        this.buttonC = buttonC;
        this.buttonAuto = buttonAuto;
        this.buttonT = buttonT;

        // Подписки для отправки команд на сервер
        fbC.actionSend += contact => actionSendContact?.Invoke(contact);
        fbT.actionSend += contact => actionSendContact?.Invoke(contact);
        fbLost.actionSend += contact => actionSendContact?.Invoke(contact);
        buttonC.actionSend += sw => actionSendSwitch?.Invoke(sw);
        buttonAuto.actionSend += sw => actionSendSwitch?.Invoke(sw);
        buttonT.actionSend += sw => actionSendSwitch?.Invoke(sw);
    }

    // Команды нажатой кнопки
    public void setButtonPressed(Switch? button) {
        buttonPrevPressed = buttonPressed;
        buttonPressed = button;
        buttonC.setState(button == buttonC);
        buttonAuto.setState(button == buttonAuto);
        buttonT.setState(button == buttonT);
    }
    public Switch? getButtonPressed() => buttonPressed;
    public Switch? getButtonPrevPressed() => buttonPrevPressed;
    public void sendButtonPressed() {
        if (buttonPrevPressed != null) {
            actionSendSwitch?.Invoke(buttonPrevPressed);
        }
        if (buttonPressed != null) {
            actionSendSwitch?.Invoke(buttonPressed);
        }
    }
}
