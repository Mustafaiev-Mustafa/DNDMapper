using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DNDMapper.Core;
using DNDMapper.Core.Enums;

namespace DNDMapper.Helpers
{
    public static class GenerateHelper
    {
        public static MapInfo GenerateMap(MapInfo model)
        {
            GenerateHeightMap(model);
            ClassifyTerrain(model);
            RefineContours(model);
            return model;
        }
        private static void GenerateHeightMap(MapInfo model)
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
        private static void ClassifyTerrain(MapInfo model)
        {
            foreach (var cell in model.Cells)
            {
                if (cell.Noise < 0.485)
                {
                    cell.Color = ColorEnum.Blue;
                }
                else if (cell.Noise < 0.6)
                {
                    cell.Color = ColorEnum.Green;
                }
                else
                {
                    cell.Color = ColorEnum.Brown;
                }
            }
        }
        private static void RefineContours(MapInfo model)
        {
            for (int iteration = 0; iteration < 3; iteration++)
            {
                var changes = new List<HexCell>();

                foreach (var cell in model.Cells)
                {
                    if (cell.Color == ColorEnum.Blue)
                    {
                        int landNeighbors = cell.Neighbors.Count(n => n.Color == ColorEnum.Green);
                        if (landNeighbors > 3)
                        {
                            changes.Add(cell);
                        }
                    }
                }

                if (changes.Count == 0) break;
                foreach (var cell in changes)
                {
                    cell.Color = ColorEnum.Green;
                }
            }
        }
    }
    public static class PerlinNoise
    {
        public static double Noise(double x, double y)
        {
            int n = (int)x + (int)y * 57;
            n = n << 13 ^ n;
            return 1.0 - (n * (n * n * 15731 + 789221) + 1376312589 & 0x7fffffff) / 1073741824.0;
        }

        public static double SmoothedNoise(double x, double y)
        {
            double corners = (Noise(x - 1, y - 1) + Noise(x + 1, y - 1) + Noise(x - 1, y + 1) + Noise(x + 1, y + 1)) / 16;
            double sides = (Noise(x - 1, y) + Noise(x + 1, y) + Noise(x, y - 1) + Noise(x, y + 1)) / 8;
            double center = Noise(x, y) / 4;
            return corners + sides + center;
        }

        public static double InterpolatedNoise(double x, double y)
        {
            int integer_X = (int)x;
            double fractional_X = x - integer_X;

            int integer_Y = (int)y;
            double fractional_Y = y - integer_Y;

            double v1 = SmoothedNoise(integer_X, integer_Y);
            double v2 = SmoothedNoise(integer_X + 1, integer_Y);
            double v3 = SmoothedNoise(integer_X, integer_Y + 1);
            double v4 = SmoothedNoise(integer_X + 1, integer_Y + 1);

            double i1 = Interpolate(v1, v2, fractional_X);
            double i2 = Interpolate(v3, v4, fractional_X);

            return Interpolate(i1, i2, fractional_Y);
        }

        public static double Perlin(double x, double y, int octaves, double persistence)
        {
            double total = 0;
            double frequency = 1;
            double amplitude = 1;
            double maxValue = 0; // Used for normalizing result

            for (int i = 0; i < octaves; i++)
            {
                total += InterpolatedNoise(x * frequency, y * frequency) * amplitude;

                maxValue += amplitude;
                amplitude *= persistence;
                frequency *= 2;
            }

            return total / maxValue;
        }

        private static double Interpolate(double a, double b, double x)
        {
            double ft = x * Math.PI;
            double f = (1 - Math.Cos(ft)) * 0.5;

            return a * (1 - f) + b * f;
        }
    }
}
