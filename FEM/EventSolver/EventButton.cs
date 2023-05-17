namespace FEM;

//: Обработчики Button
public partial class Solver
{

    //: Обработка кнопки "Построить СЛАУ"
    private void CreateSLAU_Click(object sender, RoutedEventArgs e) {

        var nu = double.Parse(NuBox.Text);

        // Составление одномерной задачи и ее решение
        harm1d = new Harm1D();
        harm1d.WriteFileHarm1D(grid.Layers, nu);
        harm1d.RunHarm1D();
        harm1d.ReadMeshAndSolve();

        // Составление СЛАУ
        FEM fem = new FEM(grid, harm1d);
        fem.TrySetParameter("Nu", nu * Nu0);
        slau = fem.CreateSLAU();

        // Запись СЛАУ
        slau.WriteTXT();
        slau.WriteBIN();
    }

    //: Обработка кнопки "Решить СЛАУ"
    private void SolveSLAU_Click(object sender, RoutedEventArgs e) {

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
    }

    //: Обработка кнопки "Выдать значения в приемниках"
    private void TableReceiver_Click(object sender, RoutedEventArgs e) {

        var receivers = new Vector<double>(receivers_str.Select(n => double.Parse(n)).OrderByDescending(n => n).ToArray());
        var nu = double.Parse(NuBox.Text);
        var w = 2.0 * PI * nu;
        TextBoxSolver.Document.Blocks.Clear();
        lEx.Clear();
        lRk.Clear();

        List<(int id, Node node)> surface = new List<(int, Node)>();
        for (int i = 0; i < grid.Edges.Count; i++) {
            if (Abs(grid.Edges[i].NodeBegin.Y) <= 1e-10 && Abs(grid.Edges[i].NodeEnd.Y) <= 1e-10) {
                Node node = new Node((grid.Edges[i].NodeBegin.X + grid.Edges[i].NodeEnd.X) / 2.0, grid.Edges[i].NodeBegin.Y);
                surface.Add((i, node));
            }
        }

        // ************************************* Для полинома ***************************** //

/*        StringBuilder table = new StringBuilder(String.Format("{0,-8} | {1,-30} | {2,-30} | {3,0} \n", "Приемник", "A*", "A", "|A* - A|"));

        var ABS = new ComplexVector(surface.Count);
        var SUB = new ComplexVector(surface.Count);
        for (int i = 0; i < surface.Count; i++)
        {

            Complex q = slau.q[surface[i].id];

            Complex A = Absolut(grid.Edges[surface[i].id]);
            Complex SubA = A - q;
            ABS[i] = A;
            SUB[i] = SubA;
            SubA = new Complex(Abs(SubA.Real), Abs(SubA.Imaginary));

            string str = String.Format("{0,-8} | {1,-30} | {2,-30} | {3,0} \n",
                                        $"{surface[i].node.X.ToString("F1")}", $"({q.Real.ToString("E3")}, {q.Imaginary.ToString("E3")})",
                                        $"({A.Real.ToString("E3")}, {A.Imaginary.ToString("E3")})", $"({SubA.Real.ToString("E3")}, {SubA.Imaginary.ToString("E3")})");
            table.Append(str.Replace(".", ","));
        }
        table.Append((Helper.Norm(SUB) / Helper.Norm(ABS)).ToString().Replace(".", ","));
        TextBoxSolver.Document.Blocks.Add(new Paragraph(new Run(table.ToString())));*/

        // ********************************************************************************************** //

        // ************************************* Для основной задачи ***************************** //

        StringBuilder table = new StringBuilder(String.Format("{0,-5} | {1,-15} | {2,-15} | {3,-30} | {4,0} \n", "Приемник", "Re Ex", "Im Ex", "Hy", "Rk"));

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

            // Добавление в листы
            lEx.Add(E_norm);
            lRk.Add(R);

            string str = String.Format("{0,-8} | {1,-15} | {2,-15} | {3,-30} | {4,0} \n",
                        $"{receivers[i]}", $"{E_norm.Real.ToString("E5")}", $"{E_norm.Imaginary.ToString("E5")}",
                        $"({H_norm.Real.ToString("E5")}, {H_norm.Imaginary.ToString("E5")})", $"{R.ToString("E5")}");
            table.Append(str.Replace(".", ","));
        }

        TextBoxSolver.Document.Blocks.Add(new Paragraph(new Run(table.ToString())));
        File.WriteAllText(@"slau/result.txt", table.ToString());

        // ********************************************************************************************** //
    }


    //: Обработка кнопки "Добавить приемник"
    private void AddReceiver_Click(object sender, RoutedEventArgs e) {
        
        // Если значение приемника пусто
        if (ReceiverBox.Text == String.Empty || receivers_str.Contains(ReceiverBox.Text)) {
            MessageBox.Show("Значение приемника не указано или такой приемник уже задан!");
            return;
        }
        receivers_str.Add(ReceiverBox.Text);
        receiversList.Items.Refresh();
    }

    //: Обработка кнопки "Удалить приемник"
    private void RemoveReceiver_Click(object sender, RoutedEventArgs e) {
        
        // Если приемник не выбран
        if (receiversList.SelectedValue == null) {
            MessageBox.Show("Выберете приемник!");
            return;
        }
        receivers_str.Remove((string)receiversList.SelectedValue);
        receiversList.Items.Refresh();
    }

    //: Обработка кнопки "Построить график Rk"
    private void DrawRkPlot_Click(object sender, RoutedEventArgs e) {
        Rk rk = new Rk(lRk, receivers_str.Select(n => double.Parse(n)).OrderByDescending(n => n).ToList());
        rk.Show();
    }

    //: Обработка кнопки "Построить график Ex"
    private void DrawExPlot_Click(object sender, RoutedEventArgs e) {
        Ex ex = new Ex(lEx, receivers_str.Select(n => double.Parse(n)).OrderByDescending(n => n).ToList());
        ex.Show();
    }
}