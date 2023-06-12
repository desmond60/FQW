namespace FEM.grid;

// % ***** Структура сетки ***** % //
public struct Grid
{
    //: Поля и свойства
    public List<Node>   Nodes;  // Узлы
    public List<Elem>   Elems;  // КЭ
    public List<Edge>   Edges;  // Ребра
    public List<Bound>  Bounds; // Краевые
    public List<Item>   Items;  // Объекты
    public List<Layer>  Layers; // Слои
    public List<(double Sigma1D, double Sigma2D)> Sigmas; // Значения проводимости

    //: Конструктор
    public Grid(List<Node> nodes, List<Edge> edges, List<Elem> elems, List<Bound> bounds, List<Item> items, List<Layer> layers, List<(double, double)> sigmas) {
        Nodes  = nodes;
        Edges  = edges;
        Elems  = elems;
        Bounds = bounds;
        Items  = items;
        Layers = layers;
        Sigmas = sigmas;
    }

    //: Записать сетку
    public void WriteGrid() {

        Directory.CreateDirectory(@"grid");
        Directory.CreateDirectory(@"slau/slauTXT");
        Directory.CreateDirectory(@"slau/slauBIN");
        Directory.CreateDirectory(@"harm1d");

        // Запись узлов сетки
        File.WriteAllText(@"grid/nodes.txt", $"{Nodes.Count} \n" + String.Join("\n", Nodes), Encoding.UTF8);

        // Запись КЭ
        File.WriteAllText(@"grid/elems.txt", $"{Elems.Count} \n" + String.Join("\n", Elems), Encoding.UTF8);

        // Записб ребер
        File.WriteAllText(@"grid/edges.txt", $"{Edges.Count} \n" + String.Join("\n", Edges), Encoding.UTF8);

        // Запись краевых
        File.WriteAllText(@"grid/bounds.txt", $"{Bounds.Count} \n" + String.Join("\n", Bounds), Encoding.UTF8);

        // Запись объектов
        File.WriteAllText(@"grid/items.txt", $"{Items.Count} \n" + String.Join("\n", Items), Encoding.UTF8);

        // Запись слоев
        File.WriteAllText(@"grid/layers.txt", $"{Layers.Count} \n" + String.Join("\n", Layers), Encoding.UTF8);

        // Запись проводимости
        File.WriteAllText(@"grid/sigmas.txt", $"{Sigmas.Count} \n" + String.Join("\n", Sigmas.Select(n => n.Sigma1D + " " + n.Sigma2D)), Encoding.UTF8);
    }

