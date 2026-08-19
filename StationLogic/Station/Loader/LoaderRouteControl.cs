public partial class Station {
    // Загрузка кнопки управления маршрутом
    public bool loadRouteButton() {
        // Проверяем окончание списка файлов
        if (loaderData.sum <= loaderData.curIndex) {
            return false;
        }

        // Путь читаемого файла
        string filePath = loaderData.files[loaderData.curIndex];

        // Шаблон параметра <name, value>
        Dictionary<string, string> fields = new();
        // Чтение всех строк файла
        string[] lines = File.ReadAllLines(filePath);

        // Читаем параметры из файла
        for (int i = 1; i < lines.Length; i++) {
            string line = lines[i].Trim();

            // Окончание блока параметров - выход из цикла
            if (line == "---") {
                break;
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

        // Определение типа кнопки маршрута
        RouteButton.Type type;
        switch (fields["type"]) {
            case "train":
                type = RouteButton.Type.Train;
                break;
            case "shunt":
                type = RouteButton.Type.Shunt;
                break;
            case "end":
                type = RouteButton.Type.End;
                break;
            default:
                throw new InvalidDataException(
                    $"Неизвестный тип кнопки '{fields["type"]}' в файле '{filePath}'"
                );
        }

        // Имя кнопки из имени файла
        string name = Path.GetFileNameWithoutExtension(filePath);
        // Убираем тип кнопки из имени
        string typeSuffix = "_" + fields["type"];
        if (name.EndsWith(typeSuffix)) {
            name = name[..^typeSuffix.Length];
        }

        // Находим мост, к которому относится кнопка маршрута.
        if (!fields.TryGetValue("Bridge", out string? bridgeName)) {
            throw new InvalidDataException(
                $"Не указан мост для кнопки маршрута в файле '{filePath}'"
            );
        }

        // Убираем оформление Obsidian-ссылки, путь и префикс файла точки.
        bridgeName = bridgeName
            .Replace("[[", "")
            .Replace("]]", "")
            .Split('/')
            .Last();
        if (bridgeName.StartsWith("P_")) {
            bridgeName = bridgeName[2..];
        }

        Bridge? bridge = atBridge(bridgeName);
        if (bridge == null) {
            throw new InvalidDataException(
                $"Не найден мост '{bridgeName}' для кнопки маршрута в файле '{filePath}'"
            );
        }

        // Создание кнопки управления маршрутом
        RouteButton routeButton = new(
            // Имя кнопки
            name,
            // Тип кнопки
            type,
            // Переключатель кнопки
            createSwitch(Switch.Type.Virt_Push, int.Parse(fields["addr_SV"])),
            // Мост кнопки маршрута
            bridge
        );
        routeButtonList.Add(routeButton);

        // Переходим к следующему файлу
        loaderData.curIndex++;
        return true;
    }
}
