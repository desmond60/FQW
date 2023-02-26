namespace FemField.other;

// % ***** Class Helper ***** % //
public static class Helper {

    //* Скалярное произведение векторов
    public static Complex Scalar(ComplexVector frst, ComplexVector scnd) {
        Complex res = 0;
        for (int i = 0; i < frst.Length; i++)
            res += frst[i]*scnd[i];
        return res;
    }

    //* Модуль комплексного вектора
    public static double Norm(ComplexVector vec) {
        double norm = 0;
        for (int i = 0; i < vec.Length; i++)
            norm += vec[i].Real*vec[i].Real + vec[i].Imaginary*vec[i].Imaginary;
        return Sqrt(norm);
    }

    //* Модуль комплексного числа
    public static double Norm(Complex ch) {
        return Sqrt(ch.Real*ch.Real + ch.Imaginary*ch.Imaginary);
    }

    //* Окно помощи при запуске (если нет аргументов или по команде)
    public static void ShowHelp() {
        WriteLine("----Команды----                        \n" + 
        "-help             - показать справку             \n" + 
        "-i                - входной файл                 \n");
    }
}