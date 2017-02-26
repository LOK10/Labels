using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Labels
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Storage storage = new Storage();
        private readonly DesktopElementsManager dem = new DesktopElementsManager();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            Environment.Exit(0);
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var elementLocations = dem.GetElementLocations();
            storage.SaveElementLocations(elementLocations);
        }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            var elementLocations = storage.GetElementsLocations();
            dem.SetElementLocations(elementLocations);
        }

        private void btnRand_Click(object sender, RoutedEventArgs e)
        {
            var elementLocations = dem.GetElementLocations();
            dem.ShufleElementLocations(elementLocations.ToList());
        }
    }
}
