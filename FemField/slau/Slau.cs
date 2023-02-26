namespace FemField.slau;

// % ***** Структура СЛАУ ***** % //
public struct SLAU
{
    //: Поля и свойства
    public ComplexVector di, ggl, ggu;   /// Матрица
    public Vector<int> ig, jg;           /// Массивы с индексами
    public ComplexVector f, q;           /// Правая часть и решение
    public ComplexVector q_abs;          /// Абсолютные значения U-функции
    public int N;                        /// Размерность матрицы
    public int N_el;                     /// Размерность gl и gu

    //: Умножение матрицы на вектор
    public ComplexVector Mult(ComplexVector x) {
        ComplexVector result = new ComplexVector(N);
        for (int i = 0; i < N; i++) {
            result[i] = di[i]*x[i];
            for (int j = ig[i]; j < ig[i + 1]; j++) {
                result[i]      += ggl[j]*x[jg[j]];
                result[jg[j]]  += ggu[j]*x[i];
            }
        }
        return result;
    }
}