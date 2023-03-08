namespace FEM;

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

    //: Обработка выбора в списке объектов
    private void itemsList_SelectionChanged(object sender, SelectionChangedEventArgs e) {

        if ((string)itemsList.SelectedValue != null) {
            string name = (string)itemsList.SelectedValue;

            Item item = items.Find(n => n.Name.Equals(name));

            Begin_SML_X.Text = item.Begin[0].ToString();
            Begin_SML_Y.Text = item.Begin[1].ToString();
            End_SML_X.Text   = item.End[0].ToString();
            End_SML_Y.Text   = item.End[1].ToString();
            N_X.Text = item.Nx.ToString();
            N_Y.Text = item.Ny.ToString();
        }
    }
}


