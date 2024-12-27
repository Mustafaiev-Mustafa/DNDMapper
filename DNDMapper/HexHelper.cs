using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using static DNDMapper.MainWindow;

namespace DNDMapper
{
    public static class HexHelper
    {
        public static void DrawHexMap(Canvas hexCanvas, MapInfoModel model, BiDictionary<HexCell, Polygon> polygonCache, double radius)
        {
            double hexWidth = 2 * radius;
            double hexHeight = Math.Sqrt(3) * radius;
            double xOffset = radius * 3 / 2;
            double yOffset = hexHeight;

            foreach (var cell in model.Cells)
            {
                double x = cell.Col * xOffset;
                double y = cell.Row * yOffset;

                if (cell.Col % 2 == 1)
                {
                    y += hexHeight / 2;
                }

                Polygon hex = CreateHexagon(cell, radius);

                Canvas.SetLeft(hex, x);
                Canvas.SetTop(hex, y);
                polygonCache.Add(cell,hex);
                hexCanvas.Children.Add(hex);
            }
        }
        public static void UpdateHexColors(BiDictionary<HexCell, Polygon> cellToPolygonMap, MapInfoModel model)
        {
            foreach (var keyValue in cellToPolygonMap)
            {
                keyValue.Item2.Fill = keyValue.Item1.Color switch
                {
                    "Green" => Brushes.Green,
                    "Brown" => Brushes.Brown,
                    _ => Brushes.LightBlue
                };
            }
        }
        private static Polygon CreateHexagon(HexCell cell, double radius)
        {
            Brush fill = cell.Color switch
            {
                "Green" => Brushes.Green,
                "Brown" => Brushes.Brown,
                _ => Brushes.LightBlue
            };

            Polygon hex = new Polygon
            {
                Stroke = Brushes.Black,
                Fill = fill,
                StrokeThickness = 1,
                RenderTransformOrigin = new Point(0.5, 0.5),
            };

            PointCollection points = new PointCollection();
            for (int i = 0; i < 6; i++)
            {
                double angle = Math.PI / 3 * i;
                double px = radius * Math.Cos(angle);
                double py = radius * Math.Sin(angle);
                points.Add(new Point(px, py));
            }
            hex.Points = points;

            hex.RenderTransform = new ScaleTransform(1, 1);

            hex.MouseEnter += (sender, args) =>
            {
                if (sender is Polygon hex && hex.Stroke != Brushes.Yellow)
                {
                    BringToFront(hex);
                    hex.Stroke = Brushes.Yellow;
                    hex.StrokeThickness = 3;
                }
            };

            hex.MouseLeave += (sender, args) =>
            {
                if (sender is Polygon hex && hex.Stroke == Brushes.Yellow)
                {
                    Panel.SetZIndex(hex, 0);
                    hex.Stroke = Brushes.Black;
                    hex.StrokeThickness = 1;
                }
            };

            return hex;
        }
        private static void BringToFront(UIElement element)
        {
            Panel.SetZIndex(element, int.MaxValue);
        }
        public static void ChangeHexColor(Polygon hex, HexCell cell)
        {
            if (cell.Color == "Blue")
            {
                cell.Color = "Green";
                hex.Fill = Brushes.Green;
            }
            else if (cell.Color == "Green")
            {
                cell.Color = "Brown";
                hex.Fill = Brushes.Brown;
            }
            else
            {
                cell.Color = "Blue";
                hex.Fill = Brushes.LightBlue;
            }
        }
    }
}