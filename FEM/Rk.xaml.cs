namespace FEM;

// % ****** Логика взаимодействия с окошком Rk Plot ***** % //
public partial class Rk : Window
{
    public Rk(List<double> lRk, List<double> receivers) {
        InitializeComponent();

        RkPlot.Plot.AddScatter(receivers.ToArray(), lRk.ToArray());

        // Настройки графика
        RkPlot.Plot.Title("График Rk");

        RkPlot.Refresh();
    }
}

