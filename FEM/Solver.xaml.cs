namespace FEM;

// % ****** Логика взаимодействия с окошком Solver ***** % //
public partial class Solver : Window
{
    public Solver()
    {
        InitializeComponent();
        grid.LoadGrid();
        LoadSigmaMaterial();
        receiversList.ItemsSource = receivers_str;   // ListBox для приемников
        sigmaList.ItemsSource = sigma_str;           // ListBox для проводимости
    }

    /* ----------------------- Переменные --------------------------------- */
    Grid grid = new Grid();   // Структура сетки
    SLAU slau = new SLAU();   // Структура СЛАУ

    List<string> receivers_str = new List<string>();   // Лист с приемниками для ListBox
    List<string> sigma_str = new List<string>();       // Лист с проводимостью для ListBox
    List<double> sigma1D = new List<double>();         // Значение одномерной sigma
    List<double> sigma2D = new List<double>();         // Значение двумерной sigma

    /* ----------------------- Переменные --------------------------------- */


    //: Загрузка 
    private void LoadSigmaMaterial() {
        sigma_str.Add($"1 1D = 0 2D = 0");
        sigma1D.Add(0);
        sigma2D.Add(0);
        for (int i = 0, id = 2; i < grid.Layers.Count; i++, id++) {
            sigma_str.Add($"{id} 1D = 1.0E-002 2D = 1.0E-002");
            sigma1D.Add(1e-2);
            sigma2D.Add(1e-2);
        }
    }
}

