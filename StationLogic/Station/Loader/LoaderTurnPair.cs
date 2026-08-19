public partial class Station {
    // Загрузка парных стрелок
    public bool loadTurnPair() {
        // Проверяем окончание списка файлов
        if (loaderData.sum <= loaderData.curIndex) {
            return false;
        }

        // Путь читаемого файла
        string filePath = loaderData.files[loaderData.curIndex];

        // Шаблон параметра <name, value>
        Dictionary<string, string> fields = new();
        List<string> turnNames = new();
        // Чтение всех строк файла
        string[] lines = File.ReadAllLines(filePath);

        // Читаем параметры из файла
        for (int i = 1; i < lines.Length; i++) {
            string line = lines[i].Trim();
            // Окончание блока параметров - выход из цикла
            if (line == "---") {
                break;
            }

            // Читаем одиночные стрелки из списка Turns
            if (line.StartsWith("-")) {
                string turnName = line
                    .TrimStart('-')
                    .Trim()
                    .Trim('"')
                    .Replace("[[", "")
                    .Replace("]]", "");

                turnName = turnName.Split('/').Last();
                turnNames.Add(turnName);
                continue;
            }

            // Разделяем строку на имя и значение
            string[] parts = line.Split(':', 2);
            if (parts.Length == 2 && parts[1].Trim() != "") {
                string key = parts[0].Trim();
                string value = parts[1].Trim().Trim('"');
                fields[key] = value;
            }
        }

        // Разделяем индексы контактов
        string[] addr = fields["addr_CV"].Split(',');
        string[] fbC = fields["FB_C_C"].Split(',');
        string[] fbLost = fields["FB_Lost_CV"].Split(',');
        string[] fbT = fields["FB_T_C"].Split(',');
        int stateBaseAddr = int.Parse(fields["state_SV"]);

        // Находим одиночные стрелки по именам
        List<TurnSolo?> turns = new();
        foreach (string turnName in turnNames) {
            turns.Add(turnSoloList.Find(turn => turn.name == turnName));
        }

        TurnPair turnPair = new(
            // Имя парной стрелки
            Path.GetFileNameWithoutExtension(filePath),
            // Список одиночных стрелок
            turns,
            // Contact для управления стрелкой
            createContact(Contact.Type.CANBus, int.Parse(addr[0]), int.Parse(addr[1])),
            // Панель управления стрелкой
            new TurnControl(
                // Индикатор Обратной связи состояния C
                createContact(Contact.Type.CANBus, int.Parse(fbC[0]), int.Parse(fbC[1])),
                // Индикатор Обратной связи состояния Lost
                createContact(Contact.Type.CANBus, int.Parse(fbLost[0]), int.Parse(fbLost[1])),
                // Индикатор Обратной связи состояния T
                createContact(Contact.Type.CANBus, int.Parse(fbT[0]), int.Parse(fbT[1])),
                // Кнопка C
                createSwitch(Switch.Type.Virt_Toggle, stateBaseAddr),
                // Кнопка Auto
                createSwitch(Switch.Type.Virt_Toggle, stateBaseAddr + 1),
                // Кнопка T
                createSwitch(Switch.Type.Virt_Toggle, stateBaseAddr + 2)
            )
        );
        turnPairList.Add(turnPair);

        loaderData.curIndex++;
        return true;
    }
}
