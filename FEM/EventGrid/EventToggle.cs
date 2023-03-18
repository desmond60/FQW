namespace FEM;

//: Обработчики Toggle
public partial class MainWindow
{

    //: Обработка Toggle "Строгость сетки"
    private void ToggleStrict_Click(object sender, RoutedEventArgs e) {
        IsStrictGrid = !IsStrictGrid;
    }
}