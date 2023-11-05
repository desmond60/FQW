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
    int min_count_step;     // Минимальное количество шагов по объекту

    bool IsStrictGrid  = false;   // Строгая ли сетка?

    List<string> layers_str  = new List<string>();   // Горизонтальные слои для ListBox
    List<string> items_str   = new List<string>();   // Объекты для  ListBox
    List<Layer> layers       = new List<Layer>();    // Горизонтальные слои
    List<Item> items         = new List<Item>();     // Объекты на сетке
    int[] SideBound;                                 // Номера краевых на сторонах
    Grid grid;                                       // Структура сетки

    /* ----------------------- Переменные --------------------------------- */

    //: Обновление компонент
    private void UpdateComponent()
    {
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
    private void WriteInterface(string path, Grid grid)
    {
        StringBuilder interface_str = new StringBuilder();

        interface_str.Append($"{grid.Begin_BG[0]} {grid.Begin_BG[1]} {grid.End_BG[0]} {grid.End_BG[1]}\n");
        interface_str.Append($"{grid.CountX} {grid.CountY}\n");
        interface_str.Append($"{grid.Kx} {grid.Ky}\n");
        interface_str.Append($"{grid.min_step} {grid.min_count_step}\n");
        interface_str.Append($"{grid.IsStrictGrid}\n");
        interface_str.Append($"{grid.SideBound[0]} {grid.SideBound[1]} {grid.SideBound[2]} {grid.SideBound[3]}\n");

        File.WriteAllText(path + @"/interface.txt", interface_str.ToString());
    }

    //: Загрузка интерфейса из grid
    private void LoadInterface(string path)
    {
        string[] interface_str = File.ReadAllLines(path + @"/interface.txt");

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
        layers.Clear();
        layers_str.Clear();
        for (int i = 0; i < grid.Layers.Count; i++) {
            layers_str.Add(grid.Layers[i].ToString());
            layers.Add(grid.Layers[i]);
        }
        layersList.Items.Refresh();

        // Первый слой
        string[] name = layers_str[0].Split(" ");
        TextLayer.Text = name[0];
        TextLayerSigma.Text = name[1];

        // Добавление объектов
        items.Clear();
        items_str.Clear();
        for (int i = 0; i < grid.Items.Count; i++) {
            items_str.Add(grid.Items[i].Name + " " + grid.Items[i].Sigma);
            items.Add(grid.Items[i]);
        }
        itemsList.Items.Refresh();

        // Первый объект
        Item item = items[0];
        Begin_SML_X.Text = item.Begin[0].ToString();
        Begin_SML_Y.Text = item.Begin[1].ToString();
        End_SML_X.Text = item.End[0].ToString();
        End_SML_Y.Text = item.End[1].ToString();
        N_X.Text = item.Nx.ToString();
        N_Y.Text = item.Ny.ToString();
        TextItemSigma.Text = item.Sigma.ToString();
        TextItem.Text = item.Name;
    }

    //: Рисование сетки
    private void DrawGrid()
    {
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
            if (ListContains(layers, dataY[0])) {
                var scatter = GridPlot.Plot.AddScatter(dataX, dataY, Color.DarkViolet);
                scatter.LineWidth = 2.5;
            }
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

        // Рисование линий в воздухе, которые будут исключены после составления СЛАУ
        
        /*GridPlot.Refresh(); // Нужно чтоб без Warninga было
        //GridPlot.Configuration.WarnIfRenderNotCalledManually = false; // или вот так можно отключить Warning
        double left_b = grid.Nodes[0].X;
        double right_b = grid.Nodes[^1].X;
        double exit_y = grid.Nodes[^1].Y;
        double temp = right_b;

        for (int i = 0; i < grid.Elems.Count; i++) {

            // Середина элемента
            Node node = new Node(
               (grid.Nodes[grid.Elems[i].Node[0]].X + grid.Nodes[grid.Elems[i].Node[1]].X) / 2.0,
               (grid.Nodes[grid.Elems[i].Node[0]].Y + grid.Nodes[grid.Elems[i].Node[2]].Y) / 2.0
            );

            if (node.Y < 0.0) continue;

            int num_up_edge = grid.Elems[i].Edge[3];
            Edge up_edge = grid.Edges[num_up_edge];

            dataX = new double[2] { up_edge.NodeBegin.X, up_edge.NodeEnd.X };
            dataY = new double[2] { up_edge.NodeBegin.Y, up_edge.NodeEnd.Y };

            if (Abs(up_edge.NodeEnd.Y - exit_y) <= 1e-10) return;

            if (Abs(up_edge.NodeEnd.X - right_b) <= 1e-10 || Abs(up_edge.NodeBegin.X - right_b) <= 1e-10) {
                if (Abs(temp - left_b) <= 1e-10) {
                    var scatter = GridPlot.Plot.AddScatter(dataX, dataY, Color.DarkRed);
                    scatter.LineWidth = 3;
                }
                temp = temp == left_b ? right_b : left_b;
                continue;
            }

            if (Abs(up_edge.NodeEnd.X - temp) <= 1e-10 || Abs(up_edge.NodeBegin.X - temp) <= 1e-10)
                continue;

            // Заносим ребро на график
            var scatter1 = GridPlot.Plot.AddScatter(dataX, dataY, Color.DarkRed);
            scatter1.LineWidth = 3;
        }*/
        
        // Настройки графика
        GridPlot.Plot.XAxis.Label("Ox");
        GridPlot.Plot.YAxis.Label("Oz");
        GridPlot.Plot.Title("Конечноэлементная сетка");

        GridPlot.Refresh();
    }
}

