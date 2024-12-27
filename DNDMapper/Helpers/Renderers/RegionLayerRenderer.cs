using System.Windows.Media;
using System.Windows.Shapes;
using DNDMapper.Core;
using DNDMapper.Infrastructure;
using static DNDMapper.MainWindow;

namespace DNDMapper.Helpers.Renderers
{
    public class RegionLayerRenderer : ILayerRenderer
    {
        private readonly List<Region> _regions;
        private readonly BiDictionary<HexCell, Polygon> _cellToPolygonMap;
        public RegionLayerRenderer(BiDictionary<HexCell, Polygon> cellToPolygonMap, MapInfo model)
        {
            _cellToPolygonMap = cellToPolygonMap;
            _regions = model.Regions;
        }
        public void Render()
        {
            foreach (var region in _regions)
            {
                foreach (var cell in region.GetCells(_cellToPolygonMap))
                {
                    if (_cellToPolygonMap.TryGetByFirst(cell, out var hex))
                    {
                        hex.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(region.Color)) { Opacity = 0.6 };
                        hex.Stroke = Brushes.DarkGray;
                    }
                }
            }
        }
    }
}