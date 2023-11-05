namespace FEM.slau;

// % ***** Структура СЛАУ ***** % //
public struct SLAU
{
    //: Поля и свойства
    public ComplexVector di, gg;   // Матрица
    public Vector<int> ig, jg;     // Массивы с индексами
    public ComplexVector pr, q;    // Правая часть и решение
    public int N;                  // Размерность матрицы

    public SLAU() { }

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
    public void WriteTXT(string path) {

        Directory.CreateDirectory(path);

        // Запись kuslau
        File.WriteAllText(path + "/kuslau.txt", N.ToString(), Encoding.UTF8);

        // Запись ig
        File.WriteAllText(path + "/ig.txt", String.Join("\n", ig.ToArray()), Encoding.UTF8);

        // Запись ig для портрета
        File.WriteAllText(path + "/ig_port.txt", String.Join(" ", ig.ToArray()));

        // Запись jg
        File.WriteAllText(path + "/jg.txt", String.Join("\n", jg.ToArray()), Encoding.UTF8);

        // Запись jg для портрета
        File.WriteAllText(path + "/jg_port.txt", String.Join(" ", jg.ToArray()));

        // Запись gg
        File.WriteAllText(path + "/gg.txt", String.Join("\n", gg.ToArray()), Encoding.UTF8);

        // Запись gg для портрета
        File.WriteAllText(path + "/gg_port.txt", String.Join(" ", gg.ToArray().Select(n => n.Real).ToArray()));

        // Запись di
        File.WriteAllText(path + "/di.txt", String.Join("\n", di.ToArray()), Encoding.UTF8);

        // Запись di для портрета
        File.WriteAllText(path + "/di_port.txt", String.Join(" ", di.ToArray().Select(n => n.Real).ToArray()));

        // Запись pr
        File.WriteAllText(path + "/pr.txt", String.Join("\n", pr.ToArray()), Encoding.UTF8);
    }

    //: Запись СЛАУ в бинарный формат
    public void WriteBIN(string path) {

        Directory.CreateDirectory(path);

        // Запись kuslau
        File.WriteAllText(path + "/kuslau", N.ToString());

        // Запись ig
        using (BinaryWriter writer = new BinaryWriter(File.Open(path + "/ig", FileMode.OpenOrCreate))) {
            foreach (int item in ig)
                writer.Write(item);
        }

        // Запись jg
        using (BinaryWriter writer = new BinaryWriter(File.Open(path + "/jg", FileMode.OpenOrCreate))) {
            foreach (int item in jg)
                writer.Write(item);
        }

        // Запись di
        using (BinaryWriter writer = new BinaryWriter(File.Open(path + "/di", FileMode.OpenOrCreate))) {
            foreach (Complex item in di) {
                writer.Write(item.Real);
                writer.Write(item.Imaginary);
            }
        }

        // Запись gg
        using (BinaryWriter writer = new BinaryWriter(File.Open(path + "/gg", FileMode.OpenOrCreate))) {
            foreach (Complex item in gg) {
                writer.Write(item.Real);
                writer.Write(item.Imaginary);
            }
        }

        // Запись pr
        using (BinaryWriter writer = new BinaryWriter(File.Open(path + "/pr", FileMode.OpenOrCreate))) {
            foreach (Complex item in pr) {
                writer.Write(item.Real);
                writer.Write(item.Imaginary);
            }
        }
    }
}
