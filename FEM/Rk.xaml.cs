﻿using ScottPlot;

namespace FEM;

// % ****** Логика взаимодействия с окошком Rk Plot ***** % //
public partial class Rk : Window
{
    public Rk(List<double> lRk, List<double> receivers) {
        InitializeComponent();

        // График модель 2Д-1 при частоте 0.1 Гц
        /*        var abs = new double[] {
                    1.650,
                    48.660,
                    114.620,
                    118.030,
                    109.440,
                    104.340,
                    102.870
                };*/

        // График модель 2Д-1 при частоте 10 Гц
/*        var abs = new double[] {
            10.340,
            49.500,
            92.820,
            98.220,
            99.530,
            99.840,
            99.830
        };*/

        var scatter = RkPlot.Plot.AddScatter(receivers.ToArray(), lRk.ToArray(), Color.Green);
        scatter.LineWidth = 1;
        scatter.MarkerColor = Color.DarkRed;

/*        scatter = RkPlot.Plot.AddScatter(receivers.ToArray(), abs, Color.Red);
        scatter.LineWidth = 1;
        scatter.MarkerColor = Color.DarkGreen;*/

        RkPlot.Refresh();
    }
}

