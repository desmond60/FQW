namespace FEM;

// % ***** Класс МКЭ ***** % //
public class FEM
{
    //: Поля и свойства
    private List<Node>    Nodes;   // Узлы
    private List<Edge>    Edges;   // Ребра
    private List<Elem>    Elems;   // КЭ
    private List<Bound>   Bounds;  // Краевые
    private List<Item>    Items;   // Объекты
    private List<double>  Layers;  // Слои
    private SLAU          slau;    // Структура СЛАУ

    public Vector<double> Receivers { get; set; }   // Значения приемников
    public double         Nu        { get; set; }   // Частота тока
    public double         W => 2.0 * PI * Nu;       // Круговая частота

    //: Конструктор FEM
    public FEM(Grid grid) {
        (Nodes, Edges, Elems, Bounds, Items, Layers) = grid;
    }

    //: Метод составления СЛАУ
    public void CreateSLAU() {
    
    }

    //: Установка параметров для МКЭ
    public bool TrySetParameter(string name, Vector<double> value) {
        if (name == nameof(Receivers))  {
            Receivers = (Vector<double>)value.Clone();
            return true;
        }
        return false;
    }

    //: Установка параметров для МКЭ
    public bool TrySetParameter(string name, double value) {
        if (name == nameof(Nu)) {
            Nu = value;
            return true;
        }
        return false;
    }
}