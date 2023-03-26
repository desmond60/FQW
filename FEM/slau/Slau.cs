namespace FEM.slau;

// % ***** Структура СЛАУ ***** % //
public struct SLAU
{
    //: Поля и свойства
    public ComplexVector di, gg;   // Матрица
    public Vector<int> ig, jg;     // Массивы с индексами
    public ComplexVector pr, q;    // Правая часть и решение
    public int N;                  // Размерность матрицы

    private string PathTXT { get; set; }
    private string PathBIN { get; set; }

    public SLAU() {
        this.PathTXT = @"slau\slauTXT";
        this.PathBIN = @"slau\slauBIN";
    }

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

    //: Запись СЛАУ в текстовый формат
    public void WriteTXT() {

        // Запись kuslau
        File.WriteAllText(PathTXT + "/kuslau.txt", N.ToString(), Encoding.UTF8);

        // Запись ig
        File.WriteAllText(PathTXT + "/ig.txt", String.Join("\n", ig.ToArray()), Encoding.UTF8);

        // Запись jg
        File.WriteAllText(PathTXT + "/jg.txt", String.Join("\n", jg.ToArray()), Encoding.UTF8);

        // Запись gg
        File.WriteAllText(PathTXT + "/gg.txt", String.Join("\n", gg.ToArray()), Encoding.UTF8);

        // Запись di
        File.WriteAllText(PathTXT + "/di.txt", String.Join("\n", di.ToArray()), Encoding.UTF8);

        // Запись pr
        File.WriteAllText(PathTXT + "/pr.txt", String.Join("\n", pr.ToArray()), Encoding.UTF8);
    }

    //: Запись СЛАУ в бинарный формат
    public void WriteBIN() {

        // Запись kuslau
        File.WriteAllText(PathBIN + "/kuslau", N.ToString());

        // Запись ig
        using (BinaryWriter writer = new BinaryWriter(File.Open(PathBIN + "/ig", FileMode.OpenOrCreate))) {
            foreach (int item in ig)
                writer.Write(item);
        }

        // Запись jg
        using (BinaryWriter writer = new BinaryWriter(File.Open(PathBIN + "/jg", FileMode.OpenOrCreate))) {
            foreach (int item in jg)
                writer.Write(item);
        }

        // Запись di
        using (BinaryWriter writer = new BinaryWriter(File.Open(PathBIN + "/di", FileMode.OpenOrCreate))) {
            foreach (Complex item in di) {
                writer.Write(item.Real);
                writer.Write(item.Imaginary);
            }
        }

        // Запись gg
        using (BinaryWriter writer = new BinaryWriter(File.Open(PathBIN + "/gg", FileMode.OpenOrCreate))) {
            foreach (Complex item in gg) {
                writer.Write(item.Real);
                writer.Write(item.Imaginary);
            }
        }

        // Запись pr
        using (BinaryWriter writer = new BinaryWriter(File.Open(PathBIN + "/pr", FileMode.OpenOrCreate))) {
            foreach (Complex item in pr) {
                writer.Write(item.Real);
                writer.Write(item.Imaginary);
            }
        }
    }
}
