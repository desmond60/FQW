namespace FEM.grid;

// % ***** Структура слоя ***** % //
public struct Layer
{
    //: Поля и свойства
    public double Y     { get; set; }  // Координата слоя
    public double Sigma { get; set; }  // Значение проводимости
    
    //: Конструктор
    public Layer(double y, double sigma) {
        this.Y = y;
        this.Sigma = sigma;
    }

    //: Строковое представление объекта
    public override string ToString() => $"{Y} {Sigma}";
}