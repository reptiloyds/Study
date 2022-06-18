using TMPro;
using UnityEngine;

namespace _Game.Scripts.View.Fx
{
    public class TextFx : BaseFx
    {
        [SerializeField] private TextMeshPro _tmp;

        public TextFx Init(string text = "")
        {
            _tmp.text = text;
            return this;
        }
    }
}
