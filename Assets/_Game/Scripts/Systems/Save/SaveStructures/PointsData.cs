using System.Collections.Generic;

namespace _Game.Scripts.Systems.Save.SaveStructures
{
    public struct PointsData
    {
        public int Region;
        public List<PointData> Points;

        public void Clear()
        {
            Points.Clear();
        }
    }
}