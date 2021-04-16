using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Approximation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CbSelectData.SelectedIndex = 0;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbSelectData.SelectedIndex == 0)
            {
                GbData.Visibility = Visibility.Hidden;
                GbData.IsEnabled = false;

                GbFunction.Visibility = Visibility.Visible;
                GbFunction.IsEnabled = true;
            }
            else if (CbSelectData.SelectedIndex == 1) 
            {
                GbFunction.Visibility = Visibility.Hidden;
                GbFunction.IsEnabled = false;

                GbData.Visibility = Visibility.Visible;
                GbData.IsEnabled = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
