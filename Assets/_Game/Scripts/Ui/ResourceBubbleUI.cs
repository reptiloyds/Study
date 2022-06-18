using System;
using _Game.Scripts.Core;
using _Game.Scripts.Systems.Base;
using _Game.Scripts.Ui.Base;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

namespace _Game.Scripts.Ui
{
    public class ResourceBubbleUI : BaseUIView
    {
        private enum Phase
        {
            First,
            Second
        }

        public Action<ResourceBubbleUI> OnFinished;
        
        [SerializeField] private Image _icon;
        [Range(0f, 1f), SerializeField] private float _firstPhaseDuration = 0.2f;
        [Range(0f, 1f), SerializeField] private float _secondPhaseDelay = 0.2f;
        [Range(0f, 1f), SerializeField] private float _secondPhaseDuration = 0.2f;

        [Inject] private SceneData _sceneData;

        private RectTransform _rect;
        private Vector2 _startMovePos, 
                        _middlePos, 
                        _targetPos,
                        _delta;
        
        private Phase _phase;
        private Tweener _tween;
        private float _progress;

        public class Pool : MonoMemoryPool<Transform, Vector3, Vector3, Sprite, float, ResourceBubbleUI>
        {
            protected override void Reinitialize(Transform parent, 
                Vector3 startPos, 
                Vector3 targetPos, 
                Sprite icon, 
                float delay, 
                ResourceBubbleUI resourceBubbleUI)
            {
                resourceBubbleUI.SetConfig(parent, startPos, targetPos, icon, delay);
            }
        }

        private void SetConfig(Transform parent, Vector3 startPos, Vector3 targetPos, Sprite icon, float delay)
        {
            transform.SetParent(parent);

            _phase = Phase.First;
            
            _startMovePos = _middlePos = startPos;
            _targetPos = targetPos;
            
            _middlePos = _startMovePos + Random.insideUnitCircle * 100;
            _delta = _middlePos - _startMovePos;

            _icon.sprite = icon;

            _rect = GetComponent<RectTransform>();
            _rect.position = _startMovePos;
            _rect.localScale = Vector3.one;
            
            StartTween(_firstPhaseDuration, delay);
        }

        private void StartTween(float duration, float delay = 0f)
        {
            _progress = 0f;
            _tween.Kill();
            _tween = DOTween.To(() => _progress, v => _progress = v, 1, duration)
                .SetDelay(delay)
                .SetEase(_phase == Phase.First ? Ease.OutCubic : Ease.InCubic)
                .SetUpdate(UpdateType.Manual)
                .OnUpdate(OnUpdate)
                .OnComplete(OnTweenPlayed);
        }

        private void OnUpdate()
        {
            var pos = _startMovePos + _progress * _delta;
            _rect.position = pos;
        }
        
        private void OnTweenPlayed()
        {
            if (_phase == Phase.Second)
            {
                Hide();
                return;
            }
            
            _phase = Phase.Second;

            _startMovePos = _middlePos;
            _delta = _targetPos - _startMovePos;

            StartTween(_secondPhaseDuration, _secondPhaseDelay);
        }

        private void Hide()
        {
            _tween?.Kill();
            OnFinished?.Invoke(this);
        }
    }
}