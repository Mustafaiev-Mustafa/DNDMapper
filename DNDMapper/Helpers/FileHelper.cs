using System.IO;
using DNDMapper.Core;
using Newtonsoft.Json;

namespace DNDMapper.Helpers
{
    public class FileHelper
    {
        public static MapInfo LoadMapFromFile(string saveFilePath)
        {
            string json = File.ReadAllText(saveFilePath);
            MapInfo mapInfoToReturn = JsonConvert.DeserializeObject<MapInfo>(json) ?? throw new FileLoadException();
            return mapInfoToReturn;
        }
        public static void SaveMapToFile(string saveFilePath, MapInfo map)
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