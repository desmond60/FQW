namespace FEM;

// % ****** Логика взаимодействия с окошком Ex Plot ***** % //
public partial class Ex : Window
{
    public Ex(List<Complex> lEx, List<double> receivers) {
        InitializeComponent();

        ExPlot.Plot.AddScatter(receivers.ToArray(), lEx.Select(n => n.Real).ToArray());

        // Настройки графика
        ExPlot.Plot.Title("График Ex");

        ExPlot.Refresh();
    }
}