﻿namespace DrawGrid;

//: Обработчики Toggle
public partial class MainWindow : Window {

    //: Обработка Toggle "Строгость сетки"
    private void ToggleStrict_Click(object sender, RoutedEventArgs e) {
        IsStrictGrid = !IsStrictGrid;
    }
}