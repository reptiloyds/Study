using System;
using _Game.Scripts.Enums;

namespace _Game.Scripts.Systems.Save.SaveStructures
{
    [Serializable]
    public struct GameParamsData
    {
        public GameParamType Type;
        public float Value;
    }
}