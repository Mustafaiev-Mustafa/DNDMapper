using System.Windows.Media;
using System.Windows.Shapes;
using DNDMapper.Core;
using DNDMapper.Core.Enums;
using DNDMapper.Infrastructure;
using static DNDMapper.MainWindow;

namespace DNDMapper.Helpers.Renderers
{
    public class PhysicalLayerRenderer : ILayerRenderer
    {
        BiDictionary<HexCell, Polygon> _cellToPolygonMap;
        public PhysicalLayerRenderer(BiDictionary<HexCell, Polygon> cellToPolygonMap)
        {
            _cellToPolygonMap = cellToPolygonMap;
        }
        public void Render()
        {

            foreach (var keyValue in _cellToPolygonMap)
            {
                keyValue.Item2.Fill = DndColorConverter.GetSolidColor(keyValue.Item1.Color);
            }
        }
        public void ChangeHexColor(Polygon hex, HexCell cell)
        {
            if (hex != null)
            {
                if (cell != null)
                {
                    hex.Fill = cell.Color switch
                    {
                        ColorEnum.Blue => Brushes.Green,
                        ColorEnum.Green => Brushes.Brown,
                        _ => Brushes.LightBlue
                    };
                }
                else
                {
                    throw new NullReferenceException("Cell you want to change color is null");
                }
            }
            else
            {
                throw new NullReferenceException("Hex you want to change color is null");
            }
        }
    }
}