namespace FEM;

//: Обработчики TextBox
public partial class MainWindow
{

    //: TextBox только цифры и точка и минус
    private void PreviewTextInputDouble(object sender, TextCompositionEventArgs e) {
        // Добавляем регулярное выражение
        var regex = new Regex("[^0-9e.-]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    //: TextBox только цифры
    private void PreviewTextInputInt(object sender, TextCompositionEventArgs e) {
        // Добавляем регулярное выражение
        var regex = new Regex("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    //: TextBox только цифры для краевых
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

            string[] name = ((string)itemsList.SelectedValue).Split(" ");
            Item item = items.Find(n => n.Name.Equals(name[0]));

            Begin_SML_X.Text = item.Begin[0].ToString();
            Begin_SML_Y.Text = item.Begin[1].ToString();
            End_SML_X.Text   = item.End[0].ToString();
            End_SML_Y.Text   = item.End[1].ToString();
            N_X.Text = item.Nx.ToString();
            N_Y.Text = item.Ny.ToString();
            TextItemSigma.Text = item.Sigma.ToString();
            TextItem.Text = item.Name;
        }
    }

    //: Обработка выбора в списске слоев
    private void layersList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        if ((string)layersList.SelectedValue != null) {
            string[] name = ((string)layersList.SelectedValue).Split(" ");
            TextLayer.Text = name[0];
            TextLayerSigma.Text = name[1];
        }
    }
}


