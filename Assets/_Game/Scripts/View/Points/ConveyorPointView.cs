using _Game.Scripts.Enums;
using _Game.Scripts.Factories;
using _Game.Scripts.Interfaces;
using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.Systems;
using _Game.Scripts.Tools;
using _Game.Scripts.View.Animations;
using _Game.Scripts.View.CollectableItems;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.View.Points
{
    public class ConveyorPointView : BasePointView, IGameProgress, IAnimationEventsListener
    {
        [SerializeField] private InteractiveView _inputPoint;
        [SerializeField] private InteractiveView _outputPoint;
        [SerializeField] private PointStorageView _inputStorage;
        [SerializeField] private PointStorageView _outputStorage;
        
        [Header("ProductionSettings")]
        [SerializeField] private Transform _outputSpawnPoint;
        [SerializeField] private Transform _outputTransitPoint;
        [SerializeField] private Animation _conveyorAnimation;
        
        [Inject] private CollectableItemsFactory _items;

        private AnimationEventsSender _animationEventsSender;
        
        private GameProgress _cycle;
        private float _count;
        private CollectableItem _currentItem;
        
        public override void OnDestroy()
        {
            Progresses.RemoveProgresses(this);
            base.OnDestroy();
        }
        
        public override void SetConfig(BasePointConfig config)
        {
            _inputStorage.Init();
            _outputStorage.Init();
            
            _animationEventsSender = GetComponentInChildren<AnimationEventsSender>();
            _animationEventsSender.AssignListener(this);
            
            _cycle = Progresses.CreateProgress(this, GameParamType.Timer,
                config.GetParam(0, GameParamType.Timer).BaseValue, false);
            _cycle.Pause();
            _cycle.CompletedEvent += PlayProduction;

            _count = (int) config.GetParam(0, GameParamType.Res2).BaseValue;
            
            _inputPoint.Activate();
            _inputPoint.OnTriggerEnterAction += OnInputTriggerEnter;

            _outputPoint.Activate();
            _outputPoint.OnTriggerEnterAction += OnOutputTriggerEnter;

            base.SetConfig(config);
        }

        private void OnInputTriggerEnter(Collider other)
        {
        }

        private void PlayProduction()
        {
            if (_cycle.IsActive) return;
            
            var items = _items.GetItemFromParent(_inputStorage, _inputStorage.ParamType);
            if (items.Count == 0) return;

            _currentItem = items.LastValue();
            _currentItem.MoveTo(_outputSpawnPoint.position, 0.1f);
            _currentItem.OnMoved += OnMovedToSpawnPoint;
        }

        private void OnMovedToSpawnPoint(CollectableItem item, bool b)
        {
            _currentItem.OnMoved -= OnMovedToSpawnPoint;
            _conveyorAnimation.Play();
        }

        private void OnOutputTriggerEnter(Collider other)
        {
        }

        public void ExecuteEvent(AnimationEventType eventType)
        {
            switch (eventType)
            {
                case AnimationEventType.ConveyorCycleStart:
                    _items.RemoveItem(_currentItem);
                    break;
                
                case AnimationEventType.ConveyorCycleComplete:
                    var item = _items.SpawnItem(_outputStorage, _outputStorage.ParamType, _outputSpawnPoint.position);
                    item.OnMoved += OnMovedItem;
                    item.MoveTo(_outputTransitPoint.position, 0.1f);
                    break;
            }
        }

        private void OnMovedItem(CollectableItem item, bool b)
        {
            item.OnMoved -= OnMovedItem;
            var pos = _items.GetPosition(item, _outputStorage, _outputStorage.StorageType, true);
            item.LocalMoveTo(pos, 0.1f);
            _cycle.Play();
        }
    }
}