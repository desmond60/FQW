using Numerics;
using Complex = System.Numerics.Complex;
using System.Globalization;

namespace NConvert;

// % ***** Структура СЛАУ double ***** % //
// public struct SLAU
// {
//     //: Поля и свойства
//     public double[] di, gg;        // Матрица
//     public int[] ig, jg;           // Массивы с индексами
//     public double[] pr, q;         // Правая часть и решение
//     public int N;                  // Размерность матрицы
//     public int N_el;               // Размерность нижнего треугольника
// }

// % ***** Структура СЛАУ Complex ***** % //
public struct SLAU
{
    //: Поля и свойства
    public ComplexVector di, gg;   // Матрица
    public Vector<int> ig, jg;     // Массивы с индексами
    public ComplexVector pr, q;    // Правая часть и решение
    public int N;                  // Размерность матрицы
    public int N_el;               // Размерность нижнего треугольника
}

public class Convert
{

    public SLAU slau;
    public string Path { get; set; }

    public Convert(string path) {
        this.Path = path;
        
        // double
        //this.slau = Read_Files(this.Path);

        // Complex
        this.slau = Read_Files_Complex(this.Path);
    }

    public SLAU Read_Files(string path) {

        SLAU slau = new SLAU();
        
        // slau.N = int.Parse(File.ReadAllText(path + "kuslau2.txt"));
        // slau.ig = File.ReadAllText(path + "ig.txt").Split(" ").Select(int.Parse).ToArray();
        // slau.jg = File.ReadAllText(path + "jg.txt").Split(" ").Select(int.Parse).ToArray();
        // slau.di = File.ReadAllText(path + "di.txt").Split(" ").Select(Complex.TryParse).ToArray();
        // slau.gg = File.ReadAllText(path + "gg.txt").Split(" ").Select(double.Parse).ToArray();
        // slau.pr = File.ReadAllText(path + "pr.txt").Split(" ").Select(double.Parse).ToArray();

        return slau;
    }

    public SLAU Read_Files_Complex(string path) {
        
        SLAU slau = new SLAU();

        slau.N = int.Parse(File.ReadAllText(path + "kuslau.txt"));
        slau.ig = File.ReadAllText(path + "ig.txt").Split(" ").Select(int.Parse).ToArray();
        slau.jg = File.ReadAllText(path + "jg.txt").Split(" ").Select(int.Parse).ToArray();

        string[] di_str = File.ReadAllText(path + "di.txt").Split(" ");
        slau.di = new ComplexVector(di_str.Length / 2);
        for (int i = 0, id = 0; i < di_str.Length - 1; i += 2, id++) {
            slau.di[id] = new Complex(double.Parse(di_str[i]), double.Parse(di_str[i + 1]));
        }

        string[] gg_str = File.ReadAllText(path + "gg.txt").Split(" ");
        slau.gg = new ComplexVector(gg_str.Length / 2);
        for (int i = 0, id = 0; i < gg_str.Length - 1; i += 2, id++) {
            slau.gg[id] = new Complex(double.Parse(gg_str[i]), double.Parse(gg_str[i + 1]));
        }

        string[] pr_str = File.ReadAllText(path + "pr.txt").Split(" ");
        slau.pr = new ComplexVector(pr_str.Length / 2);
        for (int i = 0, id = 0; i < pr_str.Length - 1; i += 2, id++) {
            slau.pr[id] = new Complex(double.Parse(pr_str[i]), double.Parse(pr_str[i + 1]));
        }

        return slau;
    }

    public void Convertio() {
        
        // string path = Path + "slau\\";
        // Directory.CreateDirectory(path);

        // // kuslau2
        // using (BinaryWriter writer = new BinaryWriter(File.Open(path + "kuslau", FileMode.OpenOrCreate)))
        // {
        //     writer.Write(slau.N);
        //     writer.Write(9.999999682655226e-021);
        //     writer.Write(10000);
        // }

        // // ig
        // using (BinaryWriter writer = new BinaryWriter(File.Open(path + "ig", FileMode.OpenOrCreate)))
        // {
        //     foreach (var item in slau.ig)
        //         writer.Write(item);
        // }

        // // jg
        // using (BinaryWriter writer = new BinaryWriter(File.Open(path + "jg", FileMode.OpenOrCreate)))
        // {
        //     foreach (var item in slau.jg)
        //         writer.Write(item);
        // }

        // // di
        // using (BinaryWriter writer = new BinaryWriter(File.Open(path + "di", FileMode.OpenOrCreate)))
        // {
        //     foreach (var item in slau.di)
        //         writer.Write(item);
        // }

        // // gg
        // using (BinaryWriter writer = new BinaryWriter(File.Open(path + "gg", FileMode.OpenOrCreate)))
        // {
        //     foreach (var item in slau.ggl)
        //         writer.Write(item);
        // }

        // // pr
        // using (BinaryWriter writer = new BinaryWriter(File.Open(path + "pr", FileMode.OpenOrCreate)))
        // {
        //     foreach (var item in slau.f)
        //         writer.Write(item);
        // }
    }

    public void Convertio_Complex() {
        string path = Path + "slau\\";
        Directory.CreateDirectory(path);

        // kuslau2 - ПОМЕНЯТЬ НА ОБЫЧНЫЙ ФАЙЛ!!!!!!
        File.WriteAllText(path + "kuslau", slau.N.ToString());

        // ig
        using (BinaryWriter writer = new BinaryWriter(File.Open(path + "ig", FileMode.OpenOrCreate)))
        {
            foreach (int item in slau.ig)
                writer.Write(item);
        }

        // jg
        using (BinaryWriter writer = new BinaryWriter(File.Open(path + "jg", FileMode.OpenOrCreate)))
        {
            foreach (int item in slau.jg)
                writer.Write(item);
        }

        // di
        using (BinaryWriter writer = new BinaryWriter(File.Open(path + "di", FileMode.OpenOrCreate)))
        {
            foreach (Complex item in slau.di) {
                writer.Write(item.Real);
                writer.Write(item.Imaginary);
            }
        }

        // gg
        using (BinaryWriter writer = new BinaryWriter(File.Open(path + "gg", FileMode.OpenOrCreate)))
        {
            foreach (Complex item in slau.gg) {
                writer.Write(item.Real);
                writer.Write(item.Imaginary);
            }
        }

        // pr
        using (BinaryWriter writer = new BinaryWriter(File.Open(path + "pr", FileMode.OpenOrCreate)))
        {
            foreach (Complex item in slau.pr) {
                writer.Write(item.Real);
                writer.Write(item.Imaginary);
            }
        }
    }
}