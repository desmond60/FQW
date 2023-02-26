namespace FemField.grid;

// % ***** Structure Grid ***** % //
public struct Grid<T> where T : System.Numerics.INumber<T>
{
    //: Fields and properties
    public int Count_Node  { get; set; }
    public int Count_Elem  { get; set; }
    public int Count_Kraev { get; set; }
    public int Count_Edge  { get; set; }

    public Node<T>[]  Nodes;
    public Elem[]     Elems;
    public Edge<T>[]  Edges;
    public Bound[]    Bounds;

    //: Constructor
    public Grid(Node<T>[] nodes, Edge<T>[] edges, Elem[] elem, Bound[] bounds) {
        this.Count_Node  = nodes.Length;
        this.Count_Edge  = edges.Length;
        this.Count_Elem  = elem.Length;
        this.Count_Kraev = bounds.Length;
        this.Nodes       = nodes;
        this.Edges       = edges;
        this.Elems       = elem;
        this.Bounds      = bounds;
    }

    //: Deconstructor
    public void Deconstruct(out Node<T>[]  nodes,
                            out Edge<T>[]  edges,
                            out Elem[]     elems,
                            out Bound[]    kraevs) {
        edges  = this.Edges;
        nodes  = this.Nodes;
        elems  = this.Elems;
        kraevs = this.Bounds;
    }
}