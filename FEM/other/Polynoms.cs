namespace FEM;

// % ***** Класс с полиномами ***** % //
public static class Polynoms
{
    public static int numberFunc = 3; // Номер полинома

    //: Заданная функция вектор (нахождение через ребро)
    public static Complex Absolut(Edge edge)
    {

        // Определение компоненты вектора и узла
        char axe;
        Node node = new Node();
        if (edge.NodeBegin.Y == edge.NodeEnd.Y) {
            axe = 'x';
            node.X = (edge.NodeBegin.X + edge.NodeEnd.X) / 2.0;
            node.Y = edge.NodeBegin.Y;
        }
        else {
            axe = 'y';
            node.X = edge.NodeBegin.X;
            node.Y = (edge.NodeBegin.Y + edge.NodeEnd.Y) / 2.0;
        }

        return Absolut(node, axe);
    }

    //: Заданная функция вектор (нахождение через узел)
    public static Complex Absolut(Node node, char axe) {

        switch (numberFunc)
        {
            // Разное местоположение второго краевого
            case 1:
                return axe switch
                {
                    'x' => new Complex(2 * node.X + 3 * node.Y, 6 * node.X + 7 * node.Y),
                    'y' => new Complex(3 * node.X - 2 * node.Y, node.X + node.Y),
                    _ => new Complex(0, 0)
                };

            // Полином второй степени
            case 2:
                return axe switch
                {
                    'x' => new Complex(2 * Pow(node.X, 2) + 3 * Pow(node.Y, 2), 6 * Pow(node.X, 2) + 7 * Pow(node.Y, 2)),
                    'y' => new Complex(3 * Pow(node.X, 2) - 2 * Pow(node.Y, 2), Pow(node.X, 2) + Pow(node.Y, 2)),
                    _ => new Complex(0, 0)
                };

            // Полином третьей степени
            case 3:
                return axe switch
                {
                    'x' => new Complex(2 * Pow(node.X, 3) + 3 * Pow(node.Y, 3), 6 * Pow(node.X, 3) + 7 * Pow(node.Y, 3)),
                    'y' => new Complex(3 * Pow(node.X, 3) - 2 * Pow(node.Y, 3), Pow(node.X, 3) + Pow(node.Y, 3)),
                    _ => new Complex(0, 0)
                };

            // Полином четвертой степени
            case 4:
                return axe switch
                {
                    'x' => new Complex(2 * Pow(node.X, 4) + 3 * Pow(node.Y, 4), 6 * Pow(node.X, 4) + 7 * Pow(node.Y, 4)),
                    'y' => new Complex(3 * Pow(node.X, 4) - 2 * Pow(node.Y, 4), Pow(node.X, 4) + Pow(node.Y, 4)),
                    _ => new Complex(0, 0)
                };

            default:
                return 0;
        }
    }

    //: Вектор-функция правой части
    public static Complex Func(Edge edge, Complex coef, double nu)
    {

        // Определение компоненты вектора и узла
        char axe;
        Node node = new Node();
        if (edge.NodeBegin.Y == edge.NodeEnd.Y)
        {
            axe = 'x';
            node.X = (edge.NodeBegin.X + edge.NodeEnd.X) / 2.0;
            node.Y = edge.NodeBegin.Y;
        }
        else
        {
            axe = 'y';
            node.X = edge.NodeBegin.X;
            node.Y = (edge.NodeBegin.Y + edge.NodeEnd.Y) / 2.0;
        }

        // Коеффициент второго слагаемого уравнения
        Complex sigma_omega_A = coef * Absolut(node, axe);

        switch (numberFunc)
        {
            // Разное местоположение второго краевого
            case 1:
                return axe switch
                {
                    'x' => 1 / nu * new Complex(0, 0) + sigma_omega_A,
                    'y' => 1 / nu * new Complex(0, 0) + sigma_omega_A,
                    _ => new Complex(0, 0)
                };

            // Полином второй степени
            case 2:
                return axe switch
                {
                    'x' => 1 / nu * new Complex(-6, -14) + sigma_omega_A,
                    'y' => 1 / nu * new Complex(-6, -2) + sigma_omega_A,
                    _ => new Complex(0, 0)
                };

            // Полином третьей степени
            case 3:
                return axe switch
                {
                    'x' => 1 / nu * new Complex(-18 * node.Y, -42 * node.Y) + sigma_omega_A,
                    'y' => 1 / nu * new Complex(-18 * node.X, -6 * node.X) + sigma_omega_A,
                    _ => new Complex(0, 0)
                };

            // Полином четвертой степени
            case 4:
                return axe switch
                {
                    'x' => 1 / nu * new Complex(-36 * Pow(node.Y, 2), -84 * Pow(node.Y, 2)) + sigma_omega_A,
                    'y' => 1 / nu * new Complex(-36 * Pow(node.X, 2), -12 * Pow(node.X, 2)) + sigma_omega_A,
                    _ => new Complex(0, 0)
                };

            default:
                return 0;
        }
    }

}