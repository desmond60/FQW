namespace FEM;

//: Класс МКЭ 
public class FEM
{
    //: Поля и свойства
    private List<Node>                             Nodes;   // Узлы
    private List<Edge>                             Edges;   // Ребра
    private List<Elem>                             Elems;   // КЭ
    private List<Bound>                            Bounds;  // Краевые
    private List<Item>                             Items;   // Объекты
    private List<Layer>                            Layers;  // Слои
    private List<(double Sigma1D, double Sigma2D)> Sigmas;  // Значения проводимости
    private SLAU slau;    // Структура СЛАУ

    public double         Nu        { get; set; }   // Частота тока
    public double         W => 2.0 * PI * Nu;       // Круговая частота
    public Harm1D         harm1D    { get; set; }   // Структура одномерной задачи

    //: Конструктор FEM
    public FEM(Grid grid, Harm1D harm1D) {
        (Nodes, Edges, Elems, Bounds, Items, Layers, Sigmas) = grid;
        this.harm1D = harm1D;
        this.slau = new SLAU();
    }

    //: Метод составления СЛАУ
    public SLAU CreateSLAU() {
        portrait();               // Составление портрета
        global();                 // Составление глобальной матрицы
        return slau;
    }

    //: Составление портрета
    private void portrait() {
        Portrait portrait = new Portrait(Edges.Count);

        // Генерируем массивы ig и jg и размерность
        portrait.GenPortrait(ref slau.ig, ref slau.jg, Elems.ToArray());
        slau.N = Edges.Count;

        // Выделяем память
        slau.gg    = new ComplexVector(slau.ig[slau.N]);
        slau.di    = new ComplexVector(slau.N);
        slau.pr    = new ComplexVector(slau.N);
        slau.q     = new ComplexVector(slau.N);
    }

    //: Составление глобальной матрицы
    private void global() {
        
        // Обходим конечные элементы
        for (int index_fin_el = 0; index_fin_el < Elems.Count; index_fin_el++) {

            // Составляем локальную матрицу и локальный вектор
            (ComplexMatrix loc_mat, ComplexVector local_f) = local(index_fin_el);

            // Заносим в глобальную матрицу
            EntryMatInGlobalMatrix(loc_mat, Elems[index_fin_el].Edge);
            EntryVecInGlobalMatrix(local_f, Elems[index_fin_el].Edge);
        }

        // Обходим краевые
        for (int index_kraev = 0; index_kraev < Bounds.Count; index_kraev++) {
            Bound kraev = Bounds[index_kraev];
            if (kraev.NumBound == 1)
                MainKraev(kraev); // главное краевое
            else if (kraev.NumBound == 2)
                NaturalKraev(kraev); // естественное краевое
        }
    }

    //: Построение локальной матрицы и вектора
    private (ComplexMatrix, ComplexVector) local(int index_fin_el) {

        // Подсчет компонент
        double hx = Nodes[Elems[index_fin_el].Node[1]].X - Nodes[Elems[index_fin_el].Node[0]].X;
        double hy = Nodes[Elems[index_fin_el].Node[2]].Y - Nodes[Elems[index_fin_el].Node[0]].Y;

        Matrix<double> G           = build_G(index_fin_el, hx, hy);   // Построение матрицы жесткости (G)
        ComplexMatrix M            = build_M(index_fin_el, hx, hy);   // Построение матрицы массы (M)
        ComplexVector local_f      = build_F(index_fin_el, hx, hy);   // Построение локальной правой части
        ComplexMatrix local_matrix = G + new Complex(0, 1) * M;       // Построение локальной матрицы

        return (local_matrix, local_f);
    }

    //: Построение матрицы жесткости (G)
    private Matrix<double> build_G(int index_fin_el, double hx, double hy) {

        // Подсчет коэффициентов
        double coef_y_on_x = hy / hx;
        double coef_x_on_y = hx / hy;
        double coef_nu = 1.0 / Nu;

        // Матрица жесткости
        var G_matrix = new Matrix<double>(new double[4, 4]{
            { 1, -1, -1,  1},
            {-1,  1,  1, -1},
            {-1,  1,  1, -1},
            { 1, -1, -1,  1}
        });

        // Умножение на coef_y_on_x
        for (int i = 0; i < 2; i++)
            for (int j = 0; j < 2; j++)
                G_matrix[i, j] *= coef_y_on_x;

        // Умножение на coef_x_on_y
        for (int i = 2; i < 4; i++)
            for (int j = 2; j < 4; j++)
                G_matrix[i, j] *= coef_x_on_y;

        return coef_nu * G_matrix;
    }

