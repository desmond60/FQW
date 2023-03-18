namespace FEM.slau;

// % ***** Структура СЛАУ ***** % //
public struct SLAU
{
    //: Поля и свойства
    public ComplexVector di, gg;   // Матрица
    public Vector<int> ig, jg;     // Массивы с индексами
    public ComplexVector pr, q;    // Правая часть и решение
    public ComplexVector q_abs;    // Абсолютные значения
    public int N;                  // Размерность матрицы

    //: Умножение матрицы на вектор
    public ComplexVector Mult(ComplexVector x) {
        ComplexVector result = new ComplexVector(N);
        for (int i = 0; i < N; i++) {
            result[i] = di[i] * x[i];
            for (int j = ig[i]; j < ig[i + 1]; j++) {
                result[i] += gg[j] * x[jg[j]];
                result[jg[j]] += gg[j] * x[i];
            }
        }
        return result;
    }
}
