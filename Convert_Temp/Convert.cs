namespace NConvert;

// % ***** Структура СЛАУ ***** % //
public struct SLAU
{
    //: Поля и свойства
    public double[] di, ggl, ggu;   /// Матрица
    public int[] ig, jg;            /// Массивы с индексами
    public double[] f, q;           /// Правая часть и решение
    public double[] q_abs;          /// Абсолютные значения U-функции
    public int N;                   /// Размерность матрицы
    public int N_el;                /// Размерность gl и gu
}

public class Convert
{

    public SLAU slau;
    public string Path { get; set; }

    public Convert(string path) {
        this.Path = path;
        
        this.slau = Read_Files(this.Path);
    }

    public SLAU Read_Files(string path) {

        SLAU slau = new SLAU(); 

        slau.N = int.Parse(File.ReadAllText(path + "kuslau2.txt"));
        slau.ig = File.ReadAllText(path + "ig.txt").Split(" ").Select(int.Parse).ToArray();
        slau.jg = File.ReadAllText(path + "jg.txt").Split(" ").Select(int.Parse).ToArray();
        slau.di = File.ReadAllText(path + "di.txt").Split(" ").Select(double.Parse).ToArray();
        slau.ggl = File.ReadAllText(path + "gg.txt").Split(" ").Select(double.Parse).ToArray();
        slau.f = File.ReadAllText(path + "pr.txt").Split(" ").Select(double.Parse).ToArray();

        return slau;
    }

    public void Convertio() {
        
        string path = Path + "slau\\";
        Directory.CreateDirectory(path);

        // kuslau2
        using (BinaryWriter writer = new BinaryWriter(File.Open(path + "kuslau", FileMode.OpenOrCreate)))
        {
            writer.Write(slau.N);
            writer.Write(9.999999682655226e-021);
            writer.Write(10000);
        }

        // di
        using (BinaryWriter writer = new BinaryWriter(File.Open(path + "di", FileMode.OpenOrCreate)))
        {
            foreach (var item in slau.di)
                writer.Write(item);
        }

        // gg
        using (BinaryWriter writer = new BinaryWriter(File.Open(path + "gg", FileMode.OpenOrCreate)))
        {
            foreach (var item in slau.ggl)
                writer.Write(item);
        }

        // ig
        using (BinaryWriter writer = new BinaryWriter(File.Open(path + "ig", FileMode.OpenOrCreate)))
        {
            foreach (var item in slau.ig)
                writer.Write(item);
        }

        // jg
        using (BinaryWriter writer = new BinaryWriter(File.Open(path + "jg", FileMode.OpenOrCreate)))
        {
            foreach (var item in slau.jg)
                writer.Write(item);
        }

        // pr
        using (BinaryWriter writer = new BinaryWriter(File.Open(path + "pr", FileMode.OpenOrCreate)))
        {
            foreach (var item in slau.f)
                writer.Write(item);
        }


    }
}