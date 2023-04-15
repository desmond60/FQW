namespace FEM.grid;

// % ***** Структура одномерной задачи ***** % //
public struct Harm1D
{
    //: Поля и свойства
    public List<Node>    Nodes { get; set; }   // Узлы
    public List<Complex> U     { get; set; }   // Решение

    //: Конструктор
    public Harm1D() {
        Nodes = new List<Node>();
        U = new List<Complex>();
    }

    //: Запись данных для одномерной задачи
    public void WriteFileHarm1D(List<Layer> layers, double nu) {

        // Заполнение файла sreda1d.ay
        var sreda_str = new StringBuilder($"{layers.Count}\n");
        for (int i = 1; i < layers.Count + 1; i++)
            sreda_str.Append($"{layers[^i].Y} {1e-2.ToString("E")} {1}\n");
        File.WriteAllText(@"harm1D\sreda1d.ay", sreda_str.ToString());

        // Заполнение файла sig3d
        var sig3d_str = new StringBuilder();
        sig3d_str.Append($"{1} {0.ToString("E")} {0.ToString("E")}\n");
        for (int i = 0, id = 2; i < layers.Count; i++, id++)
            sig3d_str.Append($"{id} {0.ToString("E")} {layers[i].Sigma.ToString("E")}\n");
        File.WriteAllText(@"harm1D\sig3d", sig3d_str.ToString());

        // Заполнение файла nu
        File.WriteAllText(@"harm1D\nu", nu.ToString());
    }

    //: Запустить решатель одномерной задачи
    public void RunHarm1D() {
        string command = "cd harm1d & Harm1D.exe & cd ..";
        Process process = Process.Start("cmd.exe", "/C " + command);
        process.WaitForExit();
    }

    //: Чтение сетки и решения одномерной задачи
    public void ReadMeshAndSolve() {

        // Чтение узлов
        string[] nodes_str = File.ReadAllLines(@"harm1d\mesh1d");
        for (int i = 0; i < nodes_str.Length; i++)
            if (nodes_str[i] != String.Empty)
                Nodes.Add(new Node(0, double.Parse(nodes_str[i])));

        // Чтение решения
        string[] usin_str = File.ReadAllLines(@"harm1d\usin.dat");
        string[] ucos_str = File.ReadAllLines(@"harm1d\ucos.dat");
        for (int i = 0; i < Nodes.Count; i++)
            U.Add(new Complex(double.Parse(usin_str[i].Split(" ", StringSplitOptions.RemoveEmptyEntries)[1]),
                              double.Parse(ucos_str[i].Split(" ", StringSplitOptions.RemoveEmptyEntries)[1])
                              )
                );
    }
}