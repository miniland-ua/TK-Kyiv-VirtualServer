public partial class Station {
    // Загрузка одиночной стрелки
    public bool loadTurnSolo() {
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
                // Значение параметра (кавычки убираем если он есть)
                string value = parts[1].Trim().Trim('"');
                fields[key] = value;
            }
        }

        // Разделяем индексы контактов
        string[] indC = fields["indC_CV"].Split(',');
        string[] indT = fields["indT_CV"].Split(',');

        TurnSolo turnSolo = new(
            // Имя стрелки
            Path.GetFileNameWithoutExtension(filePath),
            // Реальный свитч стрелки
            createSwitch(Switch.Type.Real, int.Parse(fields["addr_SR"])),
            // Индикатор стрелки положения C (Contact)
            createContact(Contact.Type.CANBus, int.Parse(indC[0]), int.Parse(indC[1])),
            // Индикатор стрелки положения T (Contact)
            createContact(Contact.Type.CANBus, int.Parse(indT[0]), int.Parse(indT[1]))
        );
        turnSoloList.Add(turnSolo);
    
        loaderData.curIndex++;
        return true;
    }
}