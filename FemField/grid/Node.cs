namespace FemField.grid;

// % ***** Structure Node ***** % //
public struct Node<T> where T : System.Numerics.INumber<T>
{
    //: Fields and properties
    public T  X { get; set; }  /// Coordinate X 
    public T  Y { get; set; }  /// Coordinate Y

    //: Constructor
    public Node(T _X, T _Y) {
        (this.X, this.Y) = (_X, _Y);
    }

    //: Deconstructor
    public void Deconstruct(out T x, 
                            out T y) 
    {
        (x, y) = (this.X, this.Y);
    }

    //: String view structure
    public override string ToString() { return $"{X} {Y}"; }
}