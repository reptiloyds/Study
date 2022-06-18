using _Game.Scripts.Interfaces;
using _Game.Scripts.Tools;
using UnityEngine;

namespace _Game.Scripts.View
{
    public enum ArrowState
    {
        None,
        Show,
        Hide,
    }
    
    public class PlayerArrow : BaseView, ITickableSystem
    {
        [SerializeField] private Transform _bodyArrow;
        
        private ArrowState _state;
        private Transform _target;

        public void Show(Transform target)
        {
            _bodyArrow.Activate();
            _target = target;
            _state = ArrowState.Show;
        }

        public void Hide()
        {
            _bodyArrow.Deactivate();
            _state = ArrowState.Hide;
            _target = null;
        }

        public void Tick(float deltaTime)
        {
            switch (_state)
            {
                case ArrowState.Show:
                    LookAt(_target.position);
                    break;
            }
        }
        
        private void LookAt(Vector3 targetPosition)
        {
            var lookPos = targetPosition - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = rotation;
        }
    }
}