namespace DrawGrid;

//: Обработчики Button
public partial class MainWindow : Window {

    //: Обработка кнопки "Добавить слой"
    private void AddLayer_Click(object sender, RoutedEventArgs e) {

        // Если значение слоя пусто
        if (TextLayer.Text == String.Empty || layers_str.Contains(TextLayer.Text)) {
            MessageBox.Show("Значение слоя не указано или такой слой уже задан!");
            return;
        }
        layers_str.Add(TextLayer.Text);
        layersList.Items.Refresh();
    }

    //: Обработка кнопки "Удалить слой"
    private void RemoveLayer_Click(object sender, RoutedEventArgs e) {

        // Если слой не выбран
        if (layersList.SelectedValue == null) {
            MessageBox.Show("Выберете слой!");
            return;
        }
        layers_str.Remove((string)layersList.SelectedValue);
        layersList.Items.Refresh();
    }

    //: Обработка кнопки "Добавить объект"
    private void AddItem_Click(object sender, RoutedEventArgs e) {

        Vector<double> Begin_SML = new Vector<double>(2);
        Vector<double> End_SML = new Vector<double>(2);
        Begin_SML[0] = Double.Parse(Begin_SML_X.Text);
        Begin_SML[1] = Double.Parse(Begin_SML_Y.Text);
        End_SML[0] = Double.Parse(End_SML_X.Text);
        End_SML[1] = Double.Parse(End_SML_Y.Text);
        int Nx = Int32.Parse(N_X.Text) < min_count_step ? min_count_step : Int32.Parse(N_X.Text);
        int Ny = Int32.Parse(N_Y.Text) < min_count_step ? min_count_step : Int32.Parse(N_Y.Text);
        string Name = TextItem.Text;

        // Если имя объекта не указано
        if (TextItem.Text == String.Empty || items_str.Contains(TextItem.Text)) {
            MessageBox.Show("Имя такого объекта уже существует или поле пустое!");
            return;
        }
        items_str.Add(TextItem.Text);
        itemsList.Items.Refresh();
        items.Add(new Item(Begin_SML, End_SML, Nx, Ny, Name));
    }

    //: Обработка кнопки "Удалить объект"
    private void RemoveItem_Click(object sender, RoutedEventArgs e) {

        // Если объект не выбран
        if (itemsList.SelectedValue == null) {
            MessageBox.Show("Выберете объект!");
            return;
        }
        string name = (string)layersList.SelectedValue;
        items_str.Remove(name);
        itemsList.Items.Refresh();
        items.RemoveAll(n => n.Name.Equals(name));
    }

    //: Обработка кнопки "Построить сетку"
    private void BuildGrid_Click(object sender, RoutedEventArgs e) {
        UpdateComponent(); // Обновляем компоненты
        CreateGrid();      // Строим сетку
        DrawGrid();        // Рисуем сетку
    }


}