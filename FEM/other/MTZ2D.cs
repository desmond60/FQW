namespace FEM;

// % ***** Структура двумерной задачи МТЗ ***** % //
public struct MTZ2D
{
    public List<double> Rk { get; set; } // Решение Rok (сопротивление)
    public List<double> Fi { get; set; } // Решение Fi (фаза)

    public MTZ2D()
    {
        Rk = new List<double>();
        Fi = new List<double>();
    }

    //: Запись данных для одномерной задачи
    public void WriteFileMtz2D(double nu, Item item, int dir = 1)
    {
        // Изменение файла geoprep.dat
        var lines = File.ReadAllLines(@"mtz_2d\geoprep.dat").ToList();

        int index_nu = 3;
        int index_dir = 10;
        int index_obj_X = 29;
        int index_obj_Y = 33;

        // nu
        lines.RemoveAt(index_nu);
        lines.Insert(index_nu, $"0 {nu}");

        // Направление тока
        lines.RemoveAt(index_dir);
        lines.Insert(index_dir, $"{dir} //                  (0 - X, 1 - Y, 2 - X and Y)");

        // Объект
        lines.RemoveAt(index_obj_X);
        lines.Insert(index_obj_X, $"{item.Begin[0]}");
        lines.RemoveAt(index_obj_X + 1);
        lines.Insert(index_obj_X + 1, $"{item.End[0]}");

        lines.RemoveAt(index_obj_Y);
        lines.Insert(index_obj_Y, $"{item.Begin[1]}");
        lines.RemoveAt(index_obj_Y + 1);
        lines.Insert(index_obj_Y + 1, $"{item.End[1]}");

        // Заполнение файла nu
        File.WriteAllText(@"mtz_2d\geoprep.dat", String.Join("\n", lines));
    }

    //: Запустить решатель одномерной задачи
    public void RunMtz2D()
    {
        ProcessStartInfo processStart = new ProcessStartInfo();
        processStart.FileName = "cmd.exe";
        processStart.Arguments = "/C " + "cd mtz_2d & MTZ_2D.exe & cd ..";
        processStart.CreateNoWindow = true;
        processStart.UseShellExecute = false;
        processStart.WindowStyle = ProcessWindowStyle.Hidden;
        Process? process = Process.Start(processStart);
        process?.WaitForExit();
    }

    //: Запустить решатель одномерной задачи
    public void RunMtz2D(string path)
    {
        ProcessStartInfo processStart = new ProcessStartInfo();
        processStart.FileName = "cmd.exe";
        processStart.Arguments = "/C " + $"cd {path} & MTZ_2D.exe & cd ..";
        processStart.CreateNoWindow = true;
        processStart.UseShellExecute = false;
        processStart.WindowStyle = ProcessWindowStyle.Hidden;
        Process? process = Process.Start(processStart);
        process?.WaitForExit();
    }

    //: Чтение сетки и решения одномерной задачи
    public void ReadSolve()
    {
        // Чтение сопротивления
        string[] rok_str = File.ReadAllLines(@"mtz_2d\rok");
        for (int i = 0; i < rok_str.Length; i++)
            if (rok_str[i] != String.Empty)
                Rk.Add(double.Parse(rok_str[i].Split(" ", StringSplitOptions.RemoveEmptyEntries)[1]));

        // Чтение узлов
        string[] fi_str = File.ReadAllLines(@"mtz_2d\fi");
        for (int i = 0; i < fi_str.Length; i++)
            if (fi_str[i] != String.Empty)
                Fi.Add(double.Parse(fi_str[i].Split(" ", StringSplitOptions.RemoveEmptyEntries)[1]));
    }

    //: Чтение сетки и решения одномерной задачи
    public void ReadSolve(string path)
    {
        // Чтение сопротивления
        string[] rok_str = File.ReadAllLines(path + @"\rok");
        for (int i = 0; i < rok_str.Length; i++)
            if (rok_str[i] != String.Empty)
                Rk.Add(double.Parse(rok_str[i].Split(" ", StringSplitOptions.RemoveEmptyEntries)[1]));

        // Чтение узлов
        string[] fi_str = File.ReadAllLines(path + @"\fi");
        for (int i = 0; i < fi_str.Length; i++)
            if (fi_str[i] != String.Empty)
                Fi.Add(double.Parse(fi_str[i].Split(" ", StringSplitOptions.RemoveEmptyEntries)[1]));
    }

    //: Заполнение полного файла
    public void WriteFileMtz2D_Full(double nu, double sigma, double[] receivers, Vector<double> begin, Vector<double> end, string path, int dir = 1)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"// Уровень точности 1 - Low, 2 - Med, 3 - Hi");
        sb.AppendLine($"3");
        sb.AppendLine($"// Времена");
        sb.AppendLine($"0 {nu} // начальное (не используется) и частота");
        sb.AppendLine($"// Генератор");
        sb.AppendLine($"0 // X");
        sb.AppendLine($"0 // Y");
        sb.AppendLine($"0 // Z");
        sb.AppendLine($"100 // R");
        sb.AppendLine($"1 // число витков");
        sb.AppendLine($"{dir} // направление тока (0 - X, 1 - Y, 2 - X and Y)");
        sb.AppendLine($"// Приемники");
        sb.AppendLine($"1 // число витков");
        sb.AppendLine($"{receivers.Length} // кол-во");
        for (int i = 0; i < receivers.Length; i++)
            sb.AppendLine($"{receivers[i]} 0 0 0.564 // X Y Z R");
        sb.AppendLine($"// Слои");
        sb.AppendLine($"3 // кол-во");
        sb.AppendLine($"{begin[1]} 0.01 // Z Sigma (Sigma under Z)");
        sb.AppendLine($"{end[1]} 0.01 // Z Sigma (Sigma under Z)");
        sb.AppendLine($"0 0.01 // Z Sigma (Sigma under Z)");
        sb.AppendLine($"// Объекты");
        sb.AppendLine($"// Массив X координат");
        sb.AppendLine($"2 // кол-во");
        sb.AppendLine($"{begin[0]}");
        sb.AppendLine($"{end[0]}");
        sb.AppendLine($"// Массив Y координат");
        sb.AppendLine($"2 // кол-во");
        sb.AppendLine($"-20");
        sb.AppendLine($"20");
        sb.AppendLine($"// Сами объекты");
        sb.AppendLine($"1 // кол-во");
        sb.AppendLine($"{sigma} 1 2 1 2 1 2");
        sb.AppendLine($"// Нерегулярные объекты");
        sb.AppendLine($"0 // кол-во");
        sb.AppendLine($" // Конец");

        // Заполнение файла nu
        File.WriteAllText(path + @"\geoprep.dat", sb.ToString());
    }
}