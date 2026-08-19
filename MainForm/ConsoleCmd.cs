using System.Globalization;
using System.Net;

static class ConsoleLog {
    public static event Action<string, Color?>? Printed;

    public static void print(string str, Color? color = null) {
        color ??= Color.White; // Если цвет не указан, используем белый цвет
        // Добавление вывода времени
        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
        string line = $"[{timestamp}] {str}";
        // Вызов печати в консоль с цветом
        Printed?.Invoke(line, color);
    
        try {
            Console.WriteLine(line); // Печать в консоль
        // Игнорируем исключения, если консоль недоступна
        } catch (IOException) {
        } catch (InvalidOperationException) {
        }
    }
}


partial class MainForm {
    // UDP-сокет для отправки команд Z21.
    private IPEndPoint Z21ServerAddress => new(IPAddress.Parse(sta.server.Z21Host), Z21Server.Z21Port);

    // Печать текста в консоль с цветом
    public void print(string line, Color? color = null) {
        color ??= Color.White; // Если цвет не указан, используем белый цвет
        // Если форма уже закрыта, выходим
        if (IsDisposed || Disposing || !IsHandleCreated) {
            return;
        }

        // Если вызов из другого потока, то используем BeginInvoke для вызова метода в UI-потоке
        if (InvokeRequired) {
            try {
                BeginInvoke(new Action(delegate {
                    print(line, color);
                }));
            // Игнорируем, если форма уже закрыта
            } catch (ObjectDisposedException) {
            } catch (InvalidOperationException) {
            }
            return;
        }

        textBox_log.SelectionStart = textBox_log.TextLength; // Устанавка курсора в конец текста
        textBox_log.SelectionLength = 0; // Убираем выделение текста
        color ??= Color.White; // Если цвет не указан, используем белый цвет
        textBox_log.SelectionColor = color.Value; // Установка цвета текста
        textBox_log.AppendText(line + Environment.NewLine); // Добавление текста с новой строкой
        textBox_log.SelectionColor = textBox_log.ForeColor; // Возврат цвета текста к исходному
        textBox_log.ScrollToCaret(); // Прокрутка к курсору, чтобы показать последнюю строку
    }

