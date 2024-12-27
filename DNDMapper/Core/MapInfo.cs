using DNDMapper.Core;
using DNDMapper.Core.Enums;

namespace DNDMapper
{
    public class MapInfo
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime EditedAt { get; set; }
        public HexCell[,] Cells { get; set; }
        public int XSize { get; set; }
        public int YSize { get; set; }
        public List<Guid> HistoryIds { get; set; } = new List<Guid>();
        public List<Region> Regions { get; set; } = new List<Region>();
        public MapInfo() { }
        public MapInfo(string name, int xSize, int ySize) 
        { 
            if(string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            if(xSize < 0)
            {
                throw new ArgumentOutOfRangeException("x");
            }
            if (ySize < 0)
            {
                throw new ArgumentOutOfRangeException("y");
            }
            Name = name;
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
            EditedAt = DateTime.Now;
            XSize = xSize;
            YSize = ySize;
            HistoryIds = new List<Guid>();
            Regions = new List<Region>();
        }
        public void InitializeMap()
        {
            Cells = new HexCell[YSize, XSize];
            for (int i = 0; i < YSize; i++)
            {
                for (int j = 0; j < XSize; j++)
                {
                    Cells[i, j] = new HexCell { Row = i, Col = j, Noise = 0.0, Color = ColorEnum.Blue };
                }
            }
        }
        public List<HexCell> GetHexNeighbors(HexCell[,] cells, HexCell currentCell)
        {
            int row = currentCell.Row;
            int col = currentCell.Col;

            var neighbors = new List<(int Row, int Col)>
            {
                (row - 1, col),
                (row + 1, col),
                (row, col - 1),
                (row, col + 1),
                (col % 2 == 0 ? row - 1 : row + 1, col - 1),
                (col % 2 == 0 ? row - 1 : row + 1, col + 1)
            };

            return neighbors
                .Where(n => n.Row >= 0 && n.Row < cells.GetLength(0) && n.Col >= 0 && n.Col < cells.GetLength(1))
                .Select(n => cells[n.Row, n.Col])
                .ToList();
        }
    }
}