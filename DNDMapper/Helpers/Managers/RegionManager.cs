using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DNDMapper.Core;
using static DNDMapper.MainWindow;

namespace DNDMapper.Helpers.Managers
{
    public class RegionManager
    {
        private readonly List<Region> _regions;
        public RegionManager()
        {
            _regions = new List<Region>();
        }
        public RegionManager(MapInfo model)
        {
            _regions = model.Regions;
        }
        public void AddRegion(Region region)
        {
            if (_regions.Any(r => r.Name == region.Name))
                throw new InvalidOperationException("Region with this name already exist.");

            _regions.Add(region);
        }

        public void RemoveRegion(string name)
        {
            var region = _regions.FirstOrDefault(r => r.Name == name);
            if (region != null)
            {
                _regions.Remove(region);
            }
        }

        public List<Region> GetRegions()
        {
            return _regions;
        }
    }
}
