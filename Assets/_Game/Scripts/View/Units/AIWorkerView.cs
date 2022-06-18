using System;
using _Game.Scripts.Factories;
using _Game.Scripts.Tools;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace _Game.Scripts.View.Units
{
    public enum WorkerState
    {
        None,
        GoToGrid,
        Stop,
        GoBack,
        EndWay
    }

    public enum WorkerAnimation
    {
        None,
        Idle,
        Walk
    }

    public class AIWorkerView : BaseView
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _box;
        [SerializeField] private Transform _boxPoint;

        [Inject] private AIFactory _aiFactory;
        
        private WorkerState _state;
        private Transform _stopPoint;
        private Transform _endPoint;
        private Tween _jumpTween;

        private const string ANIMATION_SPEED = "Speed";

        public event Action<AIWorkerView> OnStop;

        public class Pool : MonoMemoryPool<AIWorkerView>
        {
            protected override void Reinitialize(AIWorkerView item)
            {
                item.Reset();
            }
        }

        protected override void Reset()
        {
            _box.Activate();
            
            base.Reset();
        }

        public void Tick(float deltaTime)
        {
            switch (_state)
            {
                case WorkerState.GoToGrid:
                    if (_agent.pathPending)
                        break;
                    if (_agent.remainingDistance < 0.1f)
                    {
                        SetState(WorkerState.Stop);
                    }
                    break;
                case WorkerState.GoBack:
                    if (_agent.pathPending)
                        break;
                    if (_agent.remainingDistance < 0.1f)
                    {
                        SetState(WorkerState.EndWay);
                    }
                    break;
            }
        }

        public void JumpBox(Vector3 position, float time)
        {
            _jumpTween?.Kill();
            _jumpTween = _box.transform.DOJump(position, 0.5f, 1, time).OnComplete(() => SetState(WorkerState.GoBack));
        }

        public void SetPosition(Vector3 position)
        {
            _agent.Warp(position);
        }

        public void SetMovementPoints(Transform stopPoint, Transform endPoint)
        {
            _stopPoint = stopPoint;
            _endPoint = endPoint;
            SetState(WorkerState.GoToGrid);
        }

        private void SetState(WorkerState state)
        {
            if(_state == state) return;

            _state = state;

            switch (state)
            {
                case WorkerState.GoToGrid:
                    PlayAnimation(WorkerAnimation.Walk);
                    _agent.isStopped = false;
                    _agent.SetDestination(_stopPoint.position);
                    break;
                case WorkerState.Stop:
                    PlayAnimation(WorkerAnimation.Idle);
                    _agent.isStopped = true;
                    OnStop?.Invoke(this);
                    break;
                case WorkerState.GoBack: 
                    PlayAnimation(WorkerAnimation.Walk);
                    _box.Deactivate();
                    _box.transform.position = _boxPoint.position;
                    _agent.isStopped = false;
                    _agent.SetDestination(_endPoint.position);
                    break;
                case WorkerState.EndWay:
                    _aiFactory.RemoveWorker(this);
                    break;
            }
        }

        private void PlayAnimation(WorkerAnimation animation)
        {
            switch (animation)
            {
                case WorkerAnimation.Idle:
                    _animator.SetFloat(ANIMATION_SPEED, 0);
                    break;
                case WorkerAnimation.Walk:
                    _animator.SetFloat(ANIMATION_SPEED, 1);
                    break;
            }
        }

        public override void OnDestroy()
        {
            _jumpTween?.Pause();
            _jumpTween?.Kill();
            
            base.OnDestroy();
        }
    }
}