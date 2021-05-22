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
using LiveCharts.Defaults;
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
        LineSeries lineSeries;
        ScatterSeries scatterSeries;

        public MainWindow()
        {
            InitializeComponent();
            points = new ObservableCollection<Point>()
            {
                new Point(1, 1.2),
                new Point(2, 1),
                new Point(3, 2),
                new Point(4, 1.5),
                new Point(5, 2),
                new Point(6, 1.5),
                new Point(7, 2),
                new Point(8, 2.2),

            };
            series = new SeriesCollection();
            lineSeries = new LineSeries { Values = new ChartValues<ObservablePoint>(), PointGeometrySize = 0 /*Title = "f(x)"*/, LineSmoothness = 0 };
            scatterSeries = new ScatterSeries { Values = new ChartValues<ObservablePoint>(), Title = "Узел интерполяции", MinPointShapeDiameter = 7, MaxPointShapeDiameter = 20 };
            series.Add(lineSeries);
            series.Add(scatterSeries);

            ListData.ItemsSource = points;
            Chart.Series = series;

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
                    lineSeries.Values.Clear();
                    scatterSeries.Values.Clear();
                    Interpolation interpolation = new Interpolation(points.ToList());
                    List<Point> resPoints = new List<Point>();

                    try
                    {
                        switch (CbInterpolationMethod.SelectedIndex)
                        {
                            case 0:
                                resPoints = interpolation.LinearMethod(0.01);
                                break;
                            case 1:
                                resPoints = interpolation.QuadraticMethod(0.01);
                                break;
                            case 3:
                                resPoints = interpolation.LagrangePolynomial(0.01);
                                break;
                            case 4:
                                resPoints = interpolation.NewtonPolynomial(0.01);
                                break;
                        }

                        foreach (Point point in resPoints)
                            lineSeries.Values.Add(new ObservablePoint(point.X, point.Y));
                        foreach (Point point in points)
                            scatterSeries.Values.Add(new ObservablePoint(point.X, point.Y));
                    }
                    catch (Exception ex) 
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
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
