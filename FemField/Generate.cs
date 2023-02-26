namespace FemField;

// % ****** Class Generate Grid ***** % //
public class Generate 
{
    //: Поля и свойства
    protected Vector<double> Begin_BG  { get; set; }   /// Начальная точка большого поля
    protected Vector<double> End_BG    { get; set; }   /// Конечная точка большого поля
    protected Vector<double> Begin_SML { get; set; }   /// Начальная точка маленького поля
    protected Vector<double> End_SML   { get; set; }   /// Конечная точка маленького поля
    protected Vector<double> Layers    { get; set; }   /// Горизонтальные слои сетки
    protected double   Nx           { get; set; }   /// Количество узлов маленького поля по Оси X
    protected double   Ny           { get; set; }   /// Количество узлов маленького поля по Оси Y
    protected double   Kx           { get; set; }   /// Коэффициент разрядки по Оси X
    protected double   Ky           { get; set; }   /// Коэффициент разрядки по Оси Y
    protected bool     IsStrictGrid { get; set; }   /// Строгость сетки

    protected string Path     { get; set; }   /// Путь к папке с задачейё

    private int N_X { get; set; }   /// Количество узлов по Оси X
    private int N_Y { get; set; }   /// Количество узлов по Оси Y
    private int Count_Node  => N_X * N_Y;                       /// Общее количество узлов
    private int Count_Elem  => (N_X - 1)*(N_Y - 1);             /// Общее количество КЭ
    private int Count_Bound => 2*(N_X - 1) + 2*(N_Y - 1);       /// Количество краевых
    private int Count_Edge  => N_X*(N_Y - 1) + N_Y*(N_X - 1);   /// Количество ребер

    private int[]? SideBound;                                   /// Номера краевых на сторонах
   
    //: Конструктор
    public Generate(Data _data, string _path) {
        
        (Begin_BG, End_BG, Begin_SML, End_SML, Layers, Nx, Ny, Kx, Ky, IsStrictGrid) = _data;
        SideBound = new int[] {_data.Bound[0], _data.Bound[1], _data.Bound[2], _data.Bound[3]}; 

        // Хранение сетки
        this.Path = _path +  "/grid";
        Directory.CreateDirectory(Path);
    }

    //: Основной метод генерации сетки
    public Grid<double> generate() {
        
        // Генерация узлов
        Node<double>[] nodes = generate_node();

        // Генерация элементов
        Elem[] elems = generate_elem();

        // Генерация ребер
        Edge<double>[] edges = generate_edge(elems, nodes);

        // Генерация краевых
        Bound[] bounds = generate_bound(edges);

        return new Grid<double>(nodes, edges, elems, bounds);
    }

    //: Генерация узлов сетки
    private Node<double>[] generate_node() {
        
        // Вычисление компонент
        double Hx_SML = (End_SML[0] - Begin_SML[0]) / (Nx - 1); // Шаг по мал. полю Ось X
        double Hy_SML = (End_SML[1] - Begin_SML[1]) / (Ny - 1); // Шаг по мал. полю Ось Y
        
        // Составляем листы шагов
        List<double> H_Axe_X = new List<double>();
        List<double> H_Axe_Y = new List<double>();

        // Генерация шагов по оси X
        for (int i = 0; i < Nx - 1; i++)
            H_Axe_X.Add(Hx_SML);

        double temp_V = Begin_SML[0];
        double temp_H = Hx_SML;
        while(temp_V >= Begin_BG[0]) {
            temp_H *= Kx;
            H_Axe_X.Insert(0, temp_H);
            temp_V -= temp_H;
            if (temp_V < Begin_BG[0] && IsStrictGrid) {
                temp_H -= (Begin_BG[0] - temp_V);
                H_Axe_X[0] = temp_H;
            }
        }
        double X = IsStrictGrid ? Begin_BG[1] : temp_V;

        temp_V = End_SML[0];
        temp_H = Hx_SML;
        while (temp_V <= End_BG[0]) {
            temp_H *= Kx;
            H_Axe_X.Add(temp_H);
            temp_V += temp_H;
            if (temp_V >= End_BG[0] && IsStrictGrid) {
                temp_H -= (temp_V - End_BG[0]);
                H_Axe_X[^1] = temp_H;
            }
        }

        // Генерация шагов по оси Y
        for (int i = 0; i < Ny - 1; i++)
            H_Axe_Y.Add(Hy_SML);

        temp_V = Begin_SML[1];
        temp_H = Hy_SML;
        while(temp_V >= Begin_BG[1]) {
            temp_H *= Ky;
            H_Axe_Y.Insert(0, temp_H);
            temp_V -= temp_H;
            if (temp_V < Begin_BG[1] && IsStrictGrid) {
                temp_H -= (Begin_BG[1] - temp_V);
                H_Axe_Y[0] = temp_H;
            }
        }
        double Y = IsStrictGrid ? Begin_BG[0] : temp_V;

        temp_V = End_SML[1];
        temp_H = Hy_SML;
        while (temp_V <= End_BG[1]) {
            temp_H *= Ky;
            H_Axe_Y.Add(temp_H);
            temp_V += temp_H;
            if (temp_V >= End_BG[1] && IsStrictGrid) {
                temp_H -= (temp_V - End_BG[1]);
                H_Axe_Y[^1] = temp_H;
            }
        }

        H_Axe_X.Insert(0, 0);
        H_Axe_Y.Insert(0, 0);

        // Учет горизонтальных слоев
        if (Layers.Length > 0) {
            double Y_temp = Y;
            List<(int index, double val_layer, double val_Y)> new_Axe = new List<(int, double, double)>(Layers.Length);
            for (int i = 0, id = 1; i < H_Axe_Y.Count && id != Layers.Length; i++) {
                
                // Прибавляем шаг
                Y_temp += H_Axe_Y[i];

                // Если равно значит узлы будут стоять на узлы, переходим у следующему слою
                if (Y_temp == Layers[^id]) {
                    id++;
                    continue;
                }

                // Если перескочили слой, значит добавим шаг в чтобы задеть слой
                if (Y_temp > Layers[^id]) {
                    new_Axe.Add((i, Layers[^id], Y_temp - H_Axe_Y[i]));
                    id++;
                }
            }

            // Корректировка шагов по Оси Y
            for (int i = 0; i < new_Axe.Count; i++) {
                double step = Abs(new_Axe[i].val_Y - new_Axe[i].val_layer);
                H_Axe_Y.Insert(new_Axe[i].index, step);
                H_Axe_Y[new_Axe[i].index + 1] = Abs(H_Axe_Y[new_Axe[i].index + 1] - step);
            }
        }
        
        // Количество узлов по Осям
        N_X = H_Axe_X.Count;
        N_Y = H_Axe_Y.Count;

        // Генерация узлов
        Node<double>[] nodes = new Node<double>[Count_Node];
        double Y_new = Y, X_new;
        for (int i = 0, k = 0; i < H_Axe_Y.Count; i++) {
            Y_new += H_Axe_Y[i];
            X_new = X;
            for (int j = 0; j < H_Axe_X.Count; j++, k++) {
                X_new += H_Axe_X[j];
                nodes[k] = new Node<double>(X_new, Y_new);
            }
        }

        File.WriteAllText(Path + "/nodes.txt", $"{H_Axe_X.Count} {H_Axe_Y.Count} \n" + String.Join("\n", nodes));
        return nodes;
    }

