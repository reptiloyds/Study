using System.Collections.Generic;
using _Game.Scripts.View.DrawLine;
using Zenject;

namespace _Game.Scripts.Factories
{
    public class LineFactory
    {
        [Inject] private Line.Pool _linePool;
        private readonly List<Line> _lines = new();

        public Line SpawnLine()
        {
            var line = _linePool.Spawn();
            _lines.Add(line);

            return line;
        }

        public void RemoveLine(Line line)
        {
            _lines.Remove(line);
            _linePool.Despawn(line);
        }
    }
}