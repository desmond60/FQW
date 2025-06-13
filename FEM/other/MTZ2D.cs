namespace FEM;

// % ***** ��������� ��������� ������ ��� ***** % //
public struct MTZ2D
{
    public List<double> Rk { get; set; } // ������� Rok (�������������)
    public List<double> Fi { get; set; } // ������� Fi (����)

    public MTZ2D()
    {
        Rk = new List<double>();
        Fi = new List<double>();
    }

    //: ������ ������ ��� ���������� ������
    public void WriteFileMtz2D(double nu, Item item, int dir = 1)
    {
        // ��������� ����� geoprep.dat
        var lines = File.ReadAllLines(@"mtz_2d\geoprep.dat").ToList();

        int index_nu = 3;
        int index_dir = 10;
        int index_obj_X = 29;
        int index_obj_Y = 33;

        // nu
        lines.RemoveAt(index_nu);
        lines.Insert(index_nu, $"0 {nu}");

        // ����������� ����
        lines.RemoveAt(index_dir);
        lines.Insert(index_dir, $"{dir} //                  (0 - X, 1 - Y, 2 - X and Y)");

        // ������
        lines.RemoveAt(index_obj_X);
        lines.Insert(index_obj_X, $"{item.Begin[0]}");
        lines.RemoveAt(index_obj_X + 1);
        lines.Insert(index_obj_X + 1, $"{item.End[0]}");

        lines.RemoveAt(index_obj_Y);
        lines.Insert(index_obj_Y, $"{item.Begin[1]}");
        lines.RemoveAt(index_obj_Y + 1);
        lines.Insert(index_obj_Y + 1, $"{item.End[1]}");

        // ���������� ����� nu
        File.WriteAllText(@"mtz_2d\geoprep.dat", String.Join("\n", lines));
    }

    //: ��������� �������� ���������� ������
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

    //: ��������� �������� ���������� ������
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

    //: ������ ����� � ������� ���������� ������
    public void ReadSolve()
    {
        // ������ �������������
        string[] rok_str = File.ReadAllLines(@"mtz_2d\rok");
        for (int i = 0; i < rok_str.Length; i++)
            if (rok_str[i] != String.Empty)
                Rk.Add(double.Parse(rok_str[i].Split(" ", StringSplitOptions.RemoveEmptyEntries)[1]));

        // ������ �����
        string[] fi_str = File.ReadAllLines(@"mtz_2d\fi");
        for (int i = 0; i < fi_str.Length; i++)
            if (fi_str[i] != String.Empty)
                Fi.Add(double.Parse(fi_str[i].Split(" ", StringSplitOptions.RemoveEmptyEntries)[1]));
    }

    //: ������ ����� � ������� ���������� ������
    public void ReadSolve(string path)
    {
        // ������ �������������
        string[] rok_str = File.ReadAllLines(path + @"\rok");
        for (int i = 0; i < rok_str.Length; i++)
            if (rok_str[i] != String.Empty)
                Rk.Add(double.Parse(rok_str[i].Split(" ", StringSplitOptions.RemoveEmptyEntries)[1]));

        // ������ �����
        string[] fi_str = File.ReadAllLines(path + @"\fi");
        for (int i = 0; i < fi_str.Length; i++)
            if (fi_str[i] != String.Empty)
                Fi.Add(double.Parse(fi_str[i].Split(" ", StringSplitOptions.RemoveEmptyEntries)[1]));
    }

    //: ���������� ������� �����
    public void WriteFileMtz2D_Full(double nu, double sigma, double[] receivers, Vector<double> begin, Vector<double> end, string path, int dir = 1)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"// ������� �������� 1 - Low, 2 - Med, 3 - Hi");
        sb.AppendLine($"3");
        sb.AppendLine($"// �������");
        sb.AppendLine($"0 {nu} // ��������� (�� ������������) � �������");
        sb.AppendLine($"// ���������");
        sb.AppendLine($"0 // X");
        sb.AppendLine($"0 // Y");
        sb.AppendLine($"0 // Z");
        sb.AppendLine($"100 // R");
        sb.AppendLine($"1 // ����� ������");
        sb.AppendLine($"{dir} // ����������� ���� (0 - X, 1 - Y, 2 - X and Y)");
        sb.AppendLine($"// ���������");
        sb.AppendLine($"1 // ����� ������");
        sb.AppendLine($"{receivers.Length} // ���-��");
        for (int i = 0; i < receivers.Length; i++)
            sb.AppendLine($"{receivers[i]} 0 0 0.564 // X Y Z R");
        sb.AppendLine($"// ����");
        sb.AppendLine($"3 // ���-��");
        sb.AppendLine($"{begin[1]} 0.01 // Z Sigma (Sigma under Z)");
        sb.AppendLine($"{end[1]} 0.01 // Z Sigma (Sigma under Z)");
        sb.AppendLine($"0 0.01 // Z Sigma (Sigma under Z)");
        sb.AppendLine($"// �������");
        sb.AppendLine($"// ������ X ���������");
        sb.AppendLine($"2 // ���-��");
        sb.AppendLine($"{begin[0]}");
        sb.AppendLine($"{end[0]}");
        sb.AppendLine($"// ������ Y ���������");
        sb.AppendLine($"2 // ���-��");
        sb.AppendLine($"-20");
        sb.AppendLine($"20");
        sb.AppendLine($"// ���� �������");
        sb.AppendLine($"1 // ���-��");
        sb.AppendLine($"{sigma} 1 2 1 2 1 2");
        sb.AppendLine($"// ������������ �������");
        sb.AppendLine($"0 // ���-��");
        sb.AppendLine($" // �����");

        // ���������� ����� nu
        File.WriteAllText(path + @"\geoprep.dat", sb.ToString());
    }
}