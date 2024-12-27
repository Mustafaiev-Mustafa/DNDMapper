using System.Windows.Shapes;
using DNDMapper.Infrastructure;

namespace DNDMapper.Core
{
    public class Region
    {
        public Region()
        {

        }

        public Region(string name, Guid? owner, string color, List<HexCell> cells)
        {
            if(name == null) throw new ArgumentNullException("name");
            if(color == null) throw new ArgumentNullException("color");
            if (cells == null || cells.Count == 0) throw new ArgumentNullException();
            Name = name;
            Owner = owner;
            Color = color;
            Cells = cells;
        }

        public string Name { get; set; }
        public Guid? Owner { get; set; }
        public string Color { get; set; }
        public List<HexCell> Cells { get; set; }
        public List<HexCell> GetCells(BiDictionary<HexCell, Polygon> cellToPolygonMap)
        {
            return cellToPolygonMap.Select(keyValue=> keyValue.Item1).Where(cell => Cells.Contains(cell)).ToList();
        }
    }
}