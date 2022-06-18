using System.Linq;
using _Game.Scripts.Balance;
using _Game.Scripts.Core;
using _Game.Scripts.Enums;
using _Game.Scripts.Factories;
using _Game.Scripts.Interfaces;
using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.Systems;
using _Game.Scripts.Systems.Base;
using _Game.Scripts.Tools;
using _Game.Scripts.View.Animations;
using _Game.Scripts.View.CollectableItems;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.View.Units
{
    public class PlayerView : BaseUnitView, IStorage, IAnimationEventsListener, ITickableSystem, IInvoke
    {
        [SerializeField] private Transform _collectPoint;
        [SerializeField] private ItemStorageType _storageType;
        [SerializeField] private GameObject _visual;

        [Inject] private SceneData _sceneData;
        [Inject] private GameBalanceConfigs _balance;

        private LevelSystem _levels;

        private AnimationEventsSender _animationEventsSender;
        private Rigidbody _rigidbody;
        
        private Vector2 _directionClamped;
        private Vector3 _currentSpeed;
        private Vector3 _rotation;
        private Vector3 _deltaPosition;
        
        private float _capacity;
        
        private GameParam _capacityLevelParam;

        private Vector3 _lastItemPosition;
        
        public Transform Transform { get; set; }
        public ItemStorageType StorageType { get; set; }

        public Transform ItemsContainer 
        {
            get => _collectPoint;
            set {}
        }
        
        public int Columns { get; set; }
        public int Rows { get; set; }
        

        [Inject]
        public void Construct(LevelSystem levels, GameCamera GameCamera)
        {
            _levels = levels;
            _levels.OnLoadedLevel += OnLoaded;
            _levels.OnDestroyLevel += OnDestroy;

            _animationEventsSender = GetComponentInChildren<AnimationEventsSender>();
            _animationEventsSender.AssignListener(this);
            
            Animator = GetComponentInChildren<Animator>();
            _rigidbody = GetComponent<Rigidbody>();
            
            Animator = GetComponentInChildren<Animator>();
            StorageType = _storageType;
            Transform = transform;
            
            _capacityLevelParam = Params.CreateParam(this, GameParamType.CapacityLevel, 0);
            _capacityLevelParam.UpdatedEvent += UpdateCapacity;
         
            GameCamera.Follow(transform, false);
        }

        private void OnLoaded()
        {
            UnitConfig = Balance.DefaultBalance.Units.FirstOrDefault(item => item.UnitType == UnitType.Player);
            
            base.Init();
            
            CameraRotation =  Quaternion.Euler(0, _balance.DefaultBalance.YRotation, 0);
            //SetPosition(_levels.CurrentLevel.PlayerSpawnPoint.position);

            UpdateCapacity();
        }

        public bool IsFull(float value)
        {
            return _capacity <= value;
        }

        private void UpdateCapacity()
        {
            _capacity = Params.GetParamValue(this, GameParamType.Capacity) + _capacityLevelParam.Value;
        }

        public override void Tick(float deltaTime)
        {
            switch(State)
            {
                case UnitState.Moving:
                    Rotate();
                    break;
            }
            base.Tick(deltaTime);
        }

        public void MoveUnit(Vector2 direction)
        {
            Move(direction == Vector2.zero ? _deltaPosition : direction);
            SetState(direction == Vector2.zero ? UnitState.Idle : UnitState.Moving);
        }
        
        private void Move(Vector2 direction)
        {
            if (direction == Vector2.zero)
            {
                SetAnimatorFloat(AnimationFloat.MoveSpeed, 0);
                _rigidbody.velocity = Vector3.zero;
                return;
            }

            if (direction.magnitude > 2f)
            {
                _directionClamped = direction.normalized * 2f;
            }
            else
            {
                _directionClamped = direction;
            }
            
            _currentSpeed.x = Mathf.Clamp(_directionClamped.x, -2f, 2f);
            _currentSpeed.z = Mathf.Clamp(_directionClamped.y, -2f, 2f);
            _currentSpeed.y = 0;
            _currentSpeed = CameraRotation * _currentSpeed;
            
            var forwardedVelocity = _currentSpeed * Speed.Value;
            forwardedVelocity.y = 0;

            _rigidbody.velocity = forwardedVelocity;
            
            SetAnimatorFloat(AnimationFloat.MoveSpeed, forwardedVelocity.magnitude);
        }
        
        public void Stop()
        {
            SetAnimatorFloat(AnimationFloat.MoveSpeed, 0);
            _rigidbody.velocity = Vector3.zero;
        }

        private void Rotate()
        {
            if (_currentSpeed.magnitude > 0.01f)
            {
                _rotation = Quaternion.LookRotation(_currentSpeed).eulerAngles;
                _rotation.x = 0f;
                _rotation.z = 0f;
                transform.rotation = Quaternion.Euler(_rotation);
            }
        }
        
        private void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void ExecuteEvent(AnimationEventType eventType)
        {
            switch (eventType)
            {
            }
        }

        public void ActivateVisual()
        {
            _visual.Activate();
        }

        public void DeactivateVisual()
        {
            _visual.Deactivate();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}