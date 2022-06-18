using _Game.Scripts.Enums;
using _Game.Scripts.Systems.Base;
using UnityEngine;

namespace _Game.Scripts.Systems.GamePlayElements
{
    public class BaseGamePlayElement : MonoBehaviour
    {
        [SerializeField] private GamePlayElement _type;
        
        protected WindowsSystem Windows; 
        
        private Transform _parent;
        private RectTransform _rect;
        private Vector2 _defaultPos;
        
        public GamePlayElement Type => _type;

        public virtual void Init(WindowsSystem windows)
        {
            Windows = windows;
            
            _parent = transform.parent;
            _rect = GetComponent<RectTransform>();
            _defaultPos = _rect.anchoredPosition;
        }

        public virtual void RestorePosition()
        {
            transform.SetParent(_parent);
            _rect.anchoredPosition = _defaultPos;
        }
    }
}