    // Чтение значений int типа из строки
    private int readInt(string str) {
        // Если строка начинается с "0x", то парсим как шестнадцатеричное число
        if (str.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) {
            return int.Parse(str[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }
        // Иначе парсим как десятичное число
        return int.Parse(str, CultureInfo.InvariantCulture);
    }

    // Чтение значений bool типа из строки
    private bool readBool(string str) {
        if (str == "1" 
        || str.Equals("true", StringComparison.OrdinalIgnoreCase)
        || str.Equals("on", StringComparison.OrdinalIgnoreCase)
        || str.Equals("closed", StringComparison.OrdinalIgnoreCase)
        ) {
            return true;
        } else {
            return false;
        }
    }

    // Чтение команды через textBox_cmd.
    private async Task SendCommandFromUiAsync() {
        // Убираем пробелы в начале и конце строки.
        string line = textBox_cmd.Text.Trim();
        // Если строка пустая, выходим
        if (string.IsNullOrWhiteSpace(line)) {
            return;
        }

        textBox_cmd.SelectAll(); // Выделяем текст в textBox_cmd 
        textBox_cmd.Focus(); // Фокусируемся на textBox_cmd

        try {
            // Разделяем строку на аргументы, удаляя пустые и обрезая пробелы
            string[] args = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            // Читаем количество аргументов
            if (args.Length < 1) {
                return;
            }
            int size = args.Length;

            // Имя команды
            string command = args[0];

            switch (command) {
                case "help": {
                    PrintAvailableCommands();
                    break;
                }
                
                case "power": {
                    if (size >= 2) {
                        bool state = readBool(args[1]);
                        await sta.server.setPowerStatus(state);
                    }
                    break;
                }

                case "sw": {
                    if (size >= 3) {
                        int addr = readInt(args[1]);
                        bool state = readBool(args[2]);
                        await sta.server.setSwitch(addr, state);
                    }
                    break;
                }

                case "turn": {
                    if (size >= 3) {
                        string name = args[1];
                        bool state = readBool(args[2]);
                        sta.atTurnPair(name)?.setState(state);
                        sta.atTurnPair(name)?.sendState();
                        print($"Стрелка {name} установлена в состояние {state}", Color.Green);
                    }
                    break;
                }

                case "turnIndC": {
                    if (size >= 3) {
                        string name = args[1];
                        bool state = readBool(args[2]);
                        sta.atTurnSolo(name)?.setIndC(state);
                        sta.atTurnSolo(name)?.sendIndC();
                        print($"Индикатор C стрелки {name} установлен в состояние {state}", Color.Green);
                    }
                    break;
                }

                case "turnIndT": {
                    if (size >= 3) {
                        string name = args[1];
                        bool state = readBool(args[2]);
                        sta.atTurnSolo(name)?.setIndT(state);
                        sta.atTurnSolo(name)?.sendIndT();
                        print($"Индикатор T стрелки {name} установлен в состояние {state}", Color.Green);
                    }
                    break;
                }

                case "turnFBC": {
                    if (size >= 3) {
                        string name = args[1];
                        bool state = readBool(args[2]);
                        sta.atTurnPair(name)?.tc.fbC.setState(state);
                        sta.atTurnPair(name)?.tc.fbC.sendState();
                        print($"Индикатор C стрелки {name} установлен в состояние {state}", Color.Green);
                    }
                    break;
                }

                case "turnFBT": {
                    if (size >= 3) {
                        string name = args[1];
                        bool state = readBool(args[2]);
                        sta.atTurnPair(name)?.tc.fbT.setState(state);
                        sta.atTurnPair(name)?.tc.fbT.sendState();
                        print($"Индикатор T стрелки {name} установлен в состояние {state}", Color.Green);
                    }
                    break;
                }

                case "turnFBLost": {
                    if (size >= 3) {
                        string name = args[1];
                        bool state = readBool(args[2]);
                        sta.atTurnPair(name)?.tc.fbLost.setState(state);
                        sta.atTurnPair(name)?.tc.fbLost.sendState();
                        print($"Обратная связь C стрелки {name} установлена в состояние {state}", Color.Green);
                    }
                    break;
                }

                case "turnButtonC": {
                    if (size >= 3) {
                        string name = args[1];
                        bool state = readBool(args[2]);
                        sta.atTurnPair(name)?.tc.buttonC.setState(state);
                        sta.atTurnPair(name)?.tc.buttonC.sendState();
                        print($"Кнопка C стрелки {name} установлена в состояние {state}", Color.Green);
                    }
                    break;
                }

                case "turnButtonAuto": {
                    if (size >= 3) {
                        string name = args[1];
                        bool state = readBool(args[2]);
                        sta.atTurnPair(name)?.tc.buttonAuto.setState(state);
                        sta.atTurnPair(name)?.tc.buttonAuto.sendState();
                        print($"Кнопка Auto стрелки {name} установлена в состояние {state}", Color.Green);
                    }
                    break;
                }

                case "turnButtonT": {
                    if (size >= 3) {
                        string name = args[1];
                        bool state = readBool(args[2]);
                        sta.atTurnPair(name)?.tc.buttonT.setState(state);
                        sta.atTurnPair(name)?.tc.buttonT.sendState();
                        print($"Кнопка T стрелки {name} установлена в состояние {state}", Color.Green);
                    }
                    break;
                }

                case "tl": {
                    if (size >= 3) {
                        string name = args[1];
                        int state = readInt(args[2]);
                        sta.atTL(name)?.setState(state);
                        sta.atTL(name)?.sendState();
                        print($"Светофор {name} установлен в состояние {state}", Color.Green);
                    }
                    break;
                }

                case "tlInd": {
                    if (size >= 3) {
                        string name = args[1];
                        bool state = readBool(args[2]);
                        sta.atTL(name)?.setIndState(state);
                        sta.atTL(name)?.sendIndState();
                        print($"Индикатор светофора {name} установлен в состояние {state}", Color.Green);
                    }
                    break;
                }

                case "route": {
                    if (size >= 3) {
                        string name = args[1];
                        bool state = readBool(args[2]);
                        sta.atRoute(name)?.build();
                        print($"Маршрут {name} установлен в состояние {state}", Color.Green);
                    }
                    break;
                }

                case "indDirect": {
                    if (size >= 3) {
                        string name = args[1];
                        int state = readInt(args[2]);
                        sta.routeControl.indDirection.setState(state);
                        sta.routeControl.indDirection.sendState();
                        print($"Индикатор направления маршрута {name} установлен в состояние {state}", Color.Green);
                    }
                    break;
                }

                case "clear": {
                    sta.clearAllRoutes();
                    sta.clearAllTrafficLights();
                    print($"Все маршруты и светофоры очищены", Color.Green);
                    break;
                }









                case "bus": {
                    if (size >= 4) {
                        int addr = readInt(args[1]);
                        int input = readInt(args[2]);
                        bool state = readBool(args[3]);
                        await sta.server.setRBusInput(addr, input, state);
                    }
                    break;
                }

                case "can": {
                    if (size >= 4) {
                        int addr = readInt(args[1]);
                        int input = readInt(args[2]);
                        bool state = readBool(args[3]);
                        await sta.server.setCanInput(addr, input, state);
                    }
                    break;
                }

                case "access": {
                    if (size >= 3) {
                        int addr = readInt(args[1]);
                        int state = readInt(args[2]);
                        await sta.server.setExtAccessory(addr, state);
                    }
                    break;
                }



                case "sect": {
                    if (size >= 3) {
                        string name = args[1];
                        bool state = readBool(args[2]);
                        sta.atSect(name)?.setState(state);
                        sta.atSect(name)?.sendState();
                        print($"Секция {name} установлена в состояние {state}", Color.Green);
                    }
                    break;
                }

                case "infoTurnSolo": {
                    print("Список одиночных стрелок:");
                     foreach (TurnSolo turn in sta.turnSoloList) {
                        print($"Стрелка {turn.name}: state = {turn.getState()}, indC = {turn.getIndC()}, indT = {turn.getIndT()}");
                    }
                    break;
                }

                case "infoTurnPair": {
                    print("Список комплексных стрелок:");
                     foreach (TurnPair turn in sta.turnPairList) {
                        print($"Стрелка {turn.name}:");
                        print($"  Свитч: {turn.getState()}");
                        print($"  Индикатор C: {turn.tc.fbC.getState()}");
                        print($"  Индикатор T: {turn.tc.fbT.getState()}");
                        print($"  Ошибка обратной связи: {turn.tc.fbLost.getState()}");
                        print($"  Кнопка C: {turn.tc.buttonC.getState()}");
                        print($"  Кнопка Auto: {turn.tc.buttonAuto.getState()}");
                        print($"  Кнопка T: {turn.tc.buttonT.getState()}");
                    }
                    break;
                }

                case "infoSwitch": {
                    print("Список переключателей:");
                    // Сортировка всего списка переключателей по адресу
                    sta.switchList.Sort((sw1, sw2) => sw1.addr.CompareTo(sw2.addr));
                     foreach (Switch sw in sta.switchList) {
                        print($"Переключатель addr = {sw.addr}: state = {sw.getState()}");
                    }
                    break;
                }

                case "listRules": {

                    print($"Текущий список правил {sta.ruleList.Count}: {string.Join(", ", sta.ruleList.Select(r => r.startBridge?.name + " -> " + r.finishBridge?.name))}");
                    break;
                }

                default: {
                    print("Неизвестная команда: " + command, Color.Red);
                    break;
                }
            }
        } catch (Exception ex) {
            ConsoleLog.print("Ошибка команды: " + ex.Message, Color.Red);
        }
    }
}
