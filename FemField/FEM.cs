namespace FemField;

// % ***** Класс МКЭ ***** % //
public class FEM 
{
    //: Поля и свойства
    private Node<double>[]     Nodes;   /// Узлы
    private Edge<double>[]     Edges;   /// Ребра
    private Elem[]             Elems;   /// КЭ
    private Bound[]            Bounds;  /// Краевые
    private SLAU               slau;    /// Структура СЛАУ

    public string Path { get; set; }   /// Путь к задаче

    public Vector<double> Receivers { get; set; }   /// Значения приемников
    public double         Nu        { get; set; }   /// Частота тока
    public double         W => 2.0 * PI * Nu;       /// Круговая частота

    //: Конструктор uHJ
    public FEM(Grid<double> grid, Data data, string _path) {
        (Nodes, Edges, Elems, Bounds) = grid;
        (Receivers, Nu) = data;
        this.Path = _path;
    }

    //: Основной метод решения
    public void solve() {
        portrait();                                          //? Составление портрета матрицы
        //global();                                            //? Составление глобальной матрицы
        //LOS los = new LOS(slau, 1e-30, 1000, Path);          //? LOS
        //slau.q = los.solve(IsShowLos);                       //? Решение СЛАУ
        //AbsolutSolve();                                      //? Точное решение  
        //if (IsShowSlau) WriteToSlau();                       //? Записать СЛАУ
        //WriteToTable();                                      //? Записать решение
    }

    //: Составление портрета матрицы (ig, jg, выделение памяти)
    private void portrait() {
        Portrait port = new Portrait(Edges.Length);

        // Генерируем массивы ig и jg и размерность
        slau.N_el = port.GenPortrait(ref slau.ig, ref slau.jg, Elems);
        slau.N    = Edges.Length;

        // Выделяем память
        slau.ggl   = new ComplexVector(slau.N_el);
        slau.ggu   = new ComplexVector(slau.N_el);
        slau.di    = new ComplexVector(slau.N);
        slau.f     = new ComplexVector(slau.N);
        slau.q     = new ComplexVector(slau.N);
        slau.q_abs = new ComplexVector(slau.N);
    }

    //: Построение глобальной матрицы
    private void global() {

        // Обходим КЭ
        for (int index_fin_el = 0; index_fin_el < Elems.Length; index_fin_el++) {

            // Составляем локальную матрицу и локальный вектор
            (ComplexMatrix loc_mat, ComplexVector local_f) = local(index_fin_el);

            // Заносим в глобальную матрицу
            // EntryMatInGlobalMatrix(loc_mat, Elems[index_fin_el].Edge);
            // EntryVecInGlobalMatrix(local_f, Elems[index_fin_el].Edge);
        }

        // Обходим краевые
        // for (int index_kraev = 0; index_kraev < Bounds.Length; index_kraev++) {
        //     Bound kraev = Bounds[index_kraev];
        //     if (kraev.NumBound == 1)
        //         MainKraev(kraev); // главное краевое
        //     else if (kraev.NumBound == 2)
        //         NaturalKraev(kraev); // естественное краевое
        // }
    }

    //: Построение локальной матрицы и вектора
    private (ComplexMatrix, ComplexVector) local(int index_fin_el) {
        
        // Подсчет компонент
        double hx   = Nodes[Elems[index_fin_el].Node[1]].X - Nodes[Elems[index_fin_el].Node[0]].X;
        double hy   = Nodes[Elems[index_fin_el].Node[2]].Y - Nodes[Elems[index_fin_el].Node[0]].Y;
        
        Matrix<double> G           = build_G(index_fin_el, hx, hy);    // Построение матрицы жесткости (G)
        ComplexMatrix M            = build_M(index_fin_el, hx, hy);    // Построение матрицы массы (M)
        ComplexVector local_f      = build_F(index_fin_el, hx, hy);    // Построение локальной правой части
        //ComplexMatrix local_matrix = G + new Complex(0, 1) * M;

        return (M, local_f);
    }

    //: Построение матрицы жесткости (G)
    private Matrix<double> build_G(int index_fin_el, double hx, double hy) {
        
        // Подсчет коэффициентов
        double coef_y_on_x = hy / hx; 
        double coef_x_on_y = hx / hy;
        double coef_mu     = 1.0 / Nu;

        // Матрица жесткости
        var G_matrix = new Matrix<double>(new double[4, 4]{
            {1, -1, -1, 1},
            {-1, 1, 1, -1},
            {-1, 1, 1, -1},
            {1, -1, -1, 1}
        });

        // Умножение на coef_y_on_x
        for (int i = 0; i < 2; i++)
            for (int j = 0; j < 2; j++)
                G_matrix[i,j] *= coef_y_on_x;

        // Умножение на coef_x_on_y
        for (int i = 2; i < 4; i++)
            for (int j = 2; j < 4; j++)
                G_matrix[i,j] *= coef_x_on_y;

        return coef_mu * G_matrix;
    }

    //: Построение матрицы масс (M)
    private ComplexMatrix build_M(int index_fin_el, double hx, double hy) {
        
        // Подсчет коэффициента
        double coef = (W * Elems[index_fin_el].Sigma * hx * hy) / 6.0;

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
        
        // Вычисление f - на серединах ребер КЭ
        var f = new ComplexVector(4);
        for (int i = 0; i < f.Length; i++)
            f[i] = Func(Edges[Elems[index_fin_el].Edge[i]], Elems[index_fin_el].Sigma);
        
        return M_matrix * f;
    }
}

