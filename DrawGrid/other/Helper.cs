namespace DrawGrid.other;

//: Статический класс "Помощник"
public static class Helper
{

    //: Метод проверки элемента в листе Double точность
    public static bool ListContains(List<double> list, double value) {
        bool result = false;

        foreach (var item in list) {
            if (Abs(item - value) <= 1e-6)
                result = true;
        }

        return result;
    }
}
