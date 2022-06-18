using _Game.Scripts.Interfaces;
using _Game.Scripts.Systems.Tutorial;
using _Game.Scripts.Tools;
using UnityEngine;

namespace _Game.Scripts.View
{
    public class TargetArrow : BaseView, ITickableSystem
    {
        private ArrowState _state;
        private Transform _target;
        private Vector3 _offset;

        public void Show(Transform target, Vector3 offset = default)
        {
            this.Activate();
            
            _state = ArrowState.Show;
            _target = target;

            if (offset == default)
            {
                _offset = target.GetComponentInChildren<CustomTutorialTarget>()?.Offset ?? Vector3.zero;
            }
            else
            {
                _offset = offset;                
            }

            transform.position = target.position + _offset;
        }

        public void Hide()
        {
            this.Deactivate();
            _state = ArrowState.Hide;
            _target = null;
        }

        public void Tick(float deltaTime)
        {
            switch (_state)
            {
                case ArrowState.Show:
                    LookAt();
                    break;
            }
        }
        
        private void LookAt()
        {
            transform.position = _target.position + _offset;
        }
    }
}