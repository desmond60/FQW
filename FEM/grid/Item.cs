namespace FEM.grid;

// % ***** Структура объекта ***** % //
public struct Item
{
    //: Поля и свойства
    public Vector<double> Begin { get; set; }  // Нижняя-Левая точка объекта
    public Vector<double> End   { get; set; }  // Верхняя-Правая точка объекта
    public int            Nx    { get; set; }  // Количество разбиений по Оси X
    public int            Ny    { get; set; }  // Количество разбиений по Оси Y
    public string         Name  { get; set; }  // Имя объекта
    public double         Sigma { get; set; }  // Значение проводимости

    //: Конструктор
    public Item(Vector<double> begin, Vector<double> end, int nx, int ny, double sigma, string name) {
        this.Begin = (Vector<double>)begin.Clone();
        this.End   = (Vector<double>)end.Clone();
        this.Nx    = nx;
        this.Ny    = ny;
        this.Sigma = sigma;
        this.Name  = name;
    }

    //: Строковое представление объекта
    public override string ToString() => $"{Begin[0]} {Begin[1]} {End[0]} {End[1]} {Nx} {Ny} {Sigma} {Name}";
}