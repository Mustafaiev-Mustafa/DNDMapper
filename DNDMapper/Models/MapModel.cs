using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DNDMapper.Core;
using DNDMapper.Infrastructure;
using System.Windows.Shapes;
using DNDMapper.Helpers;
using System.Windows.Controls;

namespace DNDMapper.Models
{
    public class MapModel
    {
        public MapInfo MapInfo 
        { 
            get;
            private set;
        }
        public MapHelper MapHelper { get; private set; }
        public BiDictionary<HexCell, Polygon> CellToPolygonMap {  get; private set; }
        public void SetMapInfo(Canvas canvas, MapInfo mapInfo)
        {
            MapInfo = mapInfo;
            CellToPolygonMap = HexDrawer.DrawHexMap(canvas, mapInfo);
            MapHelper = new MapHelper(CellToPolygonMap, mapInfo);
        }
        public MapModel(Canvas canvas, MapInfo mapInfo) 
        { 
            MapInfo = mapInfo;
            CellToPolygonMap = HexDrawer.DrawHexMap(canvas, mapInfo);
            MapHelper = new MapHelper(CellToPolygonMap, mapInfo);
        }
    }
}