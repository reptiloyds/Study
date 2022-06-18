using System;
using _Game.Scripts.Enums;
using _Game.Scripts.Interfaces;
using Sirenix.Serialization;
using Zenject;

namespace _Game.Scripts.Systems
{
    public enum GameProgressState
    {
        None,
        Active,
        Completed,
        Paused
    }

    public class GameProgress
    {
        [OdinSerialize] private GameProgressState _state;
        [OdinSerialize] private GameParamType _type;
        [OdinSerialize] private float _target;
        [OdinSerialize] private float _current;
        [OdinSerialize] private bool _looped;
        [OdinSerialize] private bool _updatable;
        [OdinSerialize] private float _delta;
        [OdinSerialize] private string _ownerStr => Owner.ToString();

        public event Action
            PlayEvent,
            UpdatedEvent, 
            CompletedEvent, 
            DespawnedEvent;

        public IGameProgress Owner { get; private set; }
        public GameParamType Type => _type;

        public bool IsActive => _state == GameProgressState.Active;
        public bool IsCompleted => _state == GameProgressState.Completed;
        public bool IsPaused => _state == GameProgressState.Paused;
        
        public float ProgressValue => _current != 0 ? _current / _target : 0;
        public float CurrentValue => _current;
        public float TargetValue => _target;
        public float LeftValue => _target - _current;

        public GameProgress()
        {
            _state = GameProgressState.Active;
        }
        
        public class Pool : MemoryPool<IGameProgress, GameParamType, float, bool, bool, GameProgress>
        {
            protected override void Reinitialize(IGameProgress owner, GameParamType type, float target, bool looped, bool updatable, GameProgress progress)
            {
                progress.Init(owner, type, target, looped, updatable);
            }
        }
        
        private void Init(IGameProgress owner, GameParamType type, float target, bool looped, bool updatable)
        {
            Owner = owner;
            _type = type;
            _target = target;
            _looped = looped;
            _updatable = updatable;
            _current = 0;
        }

        public GameProgress Play()
        {
            if (_state == GameProgressState.Paused || _state == GameProgressState.Completed)
            {
                SetState(GameProgressState.Active);
            }
            
            PlayEvent?.Invoke();
            
            return this;
        }

        public GameProgress Pause()
        {
            if (_state == GameProgressState.Active)
            {
                SetState(GameProgressState.Paused);
            }

            return this;
        }
        
        public void Tick(float deltaTime)
        {
            if (_state != GameProgressState.Active || !_updatable) return;
            Change(deltaTime);
        }

        public void Change(float delta, bool checkProgress = true)
        {
            _current += delta;
            UpdatedEvent?.Invoke();
            if (checkProgress) CheckProgress();
        }

        private void CheckProgress()
        {
            if (_state != GameProgressState.Active) return;
            if (_current < _target)
            {
                return;
            }

            if (_looped)
            {
                _current = 0;
            }
            else
            {
                _current = _target;
                SetState(GameProgressState.Completed);
            }
            
            CompletedEvent?.Invoke();
        }

        public GameProgress Reset()
        {
            _state = GameProgressState.Active;
            _current = 0;
            CheckProgress();
            UpdatedEvent?.Invoke();
            return this;
        }

        public void SetTarget(float target)
        {
            _current = 0;
            _target = target;
        }

        private void SetState(GameProgressState state)
        {
            if (_state == state) return;
            _state = state;
        }

        public void Despawned()
        {
            PlayEvent = null;
            UpdatedEvent = null;
            CompletedEvent = null;
            
            DespawnedEvent?.Invoke();
            DespawnedEvent = null;
        }
        
        public override string ToString()
        {
            return $"{_current} / {_target}";
        }
    }
}