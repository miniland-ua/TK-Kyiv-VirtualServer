public partial class Station {
    // Загрузка маршрута
    public bool loadRoute() {
        // Проверяем окончание списка файлов
        if (loaderData.sum <= loaderData.curIndex) {
            return false;
        }

        // Путь читаемого файла
        string filePath = loaderData.files[loaderData.curIndex];

        // Шаблон параметра <name, value>
        Dictionary<string, string> fields = new();
        // Список имен мостов маршрута
        List<string> bridgeNames = new();
        // Чтение всех строк файла
        string[] lines = File.ReadAllLines(filePath);

        // Читаем параметры из файла
        for (int i = 1; i < lines.Length; i++) {
            string line = lines[i].Trim();

            // Окончание блока параметров - выход из цикла
            if (line == "---") {
                break;
            }

            // Читаем мосты из списка Points
            if (line.StartsWith("-")) {
                string bridgeName = line
                    .TrimStart('-')
                    .Trim()
                    .Trim('"')
                    .Replace("[[", "")
                    .Replace("]]", "");

                // Убираем путь к файлу
                bridgeName = bridgeName.Split('/').Last();
                // Убираем префикс точки
                if (bridgeName.StartsWith("P_")) {
                    bridgeName = bridgeName[2..];
                }

                bridgeNames.Add(bridgeName);
                continue;
            }

            // Разделяем строку на 2 части
            string[] parts = line.Split(':', 2);
            if (parts.Length == 2 && parts[1].Trim() != "") {
                // Имя параметра
                string key = parts[0].Trim();
                // Значение параметра (кавычки убираем если они есть)
                string value = parts[1].Trim().Trim('"');
                fields[key] = value;
            }
        }

        // Находим мосты маршрута по именам
        List<Bridge> routeBridges = new();
        foreach (string bridgeName in bridgeNames) {
            Bridge? bridge = bridgeList.Find(bridge => bridge.name == bridgeName);

            if (bridge == null) {
                throw new InvalidDataException(
                    $"Не найден мост '{bridgeName}' для маршрута '{filePath}'"
                );
            }

            routeBridges.Add(bridge);
        }

        // Находим парные стрелки маршрута по именам
        List<(TurnPair, bool)> routeTurns = new();
        for (int i = 1; i <= 2; i++) {
            string turnKey = "TurnPair" + i;
            string stateKey = "stateTurn" + i;

            // Если стрелка не указана - переходим к следующей
            if (!fields.TryGetValue(turnKey, out string? turnName)) {
                continue;
            }

            // Убираем скобки ссылки и путь к файлу
            turnName = turnName
                .Replace("[[", "")
                .Replace("]]", "")
                .Split('/')
                .Last();

            TurnPair? turnPair = turnPairList.Find(turn => turn.name == turnName);
            if (turnPair == null) {
                throw new InvalidDataException(
                    $"Не найдена парная стрелка '{turnName}' для маршрута '{filePath}'"
                );
            }

            // Добавляем ссылку на стрелку и требуемое состояние
            routeTurns.Add((turnPair, bool.Parse(fields[stateKey])));
        }

        // Имя маршрута из имени файла
        string name = Path.GetFileNameWithoutExtension(filePath);
        // Убираем префикс маршрута
        if (name.StartsWith("R_")) {
            name = name[2..];
        }

        // Создание маршрута
        Route route = new(
            // Имя маршрута
            name,
            // Переключатель маршрута
            createSwitch(Switch.Type.Virt_Toggle, int.Parse(fields["addr_SV"])),
            // Список парных стрелок маршрута
            routeTurns,
            // Левый мост маршрута
            routeBridges[0],
            // Правый мост маршрута
            routeBridges[1]
        );
        routeList.Add(route);

        // Переходим к следующему файлу
        loaderData.curIndex++;
        return true;
    }
}