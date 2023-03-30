namespace FEM;

// % ****** Логика взаимодействия с окошком MainWindow ***** % //
public partial class MainWindow : Window
{
    public MainWindow() {
        InitializeComponent();
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");   // Установка культуры
        itemsList.ItemsSource  = items_str;    // ListBox для объектов
        layersList.ItemsSource = layers_str;   // ListBox для слоев
    }

    /* ----------------------- Переменные --------------------------------- */
    Vector<double> Begin_BG  = new Vector<double>(2);   // Левая-Нижняя точка (Большого поля)
    Vector<double> End_BG    = new Vector<double>(2);   // Правая-Верхняя точка (Большого поля)

    int CountX, CountY;     // Количество узлов по Осям для графика
    double Kx, Ky;          // Коэффициент разрядки от объекта
    double min_step;        // Минимальный шаг по объекту
    int min_count_step;     // Минимальное количество шагов по оюъекту

    bool IsStrictGrid  = false;   // Строгая ли сетка?
    double e = 0.01;   // e = 0.01*l не должно быть узких мест в сетке

    List<string> layers_str  = new List<string>();   // Горизонтальные слои для ListBox
    List<string> items_str   = new List<string>();   // Объекты для  ListBox
    List<Layer> layers       = new List<Layer>();    // Горизонтальные слои
    List<Item> items         = new List<Item>();     // Объекты на сетке
    int[] SideBound;                                 // Номера краевых на сторонах
    Grid grid;                                       // Структура сетки

    /* ----------------------- Переменные --------------------------------- */

    //: Обновление компонент
    private void UpdateComponent() {
        Begin_BG[0]  = Double.Parse(Begin_BG_X.Text);
        Begin_BG[1]  = Double.Parse(Begin_BG_Y.Text);
        End_BG[0]    = Double.Parse(End_BG_X.Text);
        End_BG[1]    = Double.Parse(End_BG_Y.Text);

        layers = new List<Layer>();
        for (int i = 0; i < layers_str.Count; i++) {
            var layer = layers_str[i].Split(" ");
            layers.Add(new Layer(double.Parse(layer[0]), double.Parse(layer[1])));
        }
        layers = layers.OrderByDescending(n => n.Y).ToList();

        min_step = Double.Parse(Min_step_item.Text);
        min_count_step = Int32.Parse(Min_Count_step.Text);
        
        Kx = Double.Parse(K_X.Text);
        Ky = Double.Parse(K_Y.Text);


        int side0 = Int32.Parse(Side0Text.Text);
        int side1 = Int32.Parse(Side1Text.Text); ;
        int side2 = Int32.Parse(Side2Text.Text); ;
        int side3 = Int32.Parse(Side3Text.Text); ;
        SideBound = new int[] { side0, side1, side2, side3 };
    }

    //: Запись интерфейса
    private void WriteInterface() {
        StringBuilder interface_str = new StringBuilder();

        interface_str.Append($"{Begin_BG[0]} {Begin_BG[1]} {End_BG[0]} {End_BG[1]}\n");
        interface_str.Append($"{CountX} {CountY}\n");
        interface_str.Append($"{Kx} {Ky}\n");
        interface_str.Append($"{min_step} {min_count_step}\n");
        interface_str.Append($"{IsStrictGrid}\n");
        interface_str.Append($"{SideBound[0]} {SideBound[1]} {SideBound[2]} {SideBound[3]}\n");

        File.WriteAllText(@"grid/interface.txt", interface_str.ToString());
    }

    //: Загрузка интерфейса из grid
    private void LoadInterface() {

        string[] interface_str = File.ReadAllLines(@"grid/interface.txt");

        // Установка точек большого поля
        var line = interface_str[0].Trim().Split(" ");
        Begin_BG = new Vector<double>(new double[] { double.Parse(line[0]), double.Parse(line[1])});
        End_BG = new Vector<double>(new double[] { double.Parse(line[2]), double.Parse(line[3]) });
        Begin_BG_X.Text = Begin_BG[0].ToString();
        Begin_BG_Y.Text = Begin_BG[1].ToString();
        End_BG_X.Text = End_BG[0].ToString();
        End_BG_Y.Text = End_BG[1].ToString();

        // Установка линий для графика
        line = interface_str[1].Trim().Split(" ");
        CountX = int.Parse(line[0]);
        CountY = int.Parse(line[1]);

        // Установка коэффициентов разрядки
        line = interface_str[2].Trim().Split(" ");
        Kx = double.Parse(line[0]);
        Ky = double.Parse(line[1]);
        K_X.Text = Kx.ToString();
        K_Y.Text = Ky.ToString();

        // Установка минимальных значений
        line = interface_str[3].Trim().Split(" ");
        min_step = double.Parse(line[0]);
        min_count_step = int.Parse(line[1]);
        Min_step_item.Text = min_step.ToString();
        Min_Count_step.Text = min_count_step.ToString();

        // Установка строгости сетки
        line = interface_str[4].Trim().Split(" ");
        IsStrictGrid = bool.Parse(line[0]);
        ToggleStrict.IsChecked = IsStrictGrid;

        // Установка краевых значений
        line = interface_str[5].Trim().Split(" ");
        SideBound = new int[] { int.Parse(line[0]), int.Parse(line[1]), int.Parse(line[2]), int.Parse(line[3]) };
        Side0Text.Text = SideBound[0].ToString();
        Side1Text.Text = SideBound[1].ToString();
        Side2Text.Text = SideBound[2].ToString();
        Side3Text.Text = SideBound[3].ToString();

        // Добавление слоев
        for (int i = 0; i < grid.Layers.Count; i++) {
            layers_str.Add(grid.Layers[i].ToString());
            layers.Add(grid.Layers[i]);
        }
        layersList.Items.Refresh();

        // Добавление объектов
        for (int i = 0; i < grid.Items.Count; i++) {
            items_str.Add(grid.Items[i].Name + " " + grid.Items[i].Sigma);
            items.Add(grid.Items[i]);
        }
        itemsList.Items.Refresh();
    }

