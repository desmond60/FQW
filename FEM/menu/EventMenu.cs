namespace FEM;

//: Обработчики MenuItem
public partial class MainWindow
{
    //: Обработчик открытия окна с нейронкой
    private void MIneuronet_Click(object sender, RoutedEventArgs e)
    {
        if (!Directory.Exists(@"grid"))
        {
            MessageBox.Show("Папка с сеткой (grid) не обнаружена. Постройте сетку!");
            return;
        }

        Neuronet neuronet = new Neuronet();
        neuronet.ShowDialog();
    }

    //: Обработчик рисования сетки
    private void MIdrawGrid_Click(object sender, RoutedEventArgs e)
    {
        WinForm.FolderBrowserDialog dialog = new WinForm.FolderBrowserDialog();
        dialog.InitialDirectory = Directory.GetCurrentDirectory();

        if (dialog.ShowDialog() == WinForm.DialogResult.OK)
        {
            var folder = dialog.SelectedPath;
            grid.LoadGrid(folder);
            LoadInterface(folder);
            DrawGrid();
        }
    }

    //: Обработчик закрытия окна
    private void MIeXit_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}