namespace FEM.other;

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

    //: Определение номера материала
    public static int GetNumberMaterial(List<double> layers, double value) {
        int result = 1;

        for (int i = 0; i < layers.Count; i++) {
            if (value > 0) return 1;
            if (value < layers[i]) result++;
        }

        return result;
    }
}
