namespace FEM;

// % ***** ��������� ��������� ������ ��� ***** % //
public struct MTZ2D
{
    public List<double> Rk { get; set; } // ������� Rok

    public MTZ2D()
    {
        Rk = new List<double>();
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

    //: ������ ����� � ������� ���������� ������
    public void ReadSolve()
    {
        // ������ �����
        string[] solve_str = File.ReadAllLines(@"mtz_2d\rok");
        for (int i = 0; i < solve_str.Length; i++)
            if (solve_str[i] != String.Empty)
                Rk.Add(double.Parse(solve_str[i].Split(" ", StringSplitOptions.RemoveEmptyEntries)[1]));
    }
}