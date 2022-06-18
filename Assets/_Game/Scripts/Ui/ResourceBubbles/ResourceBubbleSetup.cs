using System;
using _Game.Scripts.Enums;
using UnityEngine;

namespace _Game.Scripts.Ui.ResourceBubbles
{
    [Serializable]
    public class ResourceBubbleSetup
    {
        public GamePlayElement Type;
        public int Conversion = 1;
        public int Max = 10;
        public Sprite Icon;
    }
}