namespace FEM;

// % ****** Логика взаимодействия с окошком Solver ***** % //
public partial class Solver : Window
{
    public Solver() {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");   // Установка культуры
        InitializeComponent();
        grid.LoadGrid();
        LoadSigmaMaterial();
        receiversList.ItemsSource = receivers_str;   // ListBox для приемников
        sigmaList.ItemsSource = sigma_str;           // ListBox для проводимости

        // Чтение приемников
        string[] FReceiver = File.ReadAllLines(@"grid/receivers.txt");
        for (int i = 0; i < FReceiver.Length; i++) {
            receivers_str.Add(FReceiver[i]);
        }
        receiversList.Items.Refresh();
    }

    /* ----------------------- Переменные --------------------------------- */
    Grid grid = new Grid();   // Структура сетки
    SLAU slau = new SLAU();   // Структура СЛАУ
    Harm1D harm1d;

    List<string> receivers_str = new List<string>();   // Лист с приемниками для ListBox
    List<string> sigma_str = new List<string>();       // Лист с проводимостью для ListBox
    List<Complex> lEx = new List<Complex>();           // Лист с компонентами электрического поля
    List<double>  lRk = new List<double>();            // Лист с кажущимеся сопротивлениями

    /* ----------------------- Переменные --------------------------------- */


    //: Загрузка 
    private void LoadSigmaMaterial() {
        for (int i = 0; i < grid.Sigmas.Count; i++)
            sigma_str.Add($"{i + 1} 1D = {grid.Sigmas[i].Sigma1D.ToString("E3")} 2D = {grid.Sigmas[i].Sigma2D.ToString("E3")}");
    }
}

