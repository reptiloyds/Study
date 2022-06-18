using System.Collections.Generic;

namespace _Game.Scripts.Systems.Save.SaveStructures
{
    public struct GameData
    {
        public int Level;
        public List<GameParamsData> Params;

        public void Clear()
        {
            Params.Clear();
        }
    }
}