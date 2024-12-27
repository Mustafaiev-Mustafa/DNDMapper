using System.Windows;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace DNDMapper
{
    public class HexCell
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public string Color { get; set; } = "Blue"; 
        public double Noise { get; set; }
        [JsonIgnore]
        public List<HexCell> Neighbors { get; set; } = new List<HexCell>();
    }
}