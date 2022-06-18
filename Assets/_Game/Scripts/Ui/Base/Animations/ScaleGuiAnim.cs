using System;
using DG.Tweening;
using UnityEngine;

namespace _Game.Scripts.Ui.Base.Animations
{
    public class ScaleGuiAnim : BaseUIAnimation
    {
        [SerializeField] private float _showStartScale = 0.5f;
        [SerializeField] private float _hideEndScale = 0.7f;
        
        public override void PlayOpenAnimation(RectTransform windowRect, Action callback = null)
        {
            var rect = windowRect == null ? CacheRect : windowRect;
            
            rect.localScale = Vector3.one * _showStartScale;

            Tween?.Kill();
            Tween = rect.DOScale(Vector3.one, DurationShow)
                .SetEase(CurveShow)
                .SetUpdate(true)
                .OnComplete(() => callback?.Invoke());
        }
        
        public override void PlayCloseAnimation(RectTransform windowRect, Action callback = null)
        {
            var rect = windowRect == null ? CacheRect : windowRect;
            
            Tween?.Kill();
            Tween = rect.DOScale(Vector3.one * _hideEndScale, DurationHide)
                .SetEase(CurveHide)
                .SetUpdate(true)
                .OnComplete(() => callback?.Invoke());
        }
        
        public ScaleGuiAnim CopyFrom(ScaleGuiAnim source)
        {
            if (!source) return this;
            DurationShow = source.DurationShow;
            DurationHide = source.DurationHide;

            CurveShow = source.CurveShow;
            CurveHide = source.CurveHide;

            _showStartScale = source._showStartScale;
            _hideEndScale = source._hideEndScale;

            return this;
        }
    }
}