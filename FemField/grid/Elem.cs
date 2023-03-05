namespace FemField.grid;

// % ***** Structure Final Element ***** % //
public struct Elem
{
    //: Fields and properties
    public int[]  Node;   /// Numbers node final element
    public int[]  Edge;   /// Numbers edge final element
    public int    NumMaterial { get; set; }   /// Номер материала
    public double Sigma       { get; set; }   /// Значение Sigma

    //: Constructor
    public Elem(params int[] node) { 
        this.Node  = node; 
    }

    //: Deconstructor
    public void Deconstruct(out int[] nodes) { nodes = this.Node; }

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