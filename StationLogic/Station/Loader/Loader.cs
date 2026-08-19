using static ConsoleLog;

public partial class Station {
    // Данные для загрузки
    private LoaderData loaderData = new LoaderData("sta2");

    // Инициализация загрузчика базы данных
    private void initLoader(string staPathData) {
        loaderData = new LoaderData(staPathData);

        // Загрузка одиночных стрелок
        loaderData.setDir("TurnSolo");
        while(loadTurnSolo());
        // Загрузка комплексных стрелок
        loaderData.setDir("TurnPair");
        while(loadTurnPair());
        // Загрузка Points: мосты и светофоры
        loaderData.setDir("Points");
        while(loadTrafficLight_Bridge());
        // Загрузка связей между светофорами после создания всех Points
        loadPrevTrafficLightLinks();
        // Загрузка маршрутов
        loaderData.setDir("Routes");
        while(loadRoute());
        // Загрузка секций
        loaderData.setDir("Sections");
        while(loadSection());
        // Загрузка разъединителей
        loaderData.setDir("Separate");
        while(loadSeparate());
        // Загрузка кнопок управления маршрутами
        loaderData.setDir("RouteControl");
        while(loadRouteButton());

        // Сортировка списков
        switchList.Sort((sw1, sw2) => sw1.addr.CompareTo(sw2.addr));
        contactList.Sort((c1, c2) => c1.addr.CompareTo(c2.addr));
    }

    // Данные для загрузки
    private class LoaderData {
        public string dir = ""; // директория с файлами
        private string staPathData = ""; // путь к директории с данными
        public List<string> files = new List<string>(); // список файлов
        public int curIndex = 0; // текущий индекс
        public int sum = 0; // количество файлов

        public LoaderData(string staPathData) {
            this.staPathData = staPathData;
        }

        public void setDir(string subDir) {
            this.dir = Path.Combine("DataBase", staPathData, subDir);
            files = Directory
                .GetFiles(this.dir, "*.md")
                .OrderBy(file => file, StringComparer.Ordinal)
                .ToList();
            sum = files.Count;
            curIndex = 0;
        }
    }
}