    //: Построение сетки
    private void CreateGrid() {

        // Генерация узлов
        List<Node> nodes = generate_node();

        // Генерация элементов
        List<Elem> elems = generate_elem();

        // Генерация ребер
        List<Edge> edges = generate_edge(elems, nodes);

        // Генерация краевых
        List<Bound> bounds = generate_bound(edges);

        // Генерация проводимости
        List<(double, double)> sigmas = generate_sigma();

        grid =  new Grid(nodes, edges, elems, bounds, items, layers, sigmas);
    }

    //: Рисование сетки
    private void DrawGrid() {

        // Очищаем график
        GridPlot.Plot.Clear();

        // Рисование линий по Оси X
        double[] dataX = new double[CountX];
        double[] dataY = new double[CountX];
        for (int i = 0; i < CountY; i++) {
            dataX = new double[CountX];
            dataY = new double[CountX];
            for (int j = 0; j < CountX; j++) {
                dataX[j] = grid.Nodes[i * CountX + j].X;
                dataY[j] = grid.Nodes[i * CountX + j].Y;
            }
            if (ListContains(layers, dataY[0]))
                GridPlot.Plot.AddScatter(dataX, dataY, Color.OrangeRed);
            else
                GridPlot.Plot.AddScatter(dataX, dataY, Color.Blue);
        }

        // Рисование линий по Оси Y
        for (int i = 0; i < CountX; i++) {
            dataX = new double[CountY];
            dataY = new double[CountY];
            for (int j = 0; j < CountY; j++) {
                dataX[j] = grid.Nodes[j * CountX + i].X;
                dataY[j] = grid.Nodes[j * CountX + i].Y;
            }
            var scatter = GridPlot.Plot.AddScatter(dataX, dataY, Color.Blue);
            scatter.MarkerColor = Color.DarkBlue;
        }

        // Рисование объектов
        foreach (var item in items) {

            // Нижняя линия
            dataX = new double[2] { item.Begin[0], item.End[0] };
            dataY = new double[2] { item.Begin[1], item.Begin[1] };
            var scatter = GridPlot.Plot.AddScatter(dataX, dataY, Color.DarkGreen);
            scatter.LineWidth = 5;
            scatter.MarkerColor = Color.DarkRed;

            // Правая линия
            dataX = new double[2] { item.End[0], item.End[0] };
            dataY = new double[2] { item.Begin[1], item.End[1] };
            scatter = GridPlot.Plot.AddScatter(dataX, dataY, Color.DarkGreen);
            scatter.LineWidth = 5;
            scatter.MarkerColor = Color.DarkRed;

            // Верхняя линия
            dataX = new double[2] { item.Begin[0], item.End[0] };
            dataY = new double[2] { item.End[1], item.End[1] };
            scatter = GridPlot.Plot.AddScatter(dataX, dataY, Color.DarkGreen);
            scatter.LineWidth = 5;
            scatter.MarkerColor = Color.DarkRed;

            // Левая линия
            dataX = new double[2] { item.Begin[0], item.Begin[0] };
            dataY = new double[2] { item.Begin[1], item.End[1] };
            scatter = GridPlot.Plot.AddScatter(dataX, dataY, Color.DarkGreen);
            scatter.LineWidth = 5;
            scatter.MarkerColor = Color.DarkRed;
        }

        // Настройки графика
        GridPlot.Plot.XAxis.Label("Ox");
        GridPlot.Plot.YAxis.Label("Oy");
        GridPlot.Plot.Title("Конечноэлементная сетка");

        GridPlot.Refresh();
    }
}

