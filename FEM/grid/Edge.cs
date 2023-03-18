namespace FEM.grid;

// % ***** Структура ребра ***** % //
public struct Edge
{
    //: Поля и свойства
    public Node NodeBegin { get; set; }  // Начальный узел ребра
    public Node NodeEnd   { get; set; }  // Конечный узел ребра

    //: Конструктор
    public Edge(Node _begin, Node _end) {
        NodeBegin = _begin;
        NodeEnd = _end;
    }

    //: Деконструктор
    public void Deconstruct(out Node begin,
                            out Node end) {
        (begin, end) = (NodeBegin, NodeEnd);
    }

    //: Строковое представление ребра
    public override string ToString() => $"{NodeBegin} {NodeEnd}";
}