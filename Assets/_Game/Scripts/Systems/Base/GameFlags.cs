using System.Collections.Generic;
using _Game.Scripts.Enums;
using Sirenix.Serialization;
using UnityEngine;

namespace _Game.Scripts.Systems.Base
{
    public class GameFlags
    {
        private List<GameFlag> _flags = new();

        public void Set(GameFlag flag)
        {
            if (Has(flag)) return;
            _flags.Add(flag);
        }

        public bool Has(GameFlag flag)
        {
            return _flags.Contains(flag);
        }

        public List<GameFlag> GetAllFlags()
        {
            return _flags;
        }
    }
}