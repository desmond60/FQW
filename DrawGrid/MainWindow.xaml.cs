namespace DrawGrid;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        InitializeComponent();
        layersList.ItemsSource = layers_str;
        itemsList.ItemsSource = items_str;
    }

    /* ----------------------- Переменные --------------------------------- */
    Vector<double> Begin_BG  = new Vector<double>(2);   // Левая-Нижняя точка (Большого поля)
    Vector<double> End_BG    = new Vector<double>(2);   // Правая-Верхняя точка (Большого поля)

    int Nx, Ny;             // Количество узлов по объекту
    int CountX, CountY;     // Количество узлов по Осям
    double Kx, Ky;          // Коэффициент разрядки пот объекта
    double min_step;        // Минимальный шаг по объекту
    int min_count_step;     // Минимальное количество шагов по оюъекту

    bool IsStrictGrid  = false;   // Строгая ли сетка?
    double e = 0.01;   // e = 0.01*l не должно быть узких мест в сетке

    List<string> layers_str  = new List<string>();         // Горизонтальные слои для ListBox
    List<string> items_str   = new List<string>();         // Объекты для  ListBox
    List<double> layers      = new List<double>();         // Горизонтальные слои
    List<Item> items         = new List<Item>();           // Объекты на сетке
    int[]? SideBound;                                      // Номера краевых на сторонах
    Grid<double> grid;                                     // Структура сетки

    /* ----------------------- Переменные --------------------------------- */

    //: Обновление компонент
    private void UpdateComponent() {
        Begin_BG[0]  = Double.Parse(Begin_BG_X.Text);
        Begin_BG[1]  = Double.Parse(Begin_BG_Y.Text);
        End_BG[0]    = Double.Parse(End_BG_X.Text);
        End_BG[1]    = Double.Parse(End_BG_Y.Text);

        layers = new List<double>(layers_str.Select(n => double.Parse(n)).OrderByDescending(n => n));

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

    //: Построение сетки
    private void CreateGrid() {

        // Генерация узлов
        Node<double>[] nodes = generate_node();

        // Генерация элементов
        Elem[] elems = generate_elem();

        // Генерация ребер
        Edge<double>[] edges = generate_edge(elems, nodes);

        // Генерация краевых
        Bound[] bounds = generate_bound(edges);

        grid =  new Grid<double>(nodes, edges, elems, bounds);
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
                GridPlot.Plot.AddScatter(dataX, dataY, System.Drawing.Color.OrangeRed);
            else
                GridPlot.Plot.AddScatter(dataX, dataY, System.Drawing.Color.Blue);
        }

        // Рисование линий по Оси Y
        for (int i = 0; i < CountX; i++) {
            dataX = new double[CountY];
            dataY = new double[CountY];
            for (int j = 0; j < CountY; j++) {
                dataX[j] = grid.Nodes[j * CountX + i].X;
                dataY[j] = grid.Nodes[j * CountX + i].Y;
            }
            var scatter = GridPlot.Plot.AddScatter(dataX, dataY, System.Drawing.Color.Blue);
            scatter.MarkerColor = Color.DarkBlue;
        }

        // Настройки графика
        GridPlot.Plot.XAxis.Label("Ox");
        GridPlot.Plot.YAxis.Label("Oy");
        GridPlot.Plot.Title("Конечноэлементная сетка");

        GridPlot.Refresh();
        GridPlot.Plot.SaveFig(@"grid.png");
    }
}

