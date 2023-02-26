namespace FemField.grid;

// % ***** Structure Bound ***** % //
public struct Bound
{
    //: Fields and properties
    public int      Edge     { get; set; }   /// Number edge
    public int      NumBound { get; set; }   /// Number bound
    public int      NumSide  { get; set; }   /// Number side
    
    //: Constructor
    public Bound(int num, int side, int edge) { 
        this.NumBound = num;
        this.NumSide  = side;
        this.Edge     = edge;
    }

    //: Deconstructor
    public void Deconstruct(out int num, out int side, out int edge) { 
        num   = this.NumBound;
        side  = this.NumSide;
        edge  = this.Edge;
    }

    //: String view structure
    public override string ToString() {
        StringBuilder str_elem = new StringBuilder();
        str_elem.Append($"{NumBound,0} {NumSide,3} {Edge,5}");
        return str_elem.ToString();
    }
}