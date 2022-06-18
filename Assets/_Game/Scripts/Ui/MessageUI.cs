using System;
using _Game.Scripts.Ui.Base;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Game.Scripts.Ui
{
    public class MessageUI : BaseUIView
    {
        public Action<MessageUI> OnFinished;
        
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _back;
        [SerializeField] private Animation _animation;

        [SerializeField] private Color _defaultBackColor;
        [SerializeField] private Color _defaultTextColor;

        private RectTransform _rect;
        private float _progress;
        private const float DURATION = 2f;
        
        private Tweener _moveTween;
        private Tweener _visibleTween;
        
        public class Pool : MonoMemoryPool<Transform, string, MessageUI>
        {
            protected override void Reinitialize(Transform partType, string part, MessageUI orderUI)
            {
                orderUI.SetConfig(partType, part);
            }
        }
        
        private void SetConfig(Transform parent, string text)
        {
            transform.SetParent(parent);

            _rect = GetComponent<RectTransform>();
            _rect.localScale = Vector3.one;
            _rect.anchoredPosition = Vector2.zero;
            
            _text.text = text;
            
            _back.color = _defaultBackColor;
            _text.color = _defaultTextColor;
            
            _moveTween?.Kill();
            _moveTween = _rect.DOLocalMoveY(120, 0.5f);
            
            _progress = 0f;
            _visibleTween?.Kill();
            _visibleTween = DOTween.To(() => _progress, v => _progress = v, 1, DURATION)
                                   .OnComplete(Hide);
        }
        
        private void Hide()
        {
            _animation.Play();
            _moveTween?.Kill();
            _moveTween = _rect.DOLocalMoveY(_rect.anchoredPosition.y + 120, 0.5f)
                              .OnComplete(OnCompleted);
        }
        
        private void OnCompleted()
        {
            _visibleTween.Kill();
            _visibleTween = null;
            
            _moveTween.Kill();
            _moveTween = null;
            
            OnFinished?.Invoke(this);
        }
        
        public void Move(float newPos)
        {
            _moveTween?.Kill();
            _moveTween = _rect.DOLocalMoveY(newPos, 0.2f);
        }
    }
}