namespace DNDMapper
{
    public partial class MainWindow
    {
        public class MapInfoModel
        {
            public string Name { get; set; }
            public Guid Id { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime EditedAt { get; set; }
            public List<HexCell> Cells { get; set; } = new List<HexCell>();
            public int XSize { get; set; }
            public int YSize { get; set; }
            public List<Guid> HistoryIds { get; set; } = new List<Guid>();
            public MapInfoModel() { }
            public MapInfoModel(string name, int xSize, int ySize) 
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
                InitializeMap(xSize, ySize);
                HistoryIds = new List<Guid>();
            }
            public MapInfoModel(string name, int xSize, int ySize, List<HexCell> list, DateTime createdAt, DateTime editedAt)
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentNullException("name");
                }
                if (xSize < 0)
                {
                    throw new ArgumentOutOfRangeException("x");
                }
                if (ySize < 0)
                {
                    throw new ArgumentOutOfRangeException("y");
                }
                if(list.Count != xSize * ySize)
                {
                    throw new ArgumentOutOfRangeException("wrong cells count");
                }
                Name = name;
                Id = Guid.NewGuid();
                CreatedAt = createdAt;
                EditedAt = editedAt;
                Cells = list;
                HistoryIds = new List<Guid>();
                XSize = xSize;
                YSize = ySize;
            }
            private void InitializeMap(int xSize, int ySize)
            {
                var res = new List<HexCell>();
                for (int i = 0; i < ySize; i++)
                    for (int j = 0; j < xSize; j++)
                    {
                        HexCell cell = new HexCell()
                        { Row = i, Col = j, Noise = 0.0, Color = "Blue" };
                        res.Add(cell);
                    }
                foreach (var cell in res)
                {
                    cell.Neighbors = GetHexNeighbors(res, cell);
                }
                Cells = res;
            }
            private List<HexCell> GetHexNeighbors(List<HexCell> cells, HexCell currentCell)
            {
                var neighbors = new List<(int Row, int Col)>
            {
                (currentCell.Row - 1, currentCell.Col),
                (currentCell.Row + 1, currentCell.Col),
                (currentCell.Row, currentCell.Col - 1),
                (currentCell.Row, currentCell.Col + 1),
                (currentCell.Col % 2 == 0 ? currentCell.Row - 1 : currentCell.Row + 1, currentCell.Col - 1),
                (currentCell.Col % 2 == 0 ? currentCell.Row - 1 : currentCell.Row + 1, currentCell.Col + 1)
            };
                return neighbors
                    .Where(n => n.Row >= 0 && n.Row < YSize && n.Col >= 0 && n.Col < XSize)
                    .Select(n => cells.First(c => c.Row == n.Row && c.Col == n.Col))
                    .ToList();
            }
        }
    }
}