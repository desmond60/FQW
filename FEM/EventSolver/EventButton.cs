namespace FEM;

//: Обработчики Button
public partial class Solver
{

    //: Обработка кнопки "Построить СЛАУ"
    private void CreateSLAU_Click(object sender, RoutedEventArgs e) {

        var receivers = new Vector<double>(receivers_str.Select(n => double.Parse(n)).OrderByDescending(n => n).ToArray());
        var nu = double.Parse(NuBox.Text);

        FEM fem = new FEM(grid);
        fem.TrySetParameter("Receivers", receivers);
        fem.TrySetParameter("Nu", nu);
        fem.CreateSLAU();
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