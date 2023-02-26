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

    //: Обработка кнопки "Построить сетку"
    private void BuildGrid_Click(object sender, RoutedEventArgs e) {
        UpdateComponent(); // Обновляем компоненты
        CreateGrid();      // Строим сетку
        DrawGrid();        // Рисуем сетку
    }
}