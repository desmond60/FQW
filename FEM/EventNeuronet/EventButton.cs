namespace FEM;

//: Обработчики Button
public partial class Neuronet
{
    //: Генерация синтетических данных
    private void GenerateSynthetic_Click(object sender, RoutedEventArgs e)
    {
        // Создание таймера
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        int CountData = int.Parse(NumSyntheticData.Text);

        Directory.CreateDirectory(pathData);
        for (int i = 1; i <= CountData; i++)
        {
            string tmpPath = pathData + "/" + i;
            Directory.CreateDirectory(tmpPath);

            // Создание сетки и решения
            GenerateSynthetic(grid, tmpPath);
        }

        // Остановка таймера
        stopWatch.Stop();
        TimeSpan ts = stopWatch.Elapsed;
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        Timer.Text = elapsedTime;
    }



    //: Обработка кнопки "Добавить приемник"
    private void AddReceiver_Click(object sender, RoutedEventArgs e)
    {
        // Если значение приемника пусто
        if (ReceiverBox.Text == String.Empty || receivers_str.Contains(ReceiverBox.Text))
        {
            MessageBox.Show("Значение приемника не указано или такой приемник уже задан!");
            return;
        }
        receivers_str.Add(ReceiverBox.Text);
        receiversList.Items.Refresh();
    }

    //: Обработка кнопки "Удалить приемник"
    private void RemoveReceiver_Click(object sender, RoutedEventArgs e)
    {
        // Если приемник не выбран
        if (receiversList.SelectedValue == null)
        {
            MessageBox.Show("Выберете приемник!");
            return;
        }
        receivers_str.Remove((string)receiversList.SelectedValue);
        receiversList.Items.Refresh();
    }

}

