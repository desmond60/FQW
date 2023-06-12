namespace FEM.other;

//: Статический класс "Помощник"
public static class Helper
{
    public static double Nu0 = 4 * PI * 1e-7; // Вакуумная магнитная проницаемость

    //: Метод проверки элемента в листе Double точность
    public static bool ListContains(List<Layer> list, double value) {
        bool result = false;

        foreach (var item in list) {
            if (Abs(item.Y - value) <= 1e-6)
                result = true;
        }

        return result;
    }

    //: Вычислить материал КЭ
    public static int GetMaterial(List<Layer> layers, List<Item> items, List<Node> nodes, Elem elem) {

        int mat = 1;

        // Середина элемента
        Node node = new Node(
           (nodes[elem.Node[0]].X + nodes[elem.Node[1]].X) / 2.0,
           (nodes[elem.Node[0]].Y + nodes[elem.Node[2]].Y) / 2.0
        );

        if (node.Y > 0.0) return mat;

        for (int i = 0; i < layers.Count; i++) {

            if (node.Y < layers[i].Y) mat++;

            // Ищем индексы объектов которые находятся в слое
            int[] id = GetIdItems(items, layers, i);

            // Проверяем объекты
            if (i != layers.Count - 1) {
                if (node.Y < layers[i + 1].Y) {
                    mat += id.Length;
                    continue;
                }
            }
            for (int j = 0; j < id.Length; j++) {
                if (node.X > items[id[j]].Begin[0] && node.X < items[id[j]].End[0] &&
                    node.Y > items[id[j]].Begin[1] && node.Y < items[id[j]].End[1])
                    return ++mat;
                else mat++;
            }

            if (i == layers.Count - 1)
                return mat - id.Length;
            else if (node.Y > layers[i + 1].Y)
                    return mat - id.Length;
        }

        return 0;
    }

    //: Найти объекты с пересечением слоя
    public static int[] GetIdItems(List<Item> items, List<Layer> layers, int index) {

        List<int> id = new List<int>();
        List<(int, Item)> id_item = new List<(int, Item)>();

        if (index == layers.Count - 1) {
            for (int i = 0; i < items.Count; i++)
                if (items[i].End[1] < layers[index].Y ||
                    (layers[index].Y > items[i].Begin[1] && layers[index].Y < items[i].End[1]))
                    id_item.Add((i, items[i]));

            id_item = id_item.OrderBy(n => n.Item2.Begin[0]).ToList();

            id = id_item.Select(n => n.Item1).ToList();

            return id.ToArray();
        }

        for (int i = 0; i < items.Count; i++)
            if ((items[i].End[1] < layers[index].Y && items[i].End[1] > layers[index + 1].Y) ||
                (items[i].Begin[1] < layers[index].Y && items[i].Begin[1] > layers[index + 1].Y) ||
                (layers[index].Y >= items[i].Begin[1] && layers[index].Y <= items[i].End[1]))
                id_item.Add((i, items[i]));

        id_item = id_item.OrderBy(n => n.Item2.Begin[0]).ToList();

        id = id_item.Select(n => n.Item1).ToList();

        return id.ToArray();
    }

    //: Скалярное произведение векторов
    public static Complex Scalar(ComplexVector frst, ComplexVector scnd) {
        Complex res = 0;
        for (int i = 0; i < frst.Length; i++)
            res += frst[i] * scnd[i];
        return res;
    }

    //: Модуль комплексного вектора
    public static double Norm(ComplexVector vec) {
        double norm = 0;
        for (int i = 0; i < vec.Length; i++)
            norm += vec[i].Real * vec[i].Real + vec[i].Imaginary * vec[i].Imaginary;
        return Sqrt(norm);
    }

    //: Модуль комплексного числа
    public static double Norm(Complex ch) {
        return Sqrt(ch.Real * ch.Real + ch.Imaginary * ch.Imaginary);
    }
}
