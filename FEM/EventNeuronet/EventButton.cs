using ScottPlot.Drawing.Colormaps;
using System;
using System.Diagnostics.PerformanceData;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

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
            int id = 10032 + i;
            if (id >= 10033)
            {
                string tmpPath = pathData + "/" + id + ".txt";
                //Directory.CreateDirectory(tmpPath);

                // Создание сетки и решения
                //NewGenerateSynthetic(grid, tmpPath, centres[i - 1]);
                GenerateSynthetic(grid, tmpPath, centres[i - 1]);
            }
        }

        // Остановка таймера
        stopWatch.Stop();
        TimeSpan ts = stopWatch.Elapsed;
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        Timer.Text = elapsedTime;
    }

    private void GenerateSynthetic_MTZ_2D_Click(object sender, RoutedEventArgs e)
    {
        //double[] nuList = new[] {
        //   0.0001, 0.01, 0.2, 0.4, 1.0000, 30.0000, 60.0000, 90.0000,
        //   120.0000, 150.0000, 180.0000, 210.0000, 240.0000, 270.0000,
        //   300.0000, 325.0000, 350.0000, 375.0000, 450.0000, 500.0000
        //};

        double[] nuList = new[] {
           0.0001, 0.0004, 0.0016, 0.0064,
           0.01, 0.02, 0.04, 0.08,
           0.1, 0.2, 0.4, 0.8,
           1, 2, 4, 8, 32, 128, 256, 500
        };

        //double[] sigmaList = new[] {
        //    10000.0, 1000.0, 100.0, 10.0, 1.0, 0.5, 0.1, 2.0, 0.2, 0.8, 20
        //};

        double[] receiversList = new double[] {
            0, 500, 1000, 1500, 2000, 2500, 3000,
            3500, 4000, 4500, 5000, 5500, 6000,
            6500, 7000, 7500, 8000, 8500, 9000,
            9500, 10000, 10500, 11000, 11500, 12000,
            12500, 13000, 13500, 14000, 14500, 15000,
            15500, 16000
        };

        (double width, double height)[] sizes = new[] {
            (1000.0, 1000.0), // 0
            (1000.0, 2000.0), // 1 Основная модель
            (1000.0, 4000.0), // 2
            (1000.0, 8000.0), // 3
            (2000.0, 1000.0), // 4
            (4000.0, 1000.0), // 5
            (8000.0, 1000.0), // 6

            (2000.0, 2000.0), // 7
            (4000.0, 4000.0), // 8
            (8000.0, 8000.0) // 9
        };

        int totalPoints = 1500; //1200;

        // Для 4 потоков проводимость
        double[] numbersForThreads = new double[] { 2.0, 0.2, 0.8, 20 };

        // Создание центров для каждой пропорции
        List<List<Node>> centres = new List<List<Node>>();
        for (int i = 0; i < sizes.Length; i++)
            centres.Add(NewGenerateCentres(totalPoints, sizes[i]));

        // Пути к четырем папкам с файлами 
        var folderPaths = new List<string> { "1", "2", "3", "4" };

        // Создаем папку output, если ее нет
        Directory.CreateDirectory(pathData);
        string outputFolder = Path.Combine(pathData, $"DataSet");
        if (!Directory.Exists(outputFolder))
            Directory.CreateDirectory(outputFolder);

        // Создаем экземпляр SemaphoreSlim для последовательной записи
        var semaphore = new SemaphoreSlim(1, 1); // Один поток может писать в DataSet одновременно

        // Пример выполнения параллельной обработки
        Parallel.ForEach(folderPaths, new ParallelOptions { MaxDegreeOfParallelism = 4 }, (folderPath, state, index) =>
        {
        // folderPath - папка потока
        // outputFolder - папка сохранения DataSet
        // double threadNumber = numbersForThreads[index]; - Проводимость для потока

            for (int i = 8; i < 9; i++) //sizes.Length
            {
                (double width, double height) size = sizes[i];
                List<Node> centres_nodes = centres[i];
                double sigma = numbersForThreads[index];

                for (int j = 0; j < centres_nodes.Count; j++) // centres_nodes.Count
                {
                    // Новая точки объекта
                    Vector<double> begin = new Vector<double>(2);
                    Vector<double> end = new Vector<double>(2);
                    begin[0] = centres_nodes[j].X - size.width / 2.0;
                    begin[1] = centres_nodes[j].Y - size.height / 2.0;
                    end[0] = centres_nodes[j].X + size.width / 2.0;
                    end[1] = centres_nodes[j].Y + size.height / 2.0;

                    StringBuilder Rok = new StringBuilder();
                    StringBuilder Fi = new StringBuilder();
                    for (int k = 0; k < nuList.Length; k++)
                    {
                        string path = Path.Combine(pathData, folderPath);

                        // Решение вместо моего для первого направления тока
                        MTZ2D mtz2D = new MTZ2D();
                        mtz2D.WriteFileMtz2D_Full(nuList[k], sigma, receiversList, begin, end, path, 0);
                        mtz2D.RunMtz2D(path);
                        mtz2D.ReadSolve(path);

                        // Решение для другого напраления тока
                        MTZ2D mtz2D_2 = new MTZ2D();
                        mtz2D_2.WriteFileMtz2D_Full(nuList[k], sigma, receiversList, begin, end, path, 1);
                        mtz2D_2.RunMtz2D(path);
                        mtz2D_2.ReadSolve(path);

                        Rok.Append(String.Join(" ", mtz2D.Rk) + " ");
                        Rok.Append(String.Join(" ", mtz2D_2.Rk));
                        Rok.AppendLine();

                        Fi.Append(String.Join(" ", mtz2D.Fi) + " ");
                        Fi.Append(String.Join(" ", mtz2D_2.Fi));
                        Fi.AppendLine();
                    }

                    // Добавить начальную строку
                    StringBuilder output = new StringBuilder();
                    output.AppendLine($"{centres_nodes[j].X} {centres_nodes[j].Y} {size.width} {size.height} {sigma}");
                    output.Append(Rok.Append(Fi.ToString()));

                    // Ожидание блокировки перед записью
                    semaphore.Wait();
                    try
                    {
                        int count_files = new DirectoryInfo(outputFolder).GetFiles().Length;
                        File.WriteAllText(outputFolder + $"/{count_files + 1}.txt", output.ToString());
                    }
                    finally
                    {
                        // Освобождение блокировки
                        semaphore.Release();
                    }
                }
            }
        });
    }

    private void GenerateSynthetic_MTZ_2D_Click_Solo(object sender, RoutedEventArgs e)
    {
        double[] nuList = new[] {
           0.0001, 0.0004, 0.0016, 0.0064,
           0.01, 0.02, 0.04, 0.08,
           0.1, 0.2, 0.4, 0.8,
           1, 2, 4, 8, 32, 128, 256, 500
        };

        double[] receiversList = new double[] {
            0, 500, 1000, 1500, 2000, 2500, 3000,
            3500, 4000, 4500, 5000, 5500, 6000,
            6500, 7000, 7500, 8000, 8500, 9000,
            9500, 10000, 10500, 11000, 11500, 12000,
            12500, 13000, 13500, 14000, 14500, 15000,
            15500, 16000
        };

        (double width, double height)[] sizes = new[] {
            (1000.0, 1000.0), // 0
            (1000.0, 2000.0), // Основная модель // 1
            (1000.0, 4000.0), // 2
            (1000.0, 8000.0), // 3
            (2000.0, 1000.0), // 4
            (4000.0, 1000.0), // 5
            (8000.0, 1000.0), // 6

            (2000.0, 2000.0), // 7
            (4000.0, 4000.0), // 8
            (8000.0, 8000.0), // 9

            // Новые для теста
            (5000, 500), // 10
            (1500, 3000), // 11
            (2500, 2500), // 12
            (4000, 3500) // 13
        };

        int totalPoints = 1000;

        //double[] sigmaList = new[] {
        //    10000.0, 1000.0, 100.0, 10.0, 1.0, 0.5, 0.1, 2.0, 0.2, 0.8, 20
        //};

        double sigma = 8000.0;

        // Создание центров для каждой пропорции
        List<List<Node>> centres = new List<List<Node>>();
        for (int i = 0; i < sizes.Length; i++)
            centres.Add(new List<Node>() { new Node(8000, -3000)}); //centres.Add(NewGenerateCentres(totalPoints, sizes[i]));

        // Путь к первой папке
        var folderPath = "1";

        // Создаем папку output, если ее нет
        Directory.CreateDirectory(pathData);
        string outputFolder = Path.Combine(pathData, $"Test"); // "DataSet" or "Test"
        if (!Directory.Exists(outputFolder))
            Directory.CreateDirectory(outputFolder);

        for (int i = 0; i < 1; i++) //sizes.Length
        {
            (double width, double height) size = sizes[i];
            List<Node> centres_nodes = centres[0]; // centres[i]

            for (int j = 0; j < centres_nodes.Count; j++) // centres_nodes.Count
            {
                // Новая точки объекта
                Vector<double> begin = new Vector<double>(2);
                Vector<double> end = new Vector<double>(2);
                begin[0] = centres_nodes[j].X - size.width / 2.0;
                begin[1] = centres_nodes[j].Y - size.height / 2.0;
                end[0] = centres_nodes[j].X + size.width / 2.0;
                end[1] = centres_nodes[j].Y + size.height / 2.0;

                StringBuilder Rok = new StringBuilder();
                StringBuilder Fi = new StringBuilder();
                for (int k = 0; k < nuList.Length; k++)
                {
                    string path = Path.Combine(pathData, folderPath);

                    // Решение вместо моего для первого направления тока
                    MTZ2D mtz2D = new MTZ2D();
                    mtz2D.WriteFileMtz2D_Full(nuList[k], sigma, receiversList, begin, end, path, 0);
                    mtz2D.RunMtz2D(path);
                    mtz2D.ReadSolve(path);

                    // Решение для другого напраления тока
                    MTZ2D mtz2D_2 = new MTZ2D();
                    mtz2D_2.WriteFileMtz2D_Full(nuList[k], sigma, receiversList, begin, end, path, 1);
                    mtz2D_2.RunMtz2D(path);
                    mtz2D_2.ReadSolve(path);

                    Rok.Append(String.Join(" ", mtz2D.Rk) + " ");
                    Rok.Append(String.Join(" ", mtz2D_2.Rk));
                    Rok.AppendLine();

                    Fi.Append(String.Join(" ", mtz2D.Fi) + " ");
                    Fi.Append(String.Join(" ", mtz2D_2.Fi));
                    Fi.AppendLine();
                }

                // Добавить начальную строку
                StringBuilder output = new StringBuilder();
                output.AppendLine($"{centres_nodes[j].X} {centres_nodes[j].Y} {size.width} {size.height} {sigma}");
                output.Append(Rok.Append(Fi.ToString()));

                int count_files = new DirectoryInfo(outputFolder).GetFiles().Length;
                File.WriteAllText(outputFolder + $"/{count_files + 1}.txt", output.ToString());
            }
        }
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

