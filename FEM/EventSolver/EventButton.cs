namespace FEM;

//: Обработчики Button
public partial class Solver
{

    //: Обработка кнопки "Построить СЛАУ"
    private void CreateSLAU_Click(object sender, RoutedEventArgs e) {

        var receivers = new Vector<double>(receivers_str.Select(n => double.Parse(n)).OrderByDescending(n => n).ToArray());
        var nu = double.Parse(NuBox.Text);

        // Составление одномерной задачи и ее решение
        Harm1D harm1d = new Harm1D();
        harm1d.WriteFileHarm1D(grid.Layers, nu);
        harm1d.RunHarm1D();
        harm1d.ReadMeshAndSolve();

        // Составление СЛАУ
        FEM fem = new FEM(grid, harm1d);
        fem.TrySetParameter("Receivers", receivers);
        fem.TrySetParameter("Nu", nu);
        slau = fem.CreateSLAU();

        // Запись СЛАУ
        slau.WriteTXT();
        slau.WriteBIN();
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