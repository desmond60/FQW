namespace DrawGrid;

//: Обработчики TextBox
public partial class MainWindow : Window {

    //: TextBox только цифры и точка и минус
    private void PreviewTextInput(object sender, TextCompositionEventArgs e) {
        // Добавляем регулярное выражение
        var regex = new Regex("[^0-9.-]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    //: TextBox только цифры
    private void PreviewTextInputInt(object sender, TextCompositionEventArgs e) {
        // Добавляем регулярное выражение
        var regex = new Regex("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    //: TextBox только цифры
    private void PreviewTextInputIntBounds(object sender, TextCompositionEventArgs e) {
        // Добавляем регулярное выражение
        var regex = new Regex("[^1-3]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    //: TextBox только англ. буквы и цифры
    private void PreviewTextInputName(object sender, TextCompositionEventArgs e) {
        // Добавляем регулярное выражение
        var regex = new Regex("[^a-z A-Z 1-9]+");
        e.Handled = regex.IsMatch(e.Text);
    }
}


