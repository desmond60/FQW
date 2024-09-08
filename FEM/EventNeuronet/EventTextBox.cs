namespace FEM;

//: Обработчики TextBox
public partial class Neuronet
{
    //: TextBox только цифры
    private void PreviewTextInputInt(object sender, TextCompositionEventArgs e)
    {
        var regex = new Regex("[^0-9]");
        e.Handled = regex.IsMatch(e.Text);
    }

    //: TextBox только цифры и точка и минус
    private void PreviewTextInputDouble(object sender, TextCompositionEventArgs e)
    {
        var regex = new Regex("[^0-9e.-]+");
        e.Handled = regex.IsMatch(e.Text);
    }
}
