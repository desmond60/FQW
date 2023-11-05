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

    //: Обработчик изменения TextBox "NuBox"
    private void NuBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (double.TryParse(NuBox.Text, out double Nu) && WBox is not null)
        {
            double W = 2 * Math.PI * Nu;
            WBox.Text = $"({W.ToString("F4")})";
        }
    }
}
