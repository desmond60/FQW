namespace FemField.grid;

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