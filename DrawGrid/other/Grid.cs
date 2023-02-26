namespace DrawGrid;

// % ***** Structure Grid ***** % //
public struct Grid<T> where T : System.Numerics.INumber<T>
{
    //: Fields and properties
    public int Count_Node { get; set; }    /// Общее количество узлов
    public int Count_Elem { get; set; }    /// Общее количество КЭ
    public int Count_Kraev { get; set; }    /// Количество краевых
    public int Count_Edge { get; set; }    /// Количество ребер

    public Node<T>[] Nodes;      /// Узлы
    public Elem[]    Elems;      /// КЭ
    public Edge<T>[] Edges;      /// Ребра
    public Bound[]   Bounds;     /// Краевые

    //: Constructor
    public Grid(Node<T>[] nodes, Edge<T>[] edges, Elem[] elem, Bound[] bounds) {
        this.Count_Node = nodes.Length;
        this.Count_Edge = edges.Length;
        this.Count_Elem = elem.Length;
        this.Count_Kraev = bounds.Length;
        this.Nodes = nodes;
        this.Edges = edges;
        this.Elems = elem;
        this.Bounds = bounds;
    }

    //: Deconstructor
    public void Deconstruct(out Node<T>[] nodes,
                            out Edge<T>[] edges,
                            out Elem[] elems,
                            out Bound[] kraevs) {
        edges  = this.Edges;
        nodes  = this.Nodes;
        elems  = this.Elems;
        kraevs = this.Bounds;
    }
}

// % ***** Structure Node ***** % //
public struct Node<T> where T : System.Numerics.INumber<T>
{
    //: Fields and properties
    public T X { get; set; }  /// Coordinate X 
    public T Y { get; set; }  /// Coordinate Y

    //: Constructor
    public Node(T _X, T _Y) {
        (this.X, this.Y) = (_X, _Y);
    }

    //: Deconstructor
    public void Deconstruct(out T x,
                            out T y) {
        (x, y) = (this.X, this.Y);
    }

    //: String view structure
    public override string ToString() { return $"{X} {Y}"; }
}

// % ***** Structure Edge ***** % //
public struct Edge<T> where T : System.Numerics.INumber<T>
{
    //: Fields and properties
    public Node<T> NodeBegin { get; set; }  /// The begin node of the edge  
    public Node<T> NodeEnd   { get; set; }  /// The end node of the edge

    //: Constructor
    public Edge(Node<T> _begin, Node<T> _end) {
        this.NodeBegin = _begin;
        this.NodeEnd = _end;
    }

    //: Deconstructor
    public void Deconstruct(out Node<T> begin,
                            out Node<T> end) {
        (begin, end) = (this.NodeBegin, this.NodeEnd);
    }

    //: String view structure
    public override string ToString() => $"{NodeBegin.ToString()} {NodeEnd.ToString()}";
}

// % ***** Structure Final Element ***** % //
public struct Elem
{
    //: Fields and properties
    public int[] Node;   /// Numbers node final element
    public int[] Edge;   /// Numbers edge final element

    //: Constructor
    public Elem(params int[] node) {
        this.Node = node;
    }

    //: Deconstructor
    public void Deconstruct(out int[] nodes,
                            out int[] edges) {
        nodes = this.Node;
        edges = this.Edge;
    }

    //: String view structure
    public override string ToString() {
        StringBuilder str_elem = new StringBuilder();
        str_elem.Append($"{Node[0],0}");
        for (int i = 1; i < Node.Count(); i++)
            str_elem.Append($"{Node[i],8}");
        str_elem.Append($"\t");
        for (int i = 0; i < Edge.Count(); i++)
            str_elem.Append($"{Edge[i],8}");
        return str_elem.ToString();
    }
}

// % ***** Structure Bound ***** % //
public struct Bound
{
    //: Fields and properties
    public int Edge     { get; set; }   /// Number edge
    public int NumBound { get; set; }   /// Number bound
    public int NumSide  { get; set; }   /// Number side

    //: Constructor
    public Bound(int num, int side, int edge) {
        this.NumBound = num;
        this.NumSide = side;
        this.Edge = edge;
    }

    //: Deconstructor
    public void Deconstruct(out int num, out int side, out int edge) {
        num  = this.NumBound;
        side = this.NumSide;
        edge = this.Edge;
    }

    //: String view structure
    public override string ToString() {
        StringBuilder str_elem = new StringBuilder();
        str_elem.Append($"{NumBound,0} {NumSide,3} {Edge,5}");
        return str_elem.ToString();
    }
}
