namespace FemField;

// % ***** Data class for JSON ***** % //
public class Data
{
    //: Данные для сетки
    public double[] Begin_BG     { get; set; }   /// Начальная точка большого поля
    public double[] End_BG       { get; set; }   /// Конечная точка большого поля
    public double[] Begin_SML    { get; set; }   /// Начальная точка маленького поля
    public double[] End_SML      { get; set; }   /// Конечная точка маленького поля
    public double   Nx           { get; set; }   /// Количество узлов маленького поля по Оси X
    public double   Ny           { get; set; }   /// Количество узлов маленького поля по Оси Y
    public double   Kx           { get; set; }   /// Коэффициент разрядки по Оси X
    public double   Ky           { get; set; }   /// Коэффициент разрядки по Оси Y
    public bool     IsStrictGrid { get; set; }   /// Строгость сетки
    public double[] Layers       { get; set; }   /// Горизонтальные слои сетки
    public int[]    Bound        { get; set; }   /// Номера краевых на сторонах (н, п, в, л)

    //: Данные для задачи
    public double[] Receivers { get; set; }   /// Значения приемников
    public double   Nu        { get; set; }   /// Частота тока

    //: Деконструктор (для сетки)
    public void Deconstruct(out Vector<double> begin_bg,
                            out Vector<double> end_bg,
                            out Vector<double> begin_sml,
                            out Vector<double> end_sml,
                            out Vector<double> layers,
                            out double nx, 
                            out double ny, 
                            out double kx, 
                            out double ky,
                            out bool   isStrict) 
    {
        begin_bg  = new Vector<double>(this.Begin_BG);
        end_bg    = new Vector<double>(this.End_BG);
        begin_sml = new Vector<double>(this.Begin_SML);
        end_sml   = new Vector<double>(this.End_SML);
        layers    = new Vector<double>(this.Layers);
        nx        = this.Nx;
        ny        = this.Ny;
        kx        = this.Kx;
        ky        = this.Ky;
        isStrict  = this.IsStrictGrid;
    }

    //: Деконструктор (для задачи)
    public void Deconstruct(out Vector<double> receivers,
                            out double nu) {
        receivers = new Vector<double>(this.Receivers);
        nu = this.Nu;
    }
}