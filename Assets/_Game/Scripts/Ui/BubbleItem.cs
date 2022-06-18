using _Game.Scripts.Enums;
using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.Tools;
using _Game.Scripts.Ui.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Game.Scripts.Ui
{
    public class BubbleItem : BaseUIView
    {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _text;
        [Inject] private GameResources _resources;
        private GameParamType _type;
        private int _count;
        
        public bool IsFree => _type == GameParamType.None;

        public GameParamType Type => _type;

        public void SetType(GameParamType type)
        {
            _type = type;
            _image.sprite = _resources.GetSprite(type);
        }

        public void SetCount(int count)
        {
            _count = count;
            Redraw();
        }

        public void DecreaseCount()
        {
            _count--;
            Redraw();
            if (_count == 0)
            {
                Clear();
            }
        }

        private void Redraw()
        {
            _text.text = $"{_count}";
        }

        public void Clear()
        {
            _type = GameParamType.None;
            this.Deactivate();
        }
    }

    public class RequireItem
    {
        public GameParamType Type;
        public int Count;

        public RequireItem(GameParamType type, int count)
        {
            Type = type;
            Count = count;
        }
    }
}