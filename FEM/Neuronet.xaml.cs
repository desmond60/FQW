using FEM.grid;

namespace FEM;

// % ****** Логика взаимодействия с окошком Neuronet ***** % //
public partial class Neuronet : Window
{
    public Neuronet()
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");   // Установка культуры
        InitializeComponent();
        grid.LoadGrid(@"grid");
        LoadInterface(@"grid");
        receiversList.ItemsSource = receivers_str;   // ListBox для приемников

        // Чтение приемников
        string[] FReceiver = File.ReadAllLines(@"grid/receivers.txt");
        for (int i = 0; i < FReceiver.Length; i++)
            receivers_str.Add(FReceiver[i]);
        receiversList.Items.Refresh();
    }

    /* ----------------------- Переменные --------------------------------- */
    Grid grid = new Grid();   // Структура сетки
    string pathData = @"synthetic";


    List<string> receivers_str = new List<string>();   // Лист с приемниками для ListBox
    /* ----------------------- Переменные --------------------------------- */


    //: Генерация синтетических данных
    private (Grid, string, string) GenerateSynthetic(Grid grid)
    {

        StringBuilder outputSB = new StringBuilder();

        // Рандомное расположение объекта по среде
        (List<Item> newItems, StringBuilder inputSB) = RandomLocatedItem(grid);

        // % ***** Строим сетку ***** % //
        Generate generate = new Generate(newItems, grid.Layers, grid.IsStrictGrid);
        generate.TrySetParameter("Kx", grid.Kx);
        generate.TrySetParameter("Ky", grid.Ky);
        generate.TrySetParameter("min_step", grid.min_step);
        generate.TrySetParameter("min_count_step", grid.min_count_step);
        generate.TrySetParameter("Begin_BG", grid.Begin_BG);
        generate.TrySetParameter("End_BG", grid.End_BG);
        generate.TrySetParameter("SideBound", grid.SideBound);
        Grid newGrid = generate.CreateGrid();

        // % ***** Получаем решение ***** % //

        // Коэффициент nu
        var nu = double.Parse(NuBox.Text);

        // Составление одномерной задачи и ее решение
        Harm1D harm1d = new Harm1D();
        harm1d.WriteFileHarm1D(newGrid.Layers, nu);
        harm1d.RunHarm1D();
        harm1d.ReadMeshAndSolve();

        // Составление СЛАУ
        FEM fem = new FEM(newGrid, harm1d);
        fem.TrySetParameter("Nu", nu);
        SLAU slau = fem.CreateSLAU();

        // Запись СЛАУ
        slau.WriteTXT(@"slau/slauTXT");
        slau.WriteBIN(@"slau/slauBIN");

        // Запуск PARDISO
        string command = "cd slau & Intel.exe & cd ..";
        Process process = Process.Start("cmd.exe", "/C " + command);
        process.WaitForExit();

        // Прочитать решение
        string[] Fq = File.ReadAllLines(@"slau/slauBIN/q.txt");
        for (int i = 0; i < slau.N; i++) {
            var q = Fq[i].Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);
            slau.q[i] = new Complex(double.Parse(q[0]), double.Parse(q[1]));
        }

        // Получение решения в приемниках
        outputSB = OutputReceiver(slau, harm1d);

        return (newGrid, inputSB.ToString(), outputSB.ToString());
    }

    //: Новая локация объекта
    private (List<Item>, StringBuilder) RandomLocatedItem(Grid grid)
    {
        // Ширина и высота объекта
        double width = grid.Items[0].End[0] - grid.Items[0].Begin[0];
        double height = grid.Items[0].End[1] - grid.Items[0].Begin[1];

        // Границы центра
        double bottom = grid.Begin_BG[1] - height;
        double right = grid.End_BG[0] - width;
        double top = 0.0;
        double left = grid.Begin_BG[0] - width;

        // Новая точчки объекта
        Vector<double> begin = new Vector<double>(2);
        Vector<double> end = new Vector<double>(2);

        double centerX = left + rnd.NextDouble() * (right - left);
        double centerY = bottom + rnd.NextDouble() * (top - bottom);

        begin[0] = centerX - width / 2.0;
        begin[1] = centerY - height / 2.0;
        end[0] = centerX + width / 2.0;
        end[1] = centerY + height / 2.0;

        return (new List<Item>() { grid.Items[0] with { Begin = (Vector<double>)begin.Clone(), End = (Vector<double>)end.Clone() } },
            new StringBuilder($"{begin[0]} {begin[1]} \n {end[0]} {end[1]}")
        );
    }

    //: Получить решение в приемниках
    private StringBuilder OutputReceiver(SLAU slau, Harm1D harm1d)
    {
        var receivers = new Vector<double>(receivers_str.Select(double.Parse).OrderByDescending(n => n).ToArray());
        var nu = double.Parse(NuBox.Text);
        var w = 2.0 * PI * nu;

        List<(int id, Node node)> surface = new List<(int, Node)>();
        for (int i = 0; i < grid.Edges.Count; i++) {
            if (Abs(grid.Edges[i].NodeBegin.Y) <= 1e-10 && Abs(grid.Edges[i].NodeEnd.Y) <= 1e-10) {
                Node node = new Node((grid.Edges[i].NodeBegin.X + grid.Edges[i].NodeEnd.X) / 2.0, grid.Edges[i].NodeBegin.Y);
                surface.Add((i, node));
            }
        }

        // ************************************* Для основной задачи ***************************** //
        StringBuilder table = new StringBuilder(String.Format("{0,-5} | {1,-15} | {2,-15} | {3,-30} | {4,0} \n", "Приемник", "Re Ex", "Im Ex", "Hy", "Rk"));

        var list = receivers.ToList();
        list.Reverse();
        receivers = list.ToArray();
        for (int i = 0; i < receivers.Length; i++)
        {

            // Находим узлы
            int id1 = 0, id2 = 0;
            for (int j = 0; j < surface.Count - 1; j++)
            {
                if (receivers[i] >= surface[j].Item2.X && receivers[i] <= surface[j + 1].Item2.X)
                {
                    id1 = j;
                    id2 = j + 1;
                }
            }

            // Находим коэффициенты прямой
            Complex k = (slau.q[surface[id2].id] - slau.q[surface[id1].id]) / (surface[id2].node.X - surface[id1].node.X);
            Complex b = slau.q[surface[id2].id] - k * surface[id2].node.X;

            // Компонента электрического поля
            var A = k * receivers[i] + b + harm1d.U[^1];
            var E = (-1) * new Complex(0, 1) * w * A;
            var E1D = (-1) * new Complex(0, 1) * w * harm1d.U[^1];
            var E_norm = E / E1D;

            // Находим в какой элемент попадает приемник
            var node = new Node(receivers[i], 0);
            int id_elem = 0;
            for (int j = 0; j < grid.Elems.Count; j++)
            {
                if ((node.X >= grid.Nodes[grid.Elems[j].Node[0]].X && node.X < grid.Nodes[grid.Elems[j].Node[3]].X) &&
                    (node.Y > grid.Nodes[grid.Elems[j].Node[0]].Y && node.Y <= grid.Nodes[grid.Elems[j].Node[3]].Y))
                    id_elem = j;
            }
            var elem = grid.Elems[id_elem];

            // Компоненты hx и hy
            double hx = grid.Nodes[elem.Node[3]].X - grid.Nodes[elem.Node[0]].X;
            double hy = grid.Nodes[elem.Node[3]].Y - grid.Nodes[elem.Node[0]].Y;

            // Производная базисных функций
            var phi1 = -1 / hx;
            var phi2 = 1 / hx;
            var phi3 = 1 / hy;
            var phi4 = -1 / hy;

            // Сумма
            var Sum = phi1 * slau.q[elem.Edge[0]] + phi2 * slau.q[elem.Edge[1]] + phi3 * slau.q[elem.Edge[2]] + phi4 * slau.q[elem.Edge[3]];

            // Компонента магнитного поля
            var H1D = (harm1d.U[^1] - harm1d.U[^2]) / (harm1d.Nodes[^1].Y - harm1d.Nodes[^2].Y);
            var H = (Sum + H1D) / Nu0;
            var H_norm = H / H1D;

            // Компонента Z в случае H-поляризации ()
            var Z = E / H;

            // Кажущиеся сопротивления
            var R = Pow(Norm(Z), 2) / (w * Nu0);

            string str = String.Format("{0,-8} | {1,-15} | {2,-15} | {3,-30} | {4,0} \n",
                        $"{receivers[i]}", $"{E_norm.Real.ToString("E5")}", $"{E_norm.Imaginary.ToString("E5")}",
                        $"({H_norm.Real.ToString("E5")}, {H_norm.Imaginary.ToString("E5")})", $"{R.ToString("E5")}");
            table.Append(str.Replace(".", ","));
        }
        // ********************************************************************************************** //

        return table;
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
        grid.Begin_BG = new Vector<double>(new double[] { double.Parse(line[0]), double.Parse(line[1]) });
        grid.End_BG = new Vector<double>(new double[] { double.Parse(line[2]), double.Parse(line[3]) });

        // Установка линий для графика
        line = interface_str[1].Trim().Split(" ");
        grid.CountX = int.Parse(line[0]);
        grid.CountY = int.Parse(line[1]);

        // Установка коэффициентов разрядки
        line = interface_str[2].Trim().Split(" ");
        grid.Kx = double.Parse(line[0]);
        grid.Ky = double.Parse(line[1]);

        // Установка минимальных значений
        line = interface_str[3].Trim().Split(" ");
        grid.min_step = double.Parse(line[0]);
        grid.min_count_step = int.Parse(line[1]);

        // Установка строгости сетки
        line = interface_str[4].Trim().Split(" ");
        grid.IsStrictGrid = bool.Parse(line[0]);

        // Установка краевых значений
        line = interface_str[5].Trim().Split(" ");
        grid.SideBound = new int[] { int.Parse(line[0]), int.Parse(line[1]), int.Parse(line[2]), int.Parse(line[3]) };
    }
}
