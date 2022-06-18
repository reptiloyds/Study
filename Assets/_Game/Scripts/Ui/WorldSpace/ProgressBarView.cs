using System;
using _Game.Scripts.Systems;
using _Game.Scripts.Tools;
using _Game.Scripts.View;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Game.Scripts.UI.WorldSpace
{
    public class ProgressBarView : BaseView
    {
        public Action<ProgressBarView> DespawnedEvent;

        [SerializeField] private Image _bar;
        [SerializeField] private Color _playerColor;
        [SerializeField] private Color _enemiesColor;

        [Inject] private GameCamera _gameCamera;

        private GameProgress _progress;
        
        public class Pool : MonoMemoryPool<Transform, Vector3, int, GameProgress, bool, ProgressBarView>
        {
            protected override void Reinitialize(Transform parent, 
                Vector3 position, 
                int type,
                GameProgress progress,
                bool updatable,
                ProgressBarView bubble)
            {
                bubble.Reset(parent, position, type, progress, updatable);
            }
        }

        private void Reset(Transform parent, Vector3 position, int type, GameProgress progress, bool updatable)
        {
            _progress = progress;
            if (updatable) _progress.UpdatedEvent += UpdateProgress;
            _progress.DespawnedEvent += Despawned;
            
            var transformCache = transform;
            transformCache.SetParent(parent);
            transformCache.position = position;
            
            _bar.color = type == 0 ? _playerColor : _enemiesColor;

            UpdateProgress();
            base.Reset();

            RotateToCamera();
        }

        private void UpdateProgress()
        {
            var delta = 1 - _progress.ProgressValue;
            _bar.fillAmount = delta;
            this.SetActive(delta < 1);
        }

        public void SetScale(Vector3 scale)
        {
            transform.localScale = scale;
        }

        public void UpdatePosition(Vector3 position)
        {
            transform.position = position;
            transform.localRotation = new Quaternion(0, 0, 0, 1);
        }
        
        private void RotateToCamera()
        {
            transform.LookAt(transform.position + _gameCamera.VirtualCamera.transform.forward);
        }
        
        private void Despawned()
        {
            if (_progress != null)
            {
                _progress.UpdatedEvent -= UpdateProgress;
                _progress.DespawnedEvent -= Despawned;
                _progress = null;   
            }
            DespawnedEvent?.Invoke(this);
        }
    }
}

