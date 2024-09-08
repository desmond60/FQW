using ScottPlot.MarkerShapes;

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
        int countF = int.Parse(CountNu.Text);
        double K = 1.2;

/*        NuList.Add(beginF);
        double start = beginF;
        for (int i = 0; i < countF - 1; i++)
        {
            start *= 2;
            NuList.Add(start);
        }*/


        double distY = endF - beginF;
        double hY;
        if (K != 1)
        {
            if (K > 0)
                hY = distY * (1 - K) / (1 - Pow(K, countF));
            else
            {
                K = 1 / Abs(K);
                hY = distY * (K - 1) / (Pow(K, countF) - 1);
            }
        }
        else
            hY = distY / countF;

        NuList.Add(beginF);
        double startY = beginF;
        while (true)
        {
            startY += hY;
            hY *= K;
            NuList.Add(startY);

            if (Abs((startY + hY) - endF) < 1e-7)
            {
                NuList.Add(endF);
                break;
            }
        }


        /*        double dist = endF - beginF;
                double h = dist / countF;

                NuList.Add(beginF);
                double start = beginF;
                while (true)
                {
                    start += h;
                    NuList.Add(start);

                    if (Abs((start + h) - endF) < 1e-7)
                    {
                        NuList.Add(endF);
                        break;
                    }
                }*/

        // Создание таймера
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        int CountData = int.Parse(NumSyntheticData.Text);

        Directory.CreateDirectory(pathData);
        List<Node> centres = GenerateCentres(grid, CountData);
        for (int i = 1; i <= centres.Count; i++)
        {
            string tmpPath = pathData + "/" + i;
            Directory.CreateDirectory(tmpPath);

            // Создание сетки и решения
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

