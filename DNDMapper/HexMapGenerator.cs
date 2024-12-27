using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DNDMapper.MainWindow;

namespace DNDMapper
{
    public static class HexMapGenerator
    {
        public static MapInfoModel GenerateMap(MapInfoModel model)
        {
            GenerateHeightMap(model);
            ClassifyTerrain(model);
            RefineContours(model);
            return model;
        }
        private static void GenerateHeightMap(MapInfoModel model)
        {
            int seed = new Random().Next(1000);
            int octaves = 25;
            double persistence = 0.75;
            double scale = 0.02;



            foreach (var cell in model.Cells)
            {
                double noise = (PerlinNoise.Perlin(cell.Row * scale + seed, cell.Col * scale + seed, octaves, persistence) + 1) / 2;
                cell.Noise = noise;
            }
        }
        private static void ClassifyTerrain(MapInfoModel model)
        {
            foreach (var cell in model.Cells)
            {
                if (cell.Noise < 0.485)
                {
                    cell.Color = "Blue";
                }
                else if (cell.Noise < 0.6)
                {
                    cell.Color = "Green";
                }
                else
                {
                    cell.Color = "Brown";
                }
            }
        }
        private static void RefineContours(MapInfoModel model)
        {
            for (int iteration = 0; iteration < 3; iteration++)
            {
                var changes = new List<HexCell>();

                Parallel.ForEach(model.Cells.Where(c => c.Color == "Blue"), cell =>
                {
                    int landNeighbors = cell.Neighbors.Count(n => n.Color == "Green");
                    if (landNeighbors > 3)
                    {
                        changes.Add(cell);
                    }
                });

                foreach (var cell in changes)
                {
                    cell.Color = "Green";
                }
            }
        }
    }
}
