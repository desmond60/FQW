using ScottPlot;
using ScottPlot.Plottable;

namespace FEM;

// % ****** Логика взаимодействия с окошком Ex Plot ***** % //
public partial class Ex : Window
{
    public Ex(List<Complex> lEx, List<double> receivers) {
        InitializeComponent();

        // График модель 2Д-1 при частоте 0.1 Гц
        /*        var abs = new double[] {
                            0.127,
                            0.688,
                            1.064,
                            1.078,
                            1.038,
                            1.015,
                            1.008
                        };*/

        // График модель 2Д-1 при частоте 10 Гц
        /*var abs = new double[] {
                            0.289,
                            0.703,
                            0.962,
                            0.989,
                            0.995,
                            0.996,
                            0.996
                        };*/

        var scatter = ExPlot.Plot.AddScatter(receivers.ToArray(), lEx.Select(n => n.Real).ToArray(), Color.Green);
        scatter.LineWidth = 2;
        scatter.MarkerColor = Color.Black;
        scatter.Label = "Решение моей программы";
/*
        scatter = ExPlot.Plot.AddScatter(receivers.ToArray(), abs, Color.Brown);
        scatter.LineWidth = 2;
        scatter.MarkerColor = Color.Black;
        scatter.Label = "Решение авторами COMMEMI";*/

        ExPlot.Plot.Legend(true);
        ExPlot.Refresh();
    }
}