public partial class Station {
    // Связи светофоров, которые будут установлены после загрузки всех Points
    private readonly Dictionary<TrafficLight, string> prevTrafficLightLinks = new();

    // Загрузка светофора + моста
    public bool loadTrafficLight_Bridge() {
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
            if (parts.Length == 2) {
                // Имя параметра
                string key = parts[0].Trim();
                // Значение параметра (кавычки убираем если они есть)
                string value = parts[1].Trim().Trim('"');
                fields[key] = value;
            }
        }

        // Определение имени
        string name = Path.GetFileNameWithoutExtension(filePath);
        // Убираем префикс
        if (name.StartsWith("P_")) {
            name = name[2..];
        }

        // Светофор (если он существует)
        TrafficLight? trafficLight = null;

        // Если адрес светофора заполнен - создаем светофор
        if (fields.TryGetValue("addr_base", out string? addrBase)
        && !string.IsNullOrWhiteSpace(addrBase)) {
            // Определение типа светофора по имени
            string typeName = fields["type"]
                .Replace("[[", "")
                .Replace("]]", "");
            TrafficLight.Type type;
            switch (typeName) {
                case "Передвхідний ПАБ":
                    type = TrafficLight.Type.PreInPAB;
                    break;
                case "Вихідний ПАБ":
                    type = TrafficLight.Type.OutPAB;
                    break;
                case "Прохідний АБ":
                    type = TrafficLight.Type.PassAB;
                    break;
                case "Передвхідний АБ":
                    type = TrafficLight.Type.PreInAB;
                    break;
                case "Вихідний АБ":
                    type = TrafficLight.Type.OutAB;
                    break;
                case "Вхідний":
                    type = TrafficLight.Type.In;
                    break;
                case "Вхідний додатковий":
                    type = TrafficLight.Type.InAdd;
                    break;
                case "Маневровий":
                    type = TrafficLight.Type.Shunting;
                    break;
                default:
                    throw new InvalidDataException(
                        $"Неизвестный тип светофора '{typeName}' в файле '{filePath}'"
                    );
            }

            // Разделяем адрес и вход индикатора светофора
            string[] ind = fields["addrInd_CV"].Split(',');

            // Создание светофора
            trafficLight = new TrafficLight(
                // Имя светофора
                name,
                // Базовый адрес свитчей светофора
                int.Parse(addrBase),
                // Тип светофора
                type,
                // Индикатор светофора
                createContact(Contact.Type.CANBus, int.Parse(ind[0]), int.Parse(ind[1])),
                // Направление светофора
                Enum.Parse<Direct>(fields["direct"], true),
                switchList
            );
            tlList.Add(trafficLight);

            // Сохраняем имя предыдущего светофора для последующей загрузки связи
            if (fields.TryGetValue("prevPoint", out string? prevPoint)
            && !string.IsNullOrWhiteSpace(prevPoint)) {
                string prevTrafficLightName = prevPoint
                    .Replace("[[", "")
                    .Replace("]]", "")
                    .Split('/')
                    .Last();

                if (prevTrafficLightName.StartsWith("P_")) {
                    prevTrafficLightName = prevTrafficLightName[2..];
                }

                prevTrafficLightLinks[trafficLight] = prevTrafficLightName;
            }
        }

        // Создание моста со ссылкой на светофор или null
        Bridge bridge = new(name, trafficLight);
        bridgeList.Add(bridge);

        // Переходим к следующему файлу
        loaderData.curIndex++;
        return true;
    }

    // Загрузка связей с предыдущими светофорами
    private void loadPrevTrafficLightLinks() {
        foreach (KeyValuePair<TrafficLight, string> link in prevTrafficLightLinks) {
            TrafficLight? prevTrafficLight = atTL(link.Value);
            if (prevTrafficLight == null) {
                throw new InvalidDataException(
                    $"Не найден предыдущий светофор '{link.Value}' для светофора '{link.Key.name}'"
                );
            }

            link.Key.prevTL = prevTrafficLight;
        }
    }
}
