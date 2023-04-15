using FEM.grid;

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
        fem.TrySetParameter("Nu", nu);
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

        List<(int id, Node node)> surface = new List<(int, Node)>();
        for (int i = 0; i < grid.Edges.Count; i++) {
            if (Abs(grid.Edges[i].NodeBegin.Y) <= 1e-10 && Abs(grid.Edges[i].NodeEnd.Y) <= 1e-10) {
                Node node = new Node((grid.Edges[i].NodeBegin.X + grid.Edges[i].NodeEnd.X) / 2.0, grid.Edges[i].NodeBegin.Y);
                surface.Add((i, node));
            }
        }

        Table table_rec = new Table("Таблица с приемниками");
        table_rec.AddColumn(("Приемник", 15), ("Значение", 40));

        for (int i = 0; i < receivers.Length; i++) {

            // Находим узлы из одномернной задачи
            int id1 = 0, id2 = 0;
            for (int j = 0; j < surface.Count - 1; j++) {
                if (receivers[i] >= surface[j].Item2.X && receivers[i] <= surface[j + 1].Item2.X) {
                    id1 = j;
                    id2 = j + 1;
                }
            }

            // Находим коэффициенты прямой
            Complex k = (slau.q[surface[id2].id] - slau.q[surface[id1].id]) / (surface[id2].node.X - surface[id1].node.X);
            Complex b = slau.q[surface[id2].id] - k * surface[id2].node.X;

            var nu = double.Parse(NuBox.Text);

            var value = (-1) * new Complex(0, 1) * (2.0 * PI * nu) * (k * receivers[i] + b + harm1d.U[^1]);
            value /= harm1d.U[^1];
            //value = new Complex(value.Real / harm1d.U[^1].Real, value.Imaginary / harm1d.U[^1].Imaginary);

            table_rec.AddRow($"{receivers[i]}", $"{value.Real.ToString("E5")} {value.Imaginary.ToString("E5")}");
        }

        TextBoxSolver.Text = table_rec.ToString();
        table_rec.WriteToFile(@"slau\solve.txt");
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
}