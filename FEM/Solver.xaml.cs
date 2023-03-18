namespace FEM;

// % ****** Логика взаимодействия с окошком Solver ***** % //
public partial class Solver : Window
{
    public Solver()
    {
        InitializeComponent();
        grid.LoadGrid();
        receiversList.ItemsSource = receivers_str;   // ListBox для приемников
    }

    /* ----------------------- Переменные --------------------------------- */
    Grid grid;   // Структура сетки

    List<string> receivers_str = new List<string>();   // Лист с приемниками текстовыми

    /* ----------------------- Переменные --------------------------------- */



}

