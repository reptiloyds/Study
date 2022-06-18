using _Game.Scripts.Enums;
using _Game.Scripts.Ui.Base;
using TMPro;
using UnityEngine;

namespace _Game.Scripts.Ui
{
    public class ResourceItem : BaseUIView
    {
        [SerializeField] private GameParamType _type;
        [SerializeField] private TextMeshProUGUI _text;

        public GameParamType Type => _type;
        
        public void Redraw(int count)
        {
            _text.text = $"{count}<sprite name={_type}>";
        }
    }
}