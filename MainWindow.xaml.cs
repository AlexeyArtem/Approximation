using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using LiveCharts;
using LiveCharts.Wpf;

namespace Approximation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SeriesCollection series;
        ObservableCollection<Point> points;

        public MainWindow()
        {
            InitializeComponent();
            points = new ObservableCollection<Point>();
            series = new SeriesCollection();

            ListData.ItemsSource = points;
            Chart.Series = series;

            //series = new SeriesCollection()
            //{
            //    new LineSeries { Values = new ChartValues<double> { 3, 5, 7, 4 } },
            //    new ColumnSeries { Values = new ChartValues<decimal> { 5, 6, 2, 7 } }
            //};

            //Chart.Series = series;
            //series.Add(new ColumnSeries { Values = new ChartValues<decimal> { 5, 6, 2, 7 } });

            CbSelectData.SelectedIndex = 1;
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

        private void BtStartApproximation_Click(object sender, RoutedEventArgs e)
        {
            //Аппроксимация функции
            if (CbSelectData.SelectedIndex == 0)
            {

            }
            //Аппроксимация данных
            else 
            {
                if (points.Count != 0) 
                {
                    LineSeries line = new LineSeries();
                    line.Values = new ChartValues<LiveCharts.Defaults.ObservablePoint>();

                    Interpolation interpolation = new Interpolation(points.ToList<Point>());
                    for (int i = 0; i < points.Count; i++)
                    {
                        int k = 0;
                        int n = i;
                        if (i != 0) k = i - 1;
                        else n = i + 1;

                        Point point = interpolation.LinearMethod(points[i].X, k, n);
                        line.Values.Add(new LiveCharts.Defaults.ObservablePoint(point.X, point.Y));
                    }

                    series.Add(line);
                }
                else MessageBox.Show("Заполните список точек", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtAddPoint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                points.Add(new Point(Convert.ToDouble(TextBoxValueX.Text), Convert.ToDouble(TextBoxValueY.Text)));
            }
            catch (FormatException ex) 
            {
                MessageBox.Show(ex.Message, "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtDelPoint_Click(object sender, RoutedEventArgs e)
        {
            if (ListData.SelectedIndex != -1)
            {
                points.Remove((Point)ListData.SelectedItem);
            }
            else MessageBox.Show("Для удаления выберите точку из списка.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
