namespace FEM.grid;

// % ***** Структура краевого ***** % //
public struct Bound
{
    //: Поля и свойства
    public int Edge     { get; set; }   // Номер ребра
    public int NumBound { get; set; }   // Номер краевого
    public int NumSide  { get; set; }   // Номер стороны

    //: Конструктор
    public Bound(int num, int side, int edge) {
        NumBound = num;
        NumSide = side;
        Edge = edge;
    }

    //: Деконструктор
    public void Deconstruct(out int num, out int side, out int edge) {
        num = NumBound;
        side = NumSide;
        edge = Edge;
    }

    //: Строковое представление краевого
    public override string ToString() => $"{NumBound} {NumSide} {Edge}";
}
