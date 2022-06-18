using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Balance;
using _Game.Scripts.View.Line;
using _Game.Scripts.View.Points;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.View.Animal
{
    public enum AnimalState
    {
        None,
        Idle,
        Move,
        Comeback,
    }
    
    public class BaseAnimal : BaseView
    {
        [SerializeField] private Alignment _moveSide;
        [SerializeField] private List<GridItem> _targets;

        [Inject] private GameBalanceConfigs _balance;

        private int _targetId;
        private List<CollisionListener> _collisionListeners;
        private AnimalState _state;
        private Vector3 _alignmentVector;
        private float _speed;
        private Vector3 _startPosition;

        protected override void Reset()
        {
            base.Reset();
            Init();
        }

        public override void Init()
        {
            _targetId = 0;

            _collisionListeners = GetComponentsInChildren<CollisionListener>().ToList();
            foreach (var collisionListener in _collisionListeners)
            {
                
            }
            _speed = _balance.DefaultBalance.AnimalSpeed;
            SetState(AnimalState.Idle);
            
            base.Init();
        }

        public void Tick(float deltaTime)
        {
            switch (_state)
            {
                case AnimalState.Idle:
                    break;
                case AnimalState.Move:
                    var newPosition =
                        Vector3.MoveTowards(transform.position, _targets[_targetId].transform.position, _speed * deltaTime);
                    transform.position = newPosition;
                    if (Vector3.Distance(transform.position, _targets[_targetId].transform.position) < 0.01f)
                    {
                        NextTarget();
                    }
                    break;
                case AnimalState.Comeback:
                    break;
            }
        }

        public void TryMove()
        {
            if(_state != AnimalState.Idle) return;
            if (_startPosition == Vector3.zero)
            {
                _startPosition = transform.position;
            }
            _targetId = -1;
            NextTarget();
        }

        private void NextTarget()
        {
            _targetId++;
            if (_targetId == _targets.Count)
            {
                //TODO: STOP
                SetState(AnimalState.Idle);
            }
            else
            {
                if (!_targets[_targetId].CollisionListener.Connected)
                {
                    SetState(AnimalState.Move);
                }
                else
                {
                    //TODO: STOP
                    SetState(AnimalState.Idle);
                }
            }
        }

        private void SetState(AnimalState state)
        {
            if(_state == state) return;
            _state = state;
            switch (_state)
            {
                case AnimalState.Idle:
                    break;
                case AnimalState.Move:
                    break;
                case AnimalState.Comeback:
                    break;
            }
        }
    }
}