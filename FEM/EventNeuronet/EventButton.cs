namespace FEM;

//: Обработчики Button
public partial class Neuronet
{
    //: Генерация синтетических данных
    private void GenerateSynthetic_Click(object sender, RoutedEventArgs e)
    {
        NuList.Clear();

        // Генерация частот
        double beginF = double.Parse(NuBoxFrom.Text);
        double endF = double.Parse(NuBoxTo.Text);
        int countF = int.Parse(CountNu.Text) - 1;
        double K = 1.2;

        //double distY = endF - beginF;
        //double hY;
        //if (K != 1)
        //{
        //    if (K > 0)
        //        hY = distY * (1 - K) / (1 - Pow(K, countF));
        //    else
        //    {
        //        K = 1 / Abs(K);
        //        hY = distY * (K - 1) / (Pow(K, countF) - 1);
        //    }
        //}
        //else
        //    hY = distY / countF;

        //NuList.Add(beginF);
        //double startY = beginF;
        //while (true)
        //{
        //    startY += hY;
        //    hY *= K;
        //    NuList.Add(startY);

        //    if (Abs((startY + hY) - endF) < 1e-7)
        //    {
        //        NuList.Add(endF);
        //        break;
        //    }
        //}

        // Частоты более целые
        //NuList.AddRange(new[] {
        //    0.0001, 0.001, 0.01, 0.1, 0.2, 0.3, 0.4, 0.5, 0.7,
        //    1.0000, 16.0000, 31.0000, 46.0000, 61.0000, 76.0000, 91.0000, 106.0000, 121.0000, 136.0000,
        //    151.0000, 166.0000, 181.0000, 196.0000, 211.0000, 226.0000, 241.0000, 256.0000, 271.0000, 286.0000, 300.0000,
        //    300.0000, 325.0000, 350.0000, 375.0000, 400.0000, 425.0000, 450.0000, 475.0000, 490.0000, 500.0000});

        NuList.AddRange(new[] {
           0.0001, 0.01, 0.2, 0.4, 1.0000, 30.0000, 60.0000, 90.0000,
           120.0000, 150.0000, 180.0000, 210.0000, 240.0000, 270.0000,
           300.0000, 325.0000, 350.0000, 375.0000, 450.0000, 500.0000});

        // Создание таймера
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        int CountData = int.Parse(NumSyntheticData.Text);

        Directory.CreateDirectory(pathData);
        List<Node> centres = GenerateCentres(grid, CountData);
        for (int i = 1; i <= centres.Count; i++)
        {
            string tmpPath = pathData + "/" + i + ".txt";
            //Directory.CreateDirectory(tmpPath);

            // Создание сетки и решения
            //NewGenerateSynthetic(grid, tmpPath, centres[i - 1]);
            GenerateSynthetic(grid, tmpPath, centres[i - 1]);
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

