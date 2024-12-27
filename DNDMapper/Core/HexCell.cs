using System.Windows;
using System.Windows.Shapes;
using DNDMapper.Core.Enums;
using Newtonsoft.Json;

namespace DNDMapper.Core
{
    public class HexCell
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public ColorEnum Color { get; set; } = ColorEnum.Blue;
        public double Noise { get; set; }
        [JsonIgnore]
        public List<HexCell> Neighbors { get; set; } = new List<HexCell>();

        public override bool Equals(object? obj)
        {
            if (obj is HexCell cell)
            {
                return cell.Col == Col && cell.Row == Row;
            }
            return base.Equals(obj);
        }
    }
}