using System;
using _Game.Scripts.Enums;
using UnityEngine;

namespace _Game.Scripts.Systems.Save.SaveStructures
{
    [Serializable]
    public struct PointData
    {
        public int Region;
        public int Id;
        public int Level;
        public PointType Type;
        public int RemainingMoney;
        public bool IsBought;
    }
}