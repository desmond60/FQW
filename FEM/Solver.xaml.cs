namespace FEM;

// % ****** Логика взаимодействия с окошком Solver ***** % //
public partial class Solver : Window
{
    public Solver() {
        InitializeComponent();
        grid.LoadGrid();
        LoadSigmaMaterial();
        receiversList.ItemsSource = receivers_str;   // ListBox для приемников
        sigmaList.ItemsSource = sigma_str;           // ListBox для проводимости
    }

    /* ----------------------- Переменные --------------------------------- */
    Grid grid = new Grid();   // Структура сетки
    SLAU slau = new SLAU();   // Структура СЛАУ
    Harm1D harm1d;

    List<string> receivers_str = new List<string>();   // Лист с приемниками для ListBox
    List<string> sigma_str = new List<string>();       // Лист с проводимостью для ListBox

    /* ----------------------- Переменные --------------------------------- */


    //: Загрузка 
    private void LoadSigmaMaterial() {
        for (int i = 0; i < grid.Sigmas.Count; i++)
            sigma_str.Add($"{i + 1} 1D = {grid.Sigmas[i].Sigma1D.ToString("E3")} 2D = {grid.Sigmas[i].Sigma2D.ToString("E3")}");
    }
}

