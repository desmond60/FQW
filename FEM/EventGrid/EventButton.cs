namespace FEM;

//: Обработчики Button
public partial class MainWindow
{

    //: Обработка кнопки "Добавить слой"
    private void AddLayer_Click(object sender, RoutedEventArgs e) {

        // Если значение слоя пусто
        if (TextLayer.Text == String.Empty || TextLayerSigma.Text == String.Empty || layers_str.Contains(TextLayer.Text + " " + TextLayerSigma.Text)) {
            MessageBox.Show("Значение слоя не указано или такой слой уже задан!");
            return;
        }
        layers_str.Add(TextLayer.Text + " " + TextLayerSigma.Text);
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

        // Если имя объекта не указано
        if (TextItem.Text == String.Empty || TextItemSigma.Text == String.Empty || items_str.Contains(TextItem.Text + " " + TextItemSigma.Text)) {
            MessageBox.Show("Имя такого объекта уже существует или поля пустые!");
            return;
        }

        Vector<double> Begin_SML = new Vector<double>(2);
        Vector<double> End_SML = new Vector<double>(2);
        Begin_SML[0] = Double.Parse(Begin_SML_X.Text);
        Begin_SML[1] = Double.Parse(Begin_SML_Y.Text);
        End_SML[0] = Double.Parse(End_SML_X.Text);
        End_SML[1] = Double.Parse(End_SML_Y.Text);
        int Nx = Int32.Parse(N_X.Text) < min_count_step ? min_count_step : Int32.Parse(N_X.Text);
        int Ny = Int32.Parse(N_Y.Text) < min_count_step ? min_count_step : Int32.Parse(N_Y.Text);
        double Sigma = Double.Parse(TextItemSigma.Text);
        string Name = TextItem.Text;

        items_str.Add(TextItem.Text + " " + TextItemSigma.Text);
        itemsList.Items.Refresh();
        items.Add(new Item(Begin_SML, End_SML, Nx, Ny, Sigma, Name));
    }

    //: Обработка кнопки "Удалить объект"
    private void RemoveItem_Click(object sender, RoutedEventArgs e) {

        // Если объект не выбран
        if (itemsList.SelectedValue == null) {
            MessageBox.Show("Выберете объект!");
            return;
        }
        string[] name = ((string)itemsList.SelectedValue).Split(" ");
        items_str.Remove((string)itemsList.SelectedValue);
        itemsList.Items.Refresh();
        items.RemoveAll(n => n.Name.Equals(name[0]));
    }

    //: Обработка кнопки "Изменить объект"
    private void EditItem_Click(object sender, RoutedEventArgs e) {

        // Если объект не выбран
        if (itemsList.SelectedValue == null) {
            MessageBox.Show("Выберете объект!");
            return;
        }

        string[] name = ((string)itemsList.SelectedValue).Split(" ");
        int index = items.FindIndex(n => n.Name.Equals(name[0]));

        Vector<double> Begin_SML = new Vector<double>(2);
        Vector<double> End_SML = new Vector<double>(2);
        Begin_SML[0] = Double.Parse(Begin_SML_X.Text);
        Begin_SML[1] = Double.Parse(Begin_SML_Y.Text);
        End_SML[0] = Double.Parse(End_SML_X.Text);
        End_SML[1] = Double.Parse(End_SML_Y.Text);
        int Nx = Int32.Parse(N_X.Text) < min_count_step ? min_count_step : Int32.Parse(N_X.Text);
        int Ny = Int32.Parse(N_Y.Text) < min_count_step ? min_count_step : Int32.Parse(N_Y.Text);
        double Sigma = Double.Parse(TextItemSigma.Text);
        string Name = TextItem.Text;

        items[index] = new Item(Begin_SML, End_SML, Nx, Ny, Sigma, Name);
        items_str[index] = Name + " " + Sigma;
        itemsList.Items.Refresh();
    }


    //: Обработка кнопки "Построить сетку"
    private void BuildGrid_Click(object sender, RoutedEventArgs e) {

        if (items.Count == 0) {
            if (Directory.Exists(@"grid")) {
                grid.LoadGrid();
                LoadInterface();
                DrawGrid();
            }
            else
                MessageBox.Show("Задайте хотя бы один объект или создайте папку grid с ее содержимым!");
            return;
        }
        
        UpdateComponent(); // Обновляем компоненты
        CreateGrid();      // Строим сетку
        grid.WriteGrid();  // Запись сетки
        WriteInterface();  // Запись интерфейса
        DrawGrid();        // Рисуем сетку
    }

    //: Обработка кнопки "Решатель"
    private void Solution_Click(object sender, RoutedEventArgs e) {

        if (!Directory.Exists(@"grid")) {
            MessageBox.Show("Папка с сеткой (grid) не обнаружена. Постройте сетку!");
            return;
        }

        Solver solver = new Solver();
        solver.Show();
    }
}