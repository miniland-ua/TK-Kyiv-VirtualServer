public partial class Station {
    // Загрузка разъединителя
    public bool loadSeparate() {
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

        // Имя секции из ссылки
        string sectionName = fields["Section"]
            .Replace("[[", "")
            .Replace("]]", "")
            .Split('/')
            .Last();

        // Убираем префикс секции
        if (sectionName.StartsWith("Sect_")) {
            sectionName = sectionName[5..];
        }

        // Находим существующую секцию по имени
        Section? section = sectionList.Find(section => section.name == sectionName);
        if (section == null) {
            throw new InvalidDataException(
                $"Не найдена секция '{sectionName}' для разъединителя '{filePath}'"
            );
        }

        // Разделяем адрес и вход индикатора разъединителя
        string[] ind = fields["addr_CV"].Split(',');

        // Имя разъединителя из имени файла
        string name = Path.GetFileNameWithoutExtension(filePath);
        // Убираем префикс разъединителя
        if (name.StartsWith("Sep_")) {
            name = name[4..];
        }

        // Создание разъединителя
        Separate separate = new(
            // Имя разъединителя
            name,
            // Переключатель разъединителя
            createSwitch(Switch.Type.Virt_Toggle, int.Parse(fields["addr_SV"])),
            // Индикатор разъединителя
            createContact(Contact.Type.CANBus, int.Parse(ind[0]), int.Parse(ind[1])),
            // Секция разъединителя
            section
        );
        separateList.Add(separate);

        // Переходим к следующему файлу
        loaderData.curIndex++;
        return true;
    }
}