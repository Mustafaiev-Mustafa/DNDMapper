using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using DNDMapper.Core;
using DNDMapper.Core.Enums;
using DNDMapper.Infrastructure;

namespace DNDMapper.Helpers
{
    public static class HexDrawer
    {
        private static readonly int _hexRadius = 8;
        public static BiDictionary<HexCell, Polygon> DrawHexMap(Canvas hexCanvas, MapInfo infoModel)
        {
            double hexWidth = 2 * _hexRadius;
            double hexHeight = Math.Sqrt(3) * _hexRadius;
            double xOffset = _hexRadius * 3 / 2;
            double yOffset = hexHeight;
            BiDictionary<HexCell,Polygon> result = new BiDictionary<HexCell,Polygon>();
            foreach (var cell in infoModel.Cells)
            {
                double x = cell.Col * xOffset;
                double y = cell.Row * yOffset;

                if (cell.Col % 2 == 1)
                {
                    y += hexHeight / 2;
                }

                Polygon hex = CreateHexagon(cell, _hexRadius);

                Canvas.SetLeft(hex, x);
                Canvas.SetTop(hex, y);
                hexCanvas.Children.Add(hex);
                result.Add(cell, hex);
            }
            return result;
        }
        private static Polygon CreateHexagon(HexCell cell, double radius)
        {
            Brush fill = DndColorConverter.GetSolidColor(cell.Color);

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
                    Panel.SetZIndex(hex, int.MaxValue);
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
    }
}