using System.IO;
using Newtonsoft.Json;

namespace DNDMapper
{
    public partial class MainWindow
    {
        public static class HexFileLoader
        {
            public static MapInfoModel LoadMapFromFile(string saveFilePath)
            {
                string json = File.ReadAllText(saveFilePath);
                MapInfoModel mapInfoToReturn = JsonConvert.DeserializeObject<MapInfoModel>(json) ?? throw new FileLoadException();
                return mapInfoToReturn;
            }
            public static void SaveMapToFile(string saveFilePath, MapInfoModel map)
            {
                string directoryPath = Path.GetDirectoryName(saveFilePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                string json = JsonConvert.SerializeObject(map, Formatting.Indented);
                File.WriteAllText(saveFilePath, json);
            }
        }
    }
}