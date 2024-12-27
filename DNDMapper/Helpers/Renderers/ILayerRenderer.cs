using System.Windows.Shapes;
using DNDMapper.Core;
using DNDMapper.Infrastructure;
using static DNDMapper.MainWindow;

namespace DNDMapper.Helpers.Renderers
{
    public interface ILayerRenderer
    {
        void Render();
    }
}