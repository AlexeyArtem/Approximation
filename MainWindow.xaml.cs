using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using MathNet.Symbolics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Approximation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SeriesCollection series;
        ObservableCollection<Point> points;
        LineSeries approximationLine, functionLine;
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
            functionLine = new LineSeries { Values = new ChartValues<ObservablePoint>(), PointGeometrySize = 0, Title = "Исходная функция", LineSmoothness = 0 };
            approximationLine = new LineSeries { Values = new ChartValues<ObservablePoint>(), PointGeometrySize = 0, Title = "Апроксимированная функция", LineSmoothness = 0 };
            scatterSeries = new ScatterSeries { Values = new ChartValues<ObservablePoint>(), Title = "Узел интерполяции", MinPointShapeDiameter = 7, MaxPointShapeDiameter = 20 };
            series.Add(approximationLine);
            series.Add(functionLine);
            series.Add(scatterSeries);

            ListData.ItemsSource = points;
            Chart.Series = series;

            CbSelectData.SelectedIndex = 1;
            CbInterpolationMethod.SelectedIndex = 0;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbSelectData.SelectedIndex == 0)
            {
                GbData.Visibility = Visibility.Hidden;
                GbData.IsEnabled = false;
                GbFunction.Visibility = Visibility.Visible;
                GbFunction.IsEnabled = true;
                if (CbApproximationMethod.SelectedIndex == 0 || CbApproximationMethod.SelectedIndex == 1)
                {
                    LbBorder.Visibility = GridBorder.Visibility = Visibility.Visible;
                    LbArea.Visibility = UdArea.Visibility = LbAreaLimit.Visibility = UdAreaLimit.Visibility = Visibility.Hidden;
                }
                else if (CbApproximationMethod.SelectedIndex == 2)
                {
                    LbBorder.Visibility = GridBorder.Visibility = Visibility.Hidden;
                    LbArea.Visibility = UdArea.Visibility = LbAreaLimit.Visibility = UdAreaLimit.Visibility = Visibility.Visible;
                }
            }
            else if (CbSelectData.SelectedIndex == 1) 
            {
                GbFunction.Visibility = Visibility.Hidden;
                GbFunction.IsEnabled = false;

                GbData.Visibility = Visibility.Visible;
                GbData.IsEnabled = true;

                if (CbInterpolationMethod.SelectedIndex == 5)
                {
                    LbDegreeMnk.Visibility = UdDegreeMnk.Visibility = Visibility.Visible;
                }
                else
                {
                    LbDegreeMnk.Visibility = UdDegreeMnk.Visibility = Visibility.Hidden;
                }
            }
        }

        private List<Point> GetPoints(double leftBorder, double rightBorder, double step)
        {
            SymbolicExpression expression = SymbolicExpression.Parse(TbFunction.Text);
            Dictionary<string, FloatingPoint> variable = new Dictionary<string, FloatingPoint>();
            variable.Add("x", 0);

            List<Point> points = new List<Point>();
            decimal x = (decimal)leftBorder;
            for (double i = leftBorder; i < (rightBorder - leftBorder) / step; i++)
            {
                variable["x"] = (double)x;
                points.Add(new Point((double)x, expression.Evaluate(variable).RealValue));
                x += (decimal)step;
            }

            return points;
        }
        private void BtStartApproximation_Click(object sender, RoutedEventArgs e)
        {
            //Аппроксимация функции
            if (CbSelectData.SelectedIndex == 0)
            {
                try
                {
                    scatterSeries.Values.Clear();
                    approximationLine.Values.Clear();
                    functionLine.Values.Clear();
                    ApproximationFunction approximation = new ApproximationFunction(TbFunction.Text);
                    List<Point> result = new List<Point>();
                    List<Point> functionValues = new List<Point>();

                    switch (CbApproximationMethod.SelectedIndex)
                    {
                        case 0:
                            result = approximation.ChebyshevPolynomial(Convert.ToInt32(UdDegreeMethod.Value), Convert.ToDouble(TbLeftBorder.Text), Convert.ToDouble(TbRightBorder.Text), (double)UdStep.Value);
                            functionValues = GetPoints(Convert.ToDouble(TbLeftBorder.Text), Convert.ToDouble(TbRightBorder.Text), (double)UdStep.Value);
                            break;
                        case 1:
                            result = approximation.FourierRows(Convert.ToInt32(UdDegreeMethod.Value), Convert.ToDouble(TbLeftBorder.Text), Convert.ToDouble(TbRightBorder.Text), (double)UdStep.Value);
                            functionValues = GetPoints(Convert.ToDouble(TbLeftBorder.Text), Convert.ToDouble(TbRightBorder.Text), (double)UdStep.Value);
                            break;
                        case 2:
                            result = approximation.TaylorsRow(Convert.ToInt32(UdDegreeMethod.Value), (double)UdArea.Value, (double)UdAreaLimit.Value, (double)UdStep.Value);
                            functionValues = GetPoints((double)UdArea.Value - (double)UdAreaLimit.Value, (double)UdArea.Value + (double)UdAreaLimit.Value, (double)UdStep.Value);
                            break;
                    }

                    foreach (Point point in result)
                        approximationLine.Values.Add(new ObservablePoint(point.X, point.Y));
                    foreach (Point point in functionValues)
                        functionLine.Values.Add(new ObservablePoint(point.X, point.Y));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            //Аппроксимация данных
            else
            {
                if (points.Count != 0)
                {
                    try
                    {
                        approximationLine.Values.Clear();
                        scatterSeries.Values.Clear();
                        functionLine.Values.Clear();
                        Interpolation interpolation = new Interpolation(points.ToList());
                        ApproximationData approximation = new ApproximationData(points.ToList());
                        List<Point> resPoints = new List<Point>();

                        switch (CbInterpolationMethod.SelectedIndex)
                        {
                            case 0:
                                resPoints = interpolation.LinearMethod((double)UdStepData.Value);
                                break;
                            case 1:
                                resPoints = interpolation.QuadraticMethod((double)UdStepData.Value);
                                break;
                            case 2:
                                resPoints = interpolation.SplineMethod((double)UdStepData.Value);
                                break;
                            case 3:
                                resPoints = interpolation.LagrangePolynomial((double)UdStepData.Value);
                                break;
                            case 4:
                                resPoints = interpolation.NewtonPolynomial((double)UdStepData.Value);
                                break;
                            case 5:
                                resPoints = approximation.MethodOfMinimumRoots((int)UdDegreeMnk.Value, (double)UdStepData.Value);
                                break;
                        }

                        foreach (Point point in resPoints)
                            approximationLine.Values.Add(new ObservablePoint(point.X, point.Y));
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

        private void CbApproximationMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(CbSelectData.SelectedIndex == 0)
            {
                if (CbApproximationMethod.SelectedIndex == 0 || CbApproximationMethod.SelectedIndex == 1)
                {
                    LbBorder.Visibility = GridBorder.Visibility = Visibility.Visible;
                    LbArea.Visibility = UdArea.Visibility = LbAreaLimit.Visibility = UdAreaLimit.Visibility = Visibility.Hidden;
                }
                else if (CbApproximationMethod.SelectedIndex == 2)
                {
                    LbBorder.Visibility = GridBorder.Visibility = Visibility.Hidden;
                    LbArea.Visibility = UdArea.Visibility = LbAreaLimit.Visibility = UdAreaLimit.Visibility = Visibility.Visible;
                }
            }
            
        }

        private void CbInterpolationMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                if (CbInterpolationMethod.SelectedIndex == 5)
                {
                    LbDegreeMnk.Visibility = UdDegreeMnk.Visibility = Visibility.Visible;
                }
                else
                {
                    LbDegreeMnk.Visibility = UdDegreeMnk.Visibility = Visibility.Hidden;
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
