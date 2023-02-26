namespace DrawGrid;


//: Построение сетки
public partial class MainWindow : Window
{

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
        while (temp_V >= Begin_BG[0]) {
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
        while (temp_V >= Begin_BG[1]) {
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
        if (layers.Count > 0) {
            double Y_temp = Y;
            List<(int index, double val_layer, double val_Y)> new_Axe = new List<(int, double, double)>(layers.Count);
            for (int i = 0, id = 1; i < H_Axe_Y.Count && id != layers.Count; i++) {

                // Прибавляем шаг
                Y_temp += H_Axe_Y[i];

                // Если равно значит узлы будут стоять на узлы, переходим у следующему слою
                if (Y_temp == layers[^id]) {
                    id++;
                    continue;
                }

                // Если перескочили слой, значит добавим шаг в чтобы задеть слой
                if (Y_temp > layers[^id]) {
                    new_Axe.Add((i, layers[^id], Y_temp - H_Axe_Y[i]));
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

        // Сколько будет линий на графике (Количество узлов на Осях)
        CountX = H_Axe_X.Count;
        CountY = H_Axe_Y.Count;

        // Генерация узлов
        Node<double>[] nodes = new Node<double>[CountX * CountY];
        double Y_new = Y, X_new;
        for (int i = 0, id = 0; i < H_Axe_Y.Count; i++) {
            Y_new += H_Axe_Y[i];
            X_new = X;
            for (int j = 0; j < H_Axe_X.Count; j++, id++) {
                X_new += H_Axe_X[j];
                nodes[id] = new Node<double>(X_new, Y_new);
            }
        }

        // Запись узлов сетки
        File.WriteAllText(@"nodes.txt", $"{CountX} {CountY} \n" + String.Join("\n", nodes));

        return nodes;
    }

    //: Генерация конечных элементов
    private Elem[] generate_elem() {

        Elem[] elems = new Elem[(CountX - 1) * (CountY - 1)];

        for (int i = 0, id = 0; i < CountY - 1; i++)
            for (int j = 0; j < CountX - 1; j++, id++) {
                elems[id] = new Elem(
                      i * CountX + j,
                      i * CountX + j + 1,
                      (i + 1) * CountX + j,
                      (i + 1) * CountX + j + 1
                );
            }

        return elems;
    }

    //: Генерация ребер
    private Edge<double>[] generate_edge(Elem[] elems, Node<double>[] nodes) {

        Edge<double>[] edges = new Edge<double>[CountX * (CountY - 1) + CountY * (CountX - 1)];

        for (int i = 0; i < CountY - 1; i++)
            for (int j = 0; j < CountX - 1; j++) {
                int left = i * ((CountX - 1) + CountX) + (CountX - 1) + j;
                int right = i * ((CountX - 1) + CountX) + (CountX - 1) + j + 1;
                int bottom = i * ((CountX - 1) + CountX) + j;
                int top = (i + 1) * ((CountX - 1) + CountX) + j;
                int n_elem = i * (CountX - 1) + j;

                edges[left] = new Edge<double>(nodes[elems[n_elem].Node[0]], nodes[elems[n_elem].Node[2]]);
                edges[right] = new Edge<double>(nodes[elems[n_elem].Node[1]], nodes[elems[n_elem].Node[3]]);
                edges[bottom] = new Edge<double>(nodes[elems[n_elem].Node[0]], nodes[elems[n_elem].Node[1]]);
                edges[top] = new Edge<double>(nodes[elems[n_elem].Node[2]], nodes[elems[n_elem].Node[3]]);

                elems[n_elem] = elems[n_elem] with { Edge = new[] { left, right, bottom, top } };
            }

        // Запись ребер и КЭ
        File.WriteAllText("edges.txt", $"{CountX * (CountY - 1) + CountY * (CountX - 1)} \n" + String.Join("\n", edges));
        File.WriteAllText("elems.txt", $"{(CountX - 1) * (CountY - 1)} \n" + String.Join("\n", elems));

        return edges;
    }

    //: Генерация краевых
    private Bound[] generate_bound(Edge<double>[] edge) {

        Bound[] bounds = new Bound[2 * (CountX - 1) + 2 * (CountY - 1)];
        int id = 0;

        // Нижняя сторона
        for (int i = 0; i < CountX - 1; i++, id++)
            bounds[id] = new Bound(
                SideBound![0],
                0,
                i
            );

        // Правая сторона
        for (int i = 1; i < CountY; i++, id++)
            bounds[id] = new Bound(
                 SideBound![1],
                 1,
                 i * CountX + i * (CountX - 1) - 1
            );

        // Верхняя сторона
        for (int i = 0; i < CountX - 1; i++, id++)
            bounds[id] = new Bound(
                SideBound![2],
                2,
                CountX * (CountY - 1) + (CountX - 1) * (CountY - 1) + i
            );

        // Левая сторона
        for (int i = 0; i < CountY - 1; i++, id++)
            bounds[id] = new Bound(
                 SideBound![3],
                 3,
                 (i + 1) * (CountX - 1) + i * CountX
            );

        // Сортируем по номеру краевого
        bounds = bounds.OrderByDescending(n => n.NumBound).ToArray();

        // Запись краевых
        File.WriteAllText("bounds.txt", $"{2 * (CountX - 1) + 2 * (CountY - 1)} \n" + String.Join("\n", bounds));

        return bounds;
    }

}