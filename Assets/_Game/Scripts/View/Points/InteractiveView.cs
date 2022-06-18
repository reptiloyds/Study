using System;
using _Game.Scripts.Tools;
using TMPro;
using UnityEngine;

namespace _Game.Scripts.View.Points
{
    public class InteractiveView : BaseView
    {
        public Action<Collider> OnTriggerEnterAction;
        public Action<Collider> OnTriggerExitAction;
        
        [SerializeField] private TextMeshPro _text;
        [SerializeField] private GameObject _outline;
        [SerializeField] private GameObject _pointImage;
        [SerializeField] private ParticleSystem[] _buyVfx;
        [SerializeField] private Collider _collider;
        [SerializeField] private Animation _animation;

        public void SetText(string text)
        {
            _text.text = text;
        }

        public void PlayAnimation()
        {
            _animation.Play();
        }

        public void StopAnimation()
        {
            _animation.Stop();
            _animation.Rewind();
            transform.localScale = Vector3.one;
        }
        
        public void HidePointImage()
        {
            _pointImage.Deactivate();
        }

        public void PlayVfx()
        {
            foreach (var vfx in _buyVfx)
            {
                vfx.Play();
            }
        }

        public void Show()
        {
            _text.Activate();
            _outline.Activate();
            _collider.enabled = true;
        }
        
        public void Hide()
        {
            _text.Deactivate();
            _outline.Deactivate();
            _collider.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnterAction?.Invoke(other);
        }
        
        private void OnTriggerExit(Collider other)
        {
            OnTriggerExitAction?.Invoke(other);
        }
    }
}