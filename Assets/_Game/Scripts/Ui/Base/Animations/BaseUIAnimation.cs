using System;
using DG.Tweening;
using UnityEngine;

namespace _Game.Scripts.Ui.Base.Animations
{
    public class BaseUIAnimation : BaseUIView
    {
        [SerializeField] private RectTransform _target;
        [SerializeField] protected float DurationShow = 0.3f, DurationHide = 0.1f;

        [SerializeField] protected AnimationCurve CurveShow = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] protected AnimationCurve CurveHide = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        protected RectTransform CacheRect;
        protected Tweener Tween;
        
        public virtual void Init()
        {
            CacheRect = _target != null ? _target : RectTransform;
        }

        public virtual void PlayOpenAnimation(RectTransform windowRect, Action callback = null)
        {
        }

        public virtual void PlayCloseAnimation(RectTransform windowRect, Action callback = null)
        {
        }
    }
}