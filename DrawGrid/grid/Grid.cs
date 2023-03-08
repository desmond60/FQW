namespace FEM.grid;

// % ***** Structure Grid ***** % //
public struct Grid<T> where T : System.Numerics.INumber<T>
{
    //: Fields and properties
    public int Count_Node { get; set; }    /// Общее количество узлов
    public int Count_Elem { get; set; }    /// Общее количество КЭ
    public int Count_Kraev { get; set; }    /// Количество краевых
    public int Count_Edge { get; set; }    /// Количество ребер

    public Node<T>[] Nodes;      /// Узлы
    public Elem[] Elems;      /// КЭ
    public Edge<T>[] Edges;      /// Ребра
    public Bound[] Bounds;     /// Краевые

    //: Constructor
    public Grid(Node<T>[] nodes, Edge<T>[] edges, Elem[] elem, Bound[] bounds)
    {
        Count_Node = nodes.Length;
        Count_Edge = edges.Length;
        Count_Elem = elem.Length;
        Count_Kraev = bounds.Length;
        Nodes = nodes;
        Edges = edges;
        Elems = elem;
        Bounds = bounds;
    }

    //: Deconstructor
    public void Deconstruct(out Node<T>[] nodes,
                            out Edge<T>[] edges,
                            out Elem[] elems,
                            out Bound[] kraevs)
    {
        edges = Edges;
        nodes = Nodes;
        elems = Elems;
        kraevs = Bounds;
    }
}

// % ***** Structure Node ***** % //
public struct Node<T> where T : System.Numerics.INumber<T>
{
    //: Fields and properties
    public T X { get; set; }  /// Coordinate X 
    public T Y { get; set; }  /// Coordinate Y

    //: Constructor
    public Node(T _X, T _Y)
    {
        (X, Y) = (_X, _Y);
    }

    //: Deconstructor
    public void Deconstruct(out T x,
                            out T y)
    {
        (x, y) = (X, Y);
    }

    //: String view structure
    public override string ToString() { return $"{X} {Y}"; }
}

// % ***** Structure Edge ***** % //
public struct Edge<T> where T : System.Numerics.INumber<T>
{
    //: Fields and properties
    public Node<T> NodeBegin { get; set; }  /// The begin node of the edge  
    public Node<T> NodeEnd { get; set; }  /// The end node of the edge

    //: Constructor
    public Edge(Node<T> _begin, Node<T> _end)
    {
        NodeBegin = _begin;
        NodeEnd = _end;
    }

    //: Deconstructor
    public void Deconstruct(out Node<T> begin,
                            out Node<T> end)
    {
        (begin, end) = (NodeBegin, NodeEnd);
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
    public Elem(params int[] node)
    {
        Node = node;
    }

    //: Deconstructor
    public void Deconstruct(out int[] nodes,
                            out int[] edges)
    {
        nodes = Node;
        edges = Edge;
    }

    //: String view structure
    public override string ToString()
    {
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
    public int Edge { get; set; }   /// Number edge
    public int NumBound { get; set; }   /// Number bound
    public int NumSide { get; set; }   /// Number side

    //: Constructor
    public Bound(int num, int side, int edge)
    {
        NumBound = num;
        NumSide = side;
        Edge = edge;
    }

    //: Deconstructor
    public void Deconstruct(out int num, out int side, out int edge)
    {
        num = NumBound;
        side = NumSide;
        edge = Edge;
    }

    //: String view structure
    public override string ToString()
    {
        StringBuilder str_elem = new StringBuilder();
        str_elem.Append($"{NumBound,0} {NumSide,3} {Edge,5}");
        return str_elem.ToString();
    }
}

// % ***** Structure Item ***** % //
public struct Item
{
    //: Fields and properties
    public Vector<double> Begin { get; set; }
    public Vector<double> End { get; set; }
    public int Nx { get; set; }
    public int Ny { get; set; }
    public string Name { get; set; }

    //: Constructor
    public Item(Vector<double> begin, Vector<double> end, int nx, int ny, string name = "None")
    {
        Begin = (Vector<double>)begin.Clone();
        End = (Vector<double>)end.Clone();
        Nx = nx;
        Ny = ny;
        Name = name;
    }
}
