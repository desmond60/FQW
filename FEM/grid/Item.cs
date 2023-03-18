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

    //: Конструктор
    public Item(Vector<double> begin, Vector<double> end, int nx, int ny, string name) {
        Begin = (Vector<double>)begin.Clone();
        End   = (Vector<double>)end.Clone();
        Nx    = nx;
        Ny    = ny;
        Name  = name;
    }

    //: Строковое представление объекта
    public override string ToString() => $"{Begin[0]} {Begin[1]} {End[0]} {End[1]} {Nx} {Ny} {Name}";
}