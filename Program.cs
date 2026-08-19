class Program {
    [STAThread]
    private static void Main() {
        ApplicationConfiguration.Initialize(); // Настройка конфигурации приложения
        var mainForm = new MainForm(); // Создание главной формы приложения

        // Подписка на события печати в консоль
        ConsoleLog.Printed += mainForm.print;
        
        Application.Run(mainForm); // Запуск главной формы приложения
    }
}
