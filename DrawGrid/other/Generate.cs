using ScottPlot.Drawing.Colormaps;
using System.Collections.Generic;
using System;

namespace DrawGrid;


//: Построение сетки
public partial class MainWindow : Window
{

    //: Генерация узлов сетки
    private Node<double>[] generate_node() {

        // Слои границ объектов
        List<double> border_X = new List<double>(2 * items.Count) { items[0].Begin[0], items[0].End[0] };
        List<double> border_Y = new List<double>(2 * items.Count) { items[0].Begin[1], items[0].End[1] };

        double X_min = items[0].Begin[0], Y_min = items[0].Begin[1];
        double X_max = items[0].End[0], Y_max = items[0].End[1];
        for (int i = 1; i < items.Count; i++) {
            if (items[i].Begin[0] < X_min)
                X_min = items[i].Begin[0];
            if (items[i].Begin[1] < Y_min)
                Y_min = items[i].Begin[1];
            if (items[i].End[0] > X_max)
                X_max = items[i].End[0];
            if (items[i].End[1] > Y_max)
                Y_max = items[i].End[1];

            border_X.Add(items[i].Begin[0]);
            border_X.Add(items[i].End[0]);
            border_Y.Add(items[i].Begin[1]);
            border_Y.Add(items[i].End[1]);
        }
        double copy_Y_min = Y_min;
        double copy_X_min = X_min;

        border_X = border_X.OrderByDescending(n => n).ToList();
        border_Y = border_Y.OrderByDescending(n => n).ToList();

        // Составляем листы шагов X
        List<double> H_Axe_X = new List<double>();

        // Находим объект
        int id = 0;
        for (int i = 0; i < items.Count; i++) {
            if (X_min == items[i].Begin[0]) {
                id = i;
                break;
            }
        }

        // Считаем шаг по объекту
        double Hx = (items[id].End[0] - items[id].Begin[0]) / items[id].Nx; // Шаг по мал. полю Ось X
        double X_temp_max = items[id].End[0];

        // Генерация шагов по оси X
        while (X_min < X_max) {

            while (X_min < X_temp_max) {
                X_min += Hx;
                H_Axe_X.Add(Hx); 
            }

            // Проверяем входит ли он в другой объект
            bool isFound = false;
            for (int i = 0; i < items.Count; i++) {
                if (X_min > items[i].Begin[0] && X_min < items[i].End[0] && id != i) {
                    id = i;
                    isFound = true;
                    Hx = (items[id].End[0] - items[id].Begin[0]) / items[id].Nx; // Шаг по мал. полю Ось X
                    X_temp_max = items[id].End[0];
                    break;
                }
            }

            if (X_min >= X_max)
                break;

            while (!isFound) {
                X_min += Hx;
                H_Axe_X.Add(Hx);

                for (int i = 0; i < items.Count; i++) {
                    if (X_min > items[i].Begin[0] && X_min < items[i].End[0]) {
                        id = i;
                        isFound = true;
                        Hx = (items[id].End[0] - items[id].Begin[0]) / items[id].Nx; // Шаг по мал. полю Ось X
                        X_temp_max = items[id].End[0];
                        break;
                    }
                }
            }
        }

        // Составляем листы шагов Y
        List<double> H_Axe_Y = new List<double>();

        // Находим объект
        id = 0;
        for (int i = 0; i < items.Count; i++){
            if (Y_min == items[i].Begin[1]){
                id = i;
                break;
            }
        }

        // Считаем шаг по объекту
        double Hy = (items[id].End[1] - items[id].Begin[1]) / items[id].Ny; // Шаг по мал. полю Ось Y
        double Y_temp_max = items[id].End[1];

        // Генерация шагов по оси X
        while (Y_min < Y_max) {

            while (Y_min < Y_temp_max) {
                Y_min += Hy;
                H_Axe_Y.Add(Hy);
            }

            // Проверяем входит ли он в другой объект
            bool isFound = false;
            for (int i = 0; i < items.Count; i++) {
                if (Y_min > items[i].Begin[1] && Y_min < items[i].End[1] && id != i) {
                    id = i;
                    isFound = true;
                    Hy = (items[id].End[1] - items[id].Begin[1]) / items[id].Ny; // Шаг по мал. полю Ось Y
                    Y_temp_max = items[id].End[1];
                    break;
                }
            }

            if (Y_min >= Y_max)
                break;

            while (!isFound) {
                Y_min += Hy;
                H_Axe_Y.Add(Hy);

                for (int i = 0; i < items.Count; i++) {
                    if (Y_min > items[i].Begin[1] && Y_min < items[i].End[1]) {
                        id = i;
                        isFound = true;
                        Hy = (items[id].End[1] - items[id].Begin[1]) / items[id].Ny; // Шаг по мал. полю Ось X
                        Y_temp_max = items[id].End[1];
                        break;
                    }
                }
            }
        }

        // Учет границ Y
        H_Axe_Y.Insert(0, 0);
        double Y_temp = copy_Y_min;
        id = 1;
        List<(int index, double val_layer, double val_Y)> new_Axe = new List<(int, double, double)>();
        for (int i = 0; i < H_Axe_Y.Count && id != border_Y.Count + 1; i++) {

            // Прибавляем шаг
            Y_temp += H_Axe_Y[i];

            // Если равно значит узлы будут стоять на узлы, переходим у следующему слою
            if (Abs(Y_temp - border_Y[^id]) <= 1e-6) {
                id++;
                continue;
            }

            // Если перескочили слой, значит добавим шаг в чтобы задеть слой
            if (Y_temp > border_Y[^id]) {
                new_Axe.Add((i + new_Axe.Count, border_Y[^id], Y_temp - H_Axe_Y[i]));
                id++;
            }
        }

        // Корректировка шагов по Оси Y
        for (int i = 0; i < new_Axe.Count; i++) {
            double step = Abs(new_Axe[i].val_Y - new_Axe[i].val_layer);
            H_Axe_Y.Insert(new_Axe[i].index, step);
            H_Axe_Y[new_Axe[i].index + 1] = Abs(H_Axe_Y[new_Axe[i].index + 1] - step);
        }

        // Корректировка шагов, чтобы не было узких прямоугольников
        List<int> index_rem = new List<int>();
        for (int i = 1; i < H_Axe_Y.Count - 1; i++) {
            if (H_Axe_Y[i] < min_step) {
                H_Axe_Y[i - 1] += H_Axe_Y[i];
                index_rem.Add(i);
            }
        }

        // Удлаение не подходящих шагов
        for (int i = 0; i < index_rem.Count; i++)
            H_Axe_Y.RemoveAt(index_rem[i]);
        H_Axe_Y.RemoveAt(0);


        // Учет границ X
        H_Axe_X.Insert(0, 0);
        double X_temp = copy_X_min;
        id = 1;
        new_Axe = new List<(int, double, double)>();
        for (int i = 0; i < H_Axe_X.Count && id != border_X.Count + 1; i++) {

            // Прибавляем шаг
            X_temp += H_Axe_X[i];

            // Если равно значит узлы будут стоять на узлы, переходим у следующему слою
            if (Abs(X_temp - border_X[^id]) <= 1e-6) {
                id++;
                continue;
            }

            // Если перескочили слой, значит добавим шаг в чтобы задеть слой
            if (X_temp > border_X[^id]) {
                new_Axe.Add((i + new_Axe.Count, border_X[^id], X_temp - H_Axe_X[i]));
                id++;
            }
        }

        // Корректировка шагов по Оси Y
        for (int i = 0; i < new_Axe.Count; i++) {
            double step = Abs(new_Axe[i].val_Y - new_Axe[i].val_layer);
            H_Axe_X.Insert(new_Axe[i].index, step);
            H_Axe_X[new_Axe[i].index + 1] = Abs(H_Axe_X[new_Axe[i].index + 1] - step);
        }

        // Корректировка шагов, чтобы не было узких прямоугольников
        index_rem = new List<int>();
        for (int i = 1; i < H_Axe_X.Count - 1; i++){
            if (H_Axe_X[i] < min_step){
                H_Axe_X[i - 1] += H_Axe_X[i];
                index_rem.Add(i);
            }
        }

        // Удлаение не подходящих шагов
        for (int i = 0; i < index_rem.Count; i++)
            H_Axe_X.RemoveAt(index_rem[i]);
        H_Axe_X.RemoveAt(0);


        // Генерация шагов по оси X от объекта
        double temp_V = copy_X_min;
        double temp_H = Hx;
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

        temp_V = X_max;
        temp_H = Hx;
        while (temp_V <= End_BG[0]) {
            temp_H *= Kx;
            H_Axe_X.Add(temp_H);
            temp_V += temp_H;
            if (temp_V >= End_BG[0] && IsStrictGrid) {
                temp_H -= (temp_V - End_BG[0]);
                H_Axe_X[^1] = temp_H;
            }
        }

        // Генерация шагов по оси Y от объекта
        temp_V = copy_Y_min;
        temp_H = Hy;
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

        temp_V = Y_max;
        temp_H = Hy;
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
            Y_temp = Y;
            id = 1;
            new_Axe = new List<(int, double, double)>();
            for (int i = 0; i < H_Axe_Y.Count && id != layers.Count + 1; i++) {

                // Прибавляем шаг
                Y_temp += H_Axe_Y[i];

                // Если равно значит узлы будут стоять на узлы, переходим у следующему слою
                if (Abs(Y_temp - layers[^id]) <= 1e-6) {
                    id++;
                    continue;
                }

                // Если перескочили слой, значит добавим шаг в чтобы задеть слой
                if (Y_temp > layers[^id]) {
                    new_Axe.Add((i + new_Axe.Count, layers[^id], Y_temp - H_Axe_Y[i]));
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

        // Корректировка шагов, чтобы не было узких прямоугольников
        index_rem = new List<int>();
        for (int i = 1; i < H_Axe_Y.Count - 1; i++) {
            if (H_Axe_Y[i] < e) {
                H_Axe_Y[i - 1] += H_Axe_Y[i];
                index_rem.Add(i);
            }
        }
        // Удлаение не подходящих шагов
        for (int i = 0; i < index_rem.Count; i++) {
            H_Axe_Y.RemoveAt(index_rem[i]);
        }

        // Сколько будет линий на графике (Количество узлов на Осях)
        CountX = H_Axe_X.Count;
        CountY = H_Axe_Y.Count;

        // Генерация узлов
        Node<double>[] nodes = new Node<double>[CountX * CountY];
        double Y_new = Y, X_new;
        id = 0;
        for (int i = 0; i < H_Axe_Y.Count; i++) {
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