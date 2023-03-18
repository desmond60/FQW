namespace FEM.grid;

// % ***** Структура конечного элемента ***** % //
public struct Elem
{
    //: Поля и свойства
    public int[] Node;                 // Номера узлов конечного элемента
    public int[] Edge;                 // Номера ребер конечного элемента
    public int Material { get; set; }  // Номер материала

    //: Конструктор
    public Elem(params int[] node) {
        Node = node;
    }

    //: Деконструктор
    public void Deconstruct(out int[] nodes,
                            out int[] edges) {
        nodes = Node;
        edges = Edge;
    }

    //: Строковое представление конечного элемента
    public override string ToString() {
        StringBuilder str_elem = new StringBuilder();
        str_elem.Append($"{Node[0]}");
        for (int i = 1; i < Node.Count(); i++)
            str_elem.Append($" {Node[i]}");
        for (int i = 0; i < Edge.Count(); i++)
            str_elem.Append($" {Edge[i]}");
        str_elem.Append($" {Material}");
        return str_elem.ToString();
    }
}