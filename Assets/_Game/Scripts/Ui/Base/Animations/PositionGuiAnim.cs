using System;
using _Game.Scripts.Tools;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Scripts.Ui.Base.Animations
{
    public class PositionGuiAnim : BaseUIAnimation
    {
        [SerializeField] private Vector2 _outOffScreenPos;
        [SerializeField] private Direction _direction;

        private Vector2? _defaultPos;

        public override void Init()
        {
            _defaultPos = RectTransform.anchoredPosition;
            base.Init();
        }

        public override void PlayOpenAnimation(RectTransform windowRect, Action callback = null)
        {
            RectTransform.anchoredPosition = _outOffScreenPos;
            Tween = RectTransform.DOAnchorPos(_defaultPos ?? Vector2.zero, DurationShow)
                .SetEase(CurveShow)
                .SetUpdate(true)
                .OnComplete(() => callback?.Invoke());
        }

        public override void PlayCloseAnimation(RectTransform windowRect, Action callback = null)
        {
            Tween = RectTransform.DOAnchorPos(_outOffScreenPos, DurationHide)
                .SetEase(CurveHide)
                .SetUpdate(true)
                .OnComplete(() => callback?.Invoke());
        }

#if ODIN_INSPECTOR
        [Button("Calculate off screen pos")]
#endif
        [ContextMenu("Calculate off screen pos")]
        public void CalculateOffscreenPos()
        {
            if (_direction == Direction.None)
            {
                _direction = GetHideDirection();
                if (_direction == Direction.None)
                {
                    _direction = Direction.Up;
                }
            }

            var outOffScreenDelta = GetOutOffScreenDelta(_direction);
            _outOffScreenPos = RectTransform.anchoredPosition + outOffScreenDelta;
        }
        
        private Direction GetHideDirection()
        {
            var anchorMin = RectTransform.anchorMin;
            if (anchorMin.x.EqualTo(0f))
            {
                return anchorMin.x.EqualTo(anchorMin.y) ? Direction.Down : Direction.Left;
            }

            if (anchorMin.y.EqualTo(0f)) return Direction.Down;
            if (RectTransform.anchorMax.x.EqualTo(1f)) return Direction.Right;
            if (RectTransform.anchorMax.y.EqualTo(1f)) return Direction.Up;

            return Direction.None;
        }
        
        private Vector2 GetOutOffScreenDelta(Direction direction = Direction.None)
        {
            if (direction == Direction.None) direction = GetHideDirection();

            var delta = IsVertical(direction)
                ? RectTransform.anchoredPosition.y.Abs() + RectTransform.rect.height
                : RectTransform.anchoredPosition.x.Abs() + RectTransform.rect.width;

            return ToVec2(direction) * delta;
        }
        
        private bool IsVertical(Direction direction)
        {
            return direction is Direction.Up or Direction.Down;
        }
        
        private static Vector2 ToVec2(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return Vector2.up;

                case Direction.Right:
                    return Vector2.right;

                case Direction.Down:
                    return Vector2.down;

                case Direction.Left:
                    return Vector2.left;
            }

            return new Vector2();
        }
    }
}