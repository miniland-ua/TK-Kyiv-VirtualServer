public partial class Station {
    // Загрузка секции
    public bool loadSection() {
        // Проверяем окончание списка файлов
        if (loaderData.sum <= loaderData.curIndex) {
            return false;
        }

        // Путь читаемого файла
        string filePath = loaderData.files[loaderData.curIndex];

        // Шаблон параметра <name, value>
        Dictionary<string, string> fields = new();
        // Список имен маршрутов секции
        List<string> routeNames = new();
        // Список имен одиночных стрелок секции
        List<string> turnSoloNames = new();
        string? currentList = null;
        // Чтение всех строк файла
        string[] lines = File.ReadAllLines(filePath);

        // Читаем параметры из файла
        for (int i = 1; i < lines.Length; i++) {
            string line = lines[i].Trim();

            // Окончание блока параметров - выход из цикла
            if (line == "---") {
                break;
            }

            // Читаем элементы текущего списка
            if (line.StartsWith("-")) {
                string itemName = line
                    .TrimStart('-')
                    .Trim()
                    .Trim('"')
                    .Replace("[[", "")
                    .Replace("]]", "");

                // Убираем путь к файлу
                itemName = itemName.Split('/').Last();

                if (currentList == "Routes") {
                    // Убираем префикс маршрута
                    if (itemName.StartsWith("R_")) {
                        itemName = itemName[2..];
                    }
                    routeNames.Add(itemName);
                } else if (currentList == "turnSolo") {
                    turnSoloNames.Add(itemName);
                }
                continue;
            }

            // Разделяем строку на 2 части
            string[] parts = line.Split(':', 2);
            if (parts.Length == 2) {
                // Имя параметра
                string key = parts[0].Trim();
                currentList = key is "Routes" or "turnSolo" ? key : null;

                // Значение параметра (кавычки убираем если они есть)
                string value = parts[1].Trim().Trim('"');
                if (value != "") {
                    fields[key] = value;
                }
            }
        }

        // Находим маршруты секции по именам
        List<Route?> sectionRoutes = new();
        foreach (string routeName in routeNames) {
            Route? route = routeList.Find(route => route.name == routeName);

            if (route == null) {
                throw new InvalidDataException(
                    $"Не найден маршрут '{routeName}' для секции '{filePath}'"
                );
            }

            sectionRoutes.Add(route);
        }

        // Находим одиночные стрелки секции по именам
        List<TurnSolo?> sectionTurnSolo = new();
        foreach (string turnName in turnSoloNames) {
            TurnSolo? turn = turnSoloList.Find(turn => turn.name == turnName);

            if (turn == null) {
                throw new InvalidDataException(
                    $"Не найдена одиночная стрелка '{turnName}' для секции '{filePath}'"
                );
            }

            sectionTurnSolo.Add(turn);
        }

        // Разделяем адрес и вход реальной занятости секции
        string[] realOccup = fields["addr_CR"].Split(',');

        // Имя секции из имени файла
        string name = Path.GetFileNameWithoutExtension(filePath);
        // Убираем префикс секции
        if (name.StartsWith("Sect_")) {
            name = name[5..];
        }

        // Создание секции
        Section section = new(
            // Имя секции
            name,
            // Переключатель секции
            createSwitch(Switch.Type.Virt_Toggle, int.Parse(fields["addr_SV"])),
            // Контакт реальной занятости секции
            createContact(
                Contact.Type.CANBus,
                int.Parse(realOccup[0]),
                int.Parse(realOccup[1])
            ),
            // Список маршрутов секции
            sectionRoutes,
            // Список одиночных стрелок секции
            sectionTurnSolo
        );
        sectionList.Add(section);

        // Переходим к следующему файлу
        loaderData.curIndex++;
        return true;
    }
}