    //: Загрузить сетку
    public void LoadGrid() {

        // Чтение узлов
        string[] Fnodes = File.ReadAllLines(@"grid/nodes.txt");
        Nodes = new List<Node>();
        for (int i = 1; i < int.Parse(Fnodes[0]) + 1; i++) {
            var node = Fnodes[i].Trim().Split(" ");
            Nodes.Add(new Node(double.Parse(node[0]), double.Parse(node[1])));
        }

        // Чтение элементов
        string[] FElems = File.ReadAllLines(@"grid/elems.txt");
        Elems = new List<Elem>();
        for (int i = 1; i < int.Parse(FElems[0]) + 1; i++) {
            var elem = FElems[i].Trim().Split(" ");
            Elems.Add(new Elem(int.Parse(elem[0]), int.Parse(elem[1]), int.Parse(elem[2]), int.Parse(elem[3])));
            Elems[i - 1] = Elems[i - 1] with { Edge = new int[] { int.Parse(elem[4]), int.Parse(elem[5]), int.Parse(elem[6]), int.Parse(elem[7]) },
                                               Material = int.Parse(elem[8]) };
        }

        // Чтение ребер
        string[] FEdges = File.ReadAllLines(@"grid/edges.txt");
        Edges = new List<Edge>();
        for (int i = 1; i < int.Parse(FEdges[0]) + 1; i++) {
            var edge = FEdges[i].Trim().Split(" ");
            var node1 = new Node(double.Parse(edge[0]), double.Parse(edge[1]));
            var node2 = new Node(double.Parse(edge[2]), double.Parse(edge[3]));
            Edges.Add(new Edge(node1, node2));
        }

        // Чтение краевых
        string[] FBounds = File.ReadAllLines(@"grid/bounds.txt");
        Bounds = new List<Bound>();
        for (int i = 1; i < int.Parse(FBounds[0]) + 1; i++) {
            var bound = FBounds[i].Trim().Split(" ");
            Bounds.Add(new Bound(int.Parse(bound[0]), int.Parse(bound[1]), int.Parse(bound[2])));
        }

        // Чтение объектов
        string[] FItems = File.ReadAllLines(@"grid/items.txt");
        Items = new List<Item>();
        for (int i = 1; i < int.Parse(FItems[0]) + 1; i++) {
            var item = FItems[i].Trim().Split(" ");
            Items.Add(new Item(new Vector<double>(new double[] { double.Parse(item[0]), double.Parse(item[1]) }),
                               new Vector<double>(new double[] { double.Parse(item[2]), double.Parse(item[3]) }),
                               int.Parse(item[4]),
                               int.Parse(item[5]),
                               double.Parse(item[6]),
                               item[7]));
        }

        // Чтение слоев
        string[] FLayers = File.ReadAllLines(@"grid/layers.txt");
        Layers = new List<Layer>();
        for (int i = 1; i < int.Parse(FLayers[0]) + 1; i++) {
            var layer = FLayers[i].Trim().Split(" ");
            Layers.Add(new Layer(double.Parse(layer[0]), double.Parse(layer[1])));
        }

        // Чтение проводимости
        string[] FSigmas = File.ReadAllLines(@"grid/sigmas.txt");
        Sigmas = new List<(double Sigma1D, double Sigma2D)>();
        for (int i = 1; i < int.Parse(FSigmas[0]) + 1; i++) {
            var sigma = FSigmas[i].Trim().Split(" ");
            Sigmas.Add((double.Parse(sigma[0]), double.Parse(sigma[1])));
        }

        // Ребра, которые находятся в воздухе
        double left_b = Nodes[0].X;
        double right_b = Nodes[^1].X;
        double exit_y = Nodes[^1].Y;
        double temp = right_b;

        for (int i = 0; i < Elems.Count; i++) {

            // Середина элемента
            Node node = new Node(
               (Nodes[Elems[i].Node[0]].X + Nodes[Elems[i].Node[1]].X) / 2.0,
               (Nodes[Elems[i].Node[0]].Y + Nodes[Elems[i].Node[2]].Y) / 2.0
            );

            if (node.Y < 0.0) continue;

            int num_up_edge = Elems[i].Edge[3];
            Edge up_edge = Edges[num_up_edge];

            if (Abs(up_edge.NodeEnd.Y - exit_y) <= 1e-10) return;

            if (Abs(up_edge.NodeEnd.X - right_b) <= 1e-10 || Abs(up_edge.NodeBegin.X - right_b) <= 1e-10) {
                if (Abs(temp - left_b) <= 1e-10)
                    Bounds.Add(new Bound(1, 4, num_up_edge));
                temp = temp == left_b ? right_b : left_b;
                continue;
            }

            if (Abs(up_edge.NodeEnd.X - temp) <= 1e-10 || Abs(up_edge.NodeBegin.X - temp) <= 1e-10)
                continue;

            // Заносим ребро как краевое
            Bounds.Add(new Bound(1, 4, num_up_edge));
        }
    }

    //: Деконструктор
    public void Deconstruct(out List<Node>    nodes,
                            out List<Edge>    edges,
                            out List<Elem>    elems,
                            out List<Bound>   kraevs,
                            out List<Item>    items,
                            out List<Layer>  layers,
                            out List<(double, double)> sigmas) {
        edges  = Edges;
        nodes  = Nodes;
        elems  = Elems;
        kraevs = Bounds;
        items  = Items;
        layers = Layers;
        sigmas = Sigmas;
    }
}