    //: Генерация конечных элементов
    private Elem[] generate_elem() {

        Elem[] elems = new Elem[Count_Elem];

        for (int i = 0, id = 0; i < N_Y - 1; i++)
            for (int j = 0; j < N_X - 1; j++, id++) {
                elems[id] = new Elem(
                      i * N_X + j,
                      i * N_X + j + 1,
                      (i + 1) * N_X + j,
                      (i + 1) * N_X + j + 1
                );
            }

        return elems;
    }

    //: Генерация ребер
    private Edge<double>[] generate_edge(Elem[] elems, Node<double>[] nodes) {

        Edge<double>[] edges = new Edge<double>[Count_Edge];

        for (int i = 0; i < N_Y - 1; i++)
            for (int j = 0; j < N_X - 1; j++) {
                int left   = i*((N_X - 1) + N_X) + (N_X - 1) + j;
                int right  = i*((N_X - 1) + N_X) + (N_X - 1) + j + 1;
                int bottom = i*((N_X - 1) + N_X) + j;
                int top    = (i + 1)*((N_X - 1) + N_X) + j;
                int n_elem = i*(N_X - 1) + j;

                edges[left]   = new Edge<double>(nodes[elems[n_elem].Node[0]], nodes[elems[n_elem].Node[2]]); 
                edges[right]  = new Edge<double>(nodes[elems[n_elem].Node[1]], nodes[elems[n_elem].Node[3]]); 
                edges[bottom] = new Edge<double>(nodes[elems[n_elem].Node[0]], nodes[elems[n_elem].Node[1]]); 
                edges[top]    = new Edge<double>(nodes[elems[n_elem].Node[2]], nodes[elems[n_elem].Node[3]]); 

                elems[n_elem] = elems[n_elem] with { Edge = new [] { left, right, bottom, top} };
            }

        // Запись ребер и КЭ
        File.WriteAllText(Path + "/edges.txt", $"{Count_Edge} \n" + String.Join("\n", edges));
        File.WriteAllText(Path + "/elems.txt", $"{Count_Elem} \n" + String.Join("\n", elems));

        return edges;  
    }

    //: Генерация краевых
    private Bound[] generate_bound(Edge<double>[] edge) {

        Bound[] bounds = new Bound[Count_Bound];
        int id = 0;

        // Нижняя сторона
        for (int i = 0; i < N_X - 1; i++, id++)
            bounds[id] = new Bound(
                SideBound![0],
                0,
                i
            );

        // Правая сторона
        for (int i = 1; i < N_Y; i++, id++)
            bounds[id] = new Bound(
                 SideBound![1],
                 1,
                 i*N_X + i*(N_X - 1) - 1
            );

        // Верхняя сторона
        for (int i = 0; i < N_X - 1; i++, id++)
            bounds[id] = new Bound(
                SideBound![2],
                2,
                N_X*(N_Y - 1) + (N_X - 1)*(N_Y - 1) + i
            );

        // Левая сторона
        for (int i = 0; i < N_Y - 1; i++, id++)
            bounds[id] = new Bound(
                 SideBound![3],
                 3,
                 (i + 1)*(N_X - 1) + i*N_X
            );

        // Сортируем по номеру краевого
        bounds = bounds.OrderByDescending(n => n.NumBound).ToArray();

        // Запись краевых
        File.WriteAllText(Path + "/bound.txt", $"{Count_Bound} \n" + String.Join("\n", bounds));
        
        return bounds;
    }

}
