using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DNDMapper.Core.Enums
{
    public enum ColorEnum
    {
        Green,
        Blue,
        Brown
    }

    public static class DndColorConverter
    {
        public static SolidColorBrush GetSolidColor(ColorEnum color)
        {
            return color switch
            {
                ColorEnum.Green => Brushes.Green,
                ColorEnum.Brown => Brushes.Brown,
                ColorEnum.Blue => Brushes.LightBlue,
                _ => throw new NotImplementedException($"{color} doesn't implemented")
            };
        }
    }
}
