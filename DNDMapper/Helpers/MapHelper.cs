using System.Windows.Shapes;
using System.Xml.Linq;
using DNDMapper.Core;
using DNDMapper.Helpers.Renderers;
using DNDMapper.Infrastructure;

namespace DNDMapper.Helpers
{
    public class MapHelper
    {
        private readonly MapInfo _infoModel; 
        BiDictionary<HexCell, Polygon> _cellToPolygonMap;
        private readonly Dictionary<MapLayerEnum, ILayerRenderer> _renderers = [];
        private MapLayerEnum _currentMapLayer;

        public MapHelper(BiDictionary<HexCell, Polygon> cellToPolygonMap, MapInfo model)
        {
            _infoModel = model;
            _renderers = new Dictionary<MapLayerEnum, ILayerRenderer>()
            {
                { MapLayerEnum.Physical, new PhysicalLayerRenderer(cellToPolygonMap) },
                { MapLayerEnum.Regions, new RegionLayerRenderer(cellToPolygonMap, model) },
            };
            _currentMapLayer = MapLayerEnum.Physical;
        }
        private ILayerRenderer GetCurrentRenderer()
        {
            if (_renderers.TryGetValue(_currentMapLayer, out var renderer))
            {
                return renderer;
            }
            else
            {
                throw new ArgumentException($"Renderer for {_currentMapLayer} is not implemented.", nameof(_currentMapLayer));
            }
        }
        public void ShowLayer()
        {
            GetCurrentRenderer().Render();
        }
        public void ShowLayer(MapLayerEnum layer)
        {
            _currentMapLayer = layer;
            GetCurrentRenderer().Render();
        }
        public void ChangeHexColor(Polygon hex, HexCell cell)
        {
            if (GetCurrentRenderer() is PhysicalLayerRenderer physicalLayerRenderer)
            {
                physicalLayerRenderer.ChangeHexColor(hex, cell);
            }
        }
    }
}