    //: Построение матрицы масс (M)
    private ComplexMatrix build_M(int index_fin_el, double hx, double hy) {

        // Подсчет коэффициента
        double coef = (W * Sigmas[Elems[index_fin_el].Material - 1].Sigma2D * hx * hy) / 6.0;

        // Матрица масс
        var M_matrix = new ComplexMatrix(new Complex[4, 4]{
            {2, 1, 0, 0},
            {1, 2, 0, 0},
            {0, 0, 2, 1},
            {0, 0, 1, 2}
        });

        return coef * M_matrix;
    }

    //: Построение вектора правой части (F)
    private ComplexVector build_F(int index_fin_el, double hx, double hy) {

        // Подсчет коэффициента
        double coef = (hx * hy) / 6.0;

        // Матрица масс
        var M_matrix = new ComplexMatrix(new Complex[4, 4]{
            {2, 1, 0, 0},
            {1, 2, 0, 0},
            {0, 0, 2, 1},
            {0, 0, 1, 2}
        });
        M_matrix = coef * M_matrix;

        // Вычисление вектора-потенциала
        Complex Fcoef = -new Complex(0, 1) * W * (Sigmas[Elems[index_fin_el].Material - 1].Sigma2D - Sigmas[Elems[index_fin_el].Material - 1].Sigma1D);
        var f = new ComplexVector(4);
        ComplexVector.Fill(f, new Complex(0, 0));

        if (Abs(Sigmas[Elems[index_fin_el].Material - 1].Sigma2D - Sigmas[Elems[index_fin_el].Material - 1].Sigma1D) <= 1e-10) return f;

        // Боковые ребра = (0,0)

        // Находим значение нижнего ребра
        f[2] = F(Edges[Elems[index_fin_el].Edge[2]]);

        // Находим значение верхнего ребра 
        f[3] = F(Edges[Elems[index_fin_el].Edge[3]]);

        // Умножение на коэффициент
        f = Fcoef * f;

        return M_matrix * f;
    }

    private Complex F(Edge edge) {

        // Строим узел
        Node node = edge.NodeBegin.Y == edge.NodeEnd.Y ?
             new Node((edge.NodeBegin.X + edge.NodeEnd.X) / 2.0, edge.NodeBegin.Y) :
             new Node(edge.NodeBegin.X, (edge.NodeBegin.Y + edge.NodeEnd.Y) / 2.0);

        // Находим узлы из одномернной задачи
        int id1 = 0, id2 = 0;
        for (int i = 0; i < harm1D.Nodes.Count - 1; i++) {
            if (node.Y >= harm1D.Nodes[i].Y && node.Y <= harm1D.Nodes[i + 1].Y) {
                id1 = i;
                id2 = i + 1;
            }
        }

        // Находим коэффициенты прямой
        Complex k = (harm1D.U[id2] - harm1D.U[id1]) / (harm1D.Nodes[id2].Y - harm1D.Nodes[id1].Y);
        Complex b = harm1D.U[id2] - k * harm1D.Nodes[id2].Y;

        return k * node.Y + b;
    }

    //: Занесение матрицы в глоабальную матрицу
    private void EntryMatInGlobalMatrix(ComplexMatrix mat, int[] index) {
        for (int i = 0, h = 0; i < mat.Rows; i++) {
            int ibeg = index[i];
            for (int j = i + 1; j < mat.Columns; j++) {
                int iend = index[j];
                int temp = ibeg;

                if (temp < iend)
                    (iend, temp) = (temp, iend);

                h = slau.ig[temp];
                while (slau.jg[h++] - iend != 0) ;
                --h;
                slau.gg[h] += mat[i, j];
            }
            slau.di[ibeg] += mat[i, i];
        }
    }

    //: Занесение вектора в глолбальный вектор
    private void EntryVecInGlobalMatrix(ComplexVector vec, int[] index) {
        for (int i = 0; i < vec.Length; i++)
            slau.pr[index[i]] += vec[i];
    }

    //: Учет главного краевого условия
    private void MainKraev(Bound bound) {

        // Номер ребра и значение краевого
        (int row, Complex value) = (bound.Edge, new Complex(0, 0));

        // Учет краевого
        slau.di[row] = new Complex(1, 0);
        slau.pr[row] = value;

        // Зануляем в треугольнике (столбцы)
        for (int i = slau.ig[row]; i < slau.ig[row + 1]; i++) {
            slau.pr[slau.jg[i]] -= slau.gg[i] * value;
            slau.gg[i] = 0;
        }

        // Зануляем в треугольнике (строки)
        for (int i = row + 1; i < slau.N; i++) {
            for (int j = slau.ig[i]; j < slau.ig[i + 1]; j++) {
                if (slau.jg[j] == row) {
                    slau.pr[i] -= slau.gg[j] * value;
                    slau.gg[j] = 0;
                }
            }
        }
    }

    //: Учет естественного краевого условия
    private void NaturalKraev(Bound kraev) { }

    //: Установка параметров для МКЭ
    public bool TrySetParameter(string name, double value) {
        if (name == nameof(Nu)) {
            Nu = value;
            return true;
        }
        return false;
    }
}