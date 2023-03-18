namespace FEM.grid;

// % ***** Структура узла ***** % //
public struct Node
{
    //: Поля и свойства
    public double X { get; set; }  // Координата X 
    public double Y { get; set; }  // Координата Y

    //: Конструктор
    public Node(double _X, double _Y) {
        (X, Y) = (_X, _Y);
    }

    //: Деконструктор
    public void Deconstruct(out double x,
                            out double y) {
        (x, y) = (X, Y);
    }

    //: Строковое представление узла
    public override string ToString() => $"{X} {Y}";
}