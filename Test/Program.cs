using Approximation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //List<Point> points = new List<Point>() { new Point(0, -1), new Point(1, 3), new Point(2, 4), new Point(3, 7), new Point(4, 1), new Point(5, 8) };
            //Interpolation interpolation = new Interpolation(points);

            //Point point = interpolation.NewtonPolynomial(4.75);

            //List<Point> points = new List<Point>() { new Point(2, 3), new Point(4, 1), new Point(6, 4), new Point(8, 5), new Point(10, 7), new Point(12, 2), new Point(14, 5), new Point(16, 10), new Point(18, 3), new Point(20, 9), new Point(22, 12), new Point(24, 3), new Point(26, 4), new Point(28, 5), new Point(30, 7), new Point(32, 2), new Point(34, 5), new Point(36, 10), new Point(38, 3), new Point(40, 9), new Point(42, 12) };
            //Interpolation interpolation = new Interpolation(points);

            //List<Point> points1 = new List<Point>() { new Point(2, 3), new Point(3, 5), new Point(4, 2), new Point(1, 7), };

            //List<Point> points1 = new List<Point> { new Point(2, 5), new Point(3, 4), new Point(5, 3) };

            //List<Point> points1 = new List<Point>() { new Point(81, 183), new Point(71, 168), new Point(64, 171), new Point(69, 178), new Point(69, 176), new Point(64, 172), new Point(68, 165), new Point(59, 158), new Point(81, 183), new Point(91, 182), new Point(57, 163), new Point(65, 175), new Point(58, 164), new Point(62, 175) };

            List<Point> points1 = new List<Point> { new Point(3, 3), new Point(4, 5), new Point(2, 4)};

            Approximattion approximattion = new Approximattion(points1);
            approximattion.MethodOfMinimumRoots(3);
            
            Console.Read();
        }
    }
}
