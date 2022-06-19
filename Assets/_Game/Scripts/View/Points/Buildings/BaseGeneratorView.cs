using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Enums;
using _Game.Scripts.Factories;
using _Game.Scripts.Interfaces;
using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.Systems;
using _Game.Scripts.Tools;
using _Game.Scripts.View.CollectableItems;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.View.Points.Buildings
{
    public class BaseGeneratorView : BasePointView, IGameProgress, IGameParam, ITickableSystem
    {
        [SerializeField] private PointStorageView _spawnStorage;
        [SerializeField] private PointStorageView _buyStorage;
        [SerializeField] private InteractiveView _buyView;
        [SerializeField] private InteractiveView _collectView;
        [SerializeField] private GameParamType _spawnItemType;
        [SerializeField] private GameObject _body;
        [SerializeField] private Animation _buyEffect;

        [Inject] private GameProgressFactory _progresses;
        [Inject] private GameParamFactory _params;
        [Inject] private CollectableItemsFactory _collectableItemsFactory;
        [Inject] private PointsFactory _points;
        [Inject] private AppEventProvider _appEventProvider;

        private bool _playerEnter;
        private GameProgress _spawnItemDelay;
        private GameProgress _capacity;

        public override void OnDestroy()
        {
            _collectableItemsFactory.OnTransactionFromTo -= OnTransaction;
            _buyView.OnTriggerEnterAction -= OnBuyTriggerEnter;
            _buyView.OnTriggerExitAction -= OnBuyTriggerExit;
            _collectView.OnTriggerEnterAction -= OnCollectTriggerEnter;
            _collectView.OnTriggerExitAction -= OnCollectTriggerExit;

            _progresses.RemoveProgresses(this);
            base.OnDestroy();
        }

        public override void SetConfig(BasePointConfig config)
        {
            _collectableItemsFactory.OnTransactionFromTo += OnTransaction;
            
            var levelConfig = config.Levels.FirstOrDefault();
            if (levelConfig == null) return;

            base.SetConfig(config);

            foreach (var param in config.Levels.FirstValue().Params)
            {
                _params.CreateParam(this, param.ParamType, param.BaseValue);
            }

            _spawnItemDelay = _progresses.CreateProgress(this, GameParamType.Timer, _params.GetParam(this, GameParamType.Timer).Value);
            _spawnItemDelay.Pause();
            _spawnItemDelay.CompletedEvent += TrySpawnItem;

            _capacity = _progresses.CreateProgress(this, GameParamType.Capacity, _params.GetParam(this, GameParamType.Capacity).Value, false, false);

            _buyView.OnTriggerEnterAction += OnBuyTriggerEnter;
            _buyView.OnTriggerExitAction += OnBuyTriggerExit;
            _collectView.OnTriggerEnterAction += OnCollectTriggerEnter;
            _collectView.OnTriggerExitAction += OnCollectTriggerExit;

            _spawnStorage.Init();
            _buyStorage.Init();
            _body.SetActive(_isBought);
            
            UpdateInfo();
        }

        private void OnTransaction(CollectableItem item, IStorage from, IStorage to)
        {
            if (from == _spawnStorage as IStorage)
            {
                _capacity.Change(-1);
            }
        }

        private void OnBuyTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == GameLayers.PLAYER_LAYER)
            {
                if (!_isBought)
                {
                    SpendCurrency();
                }
            }
        }
        
        private void OnBuyTriggerExit(Collider other)
        {
        }

        private void OnCollectTriggerEnter(Collider other)
        {
        }
        
        private void OnCollectTriggerExit(Collider other)
        {
        }

        private void SpendCurrency()
        {
            if(IsCap(Level)) return;
            var prices = Prices;
            foreach (var price in prices)
            {
                var delta = price.Target - price.Current;
                if (delta > 0)
                {
                    //_points.BuyPoint(this, _buyStorage, price.Type, (int)delta);
                }
            }
        }

        private void TrySpawnItem()
        {
            if(_playerEnter && _capacity.CurrentValue > 0) return;
            if (_capacity.CurrentValue >= _capacity.TargetValue)
            {
                //TODo перезапускать цикл, когда игрок забрал предмет
                return;
            }

            var item = _collectableItemsFactory.SpawnItem(_spawnStorage, _spawnItemType);
            if (item != null)
            {
                var pos = _collectableItemsFactory.GetPosition(item, _spawnStorage);
                item.SetLocalPosition(pos);
                item.SetFuturePosition(pos);
                item.transform.localRotation = Quaternion.identity;
                _capacity.Change(1);
            }
        }

        public override bool Buy(GameParamType type, float value)
        {
            var returnValue = base.Buy(type, value);

            UpdateInfo();

            return returnValue;
        }

        protected override void CompleteBuy(bool byPlayer = true)
        {
            _body.SetActive(true);
            _buyView.Hide();
            _spawnItemDelay.Play();

            if (byPlayer)
            {
                _buyView.PlayVfx();
                _buyEffect.Play();
            }
            base.CompleteBuy(byPlayer);
        }
        

        protected override void UpdateInfo()
        {
            _buyView.SetText(GetPriceText());
            
            base.UpdateInfo();
        }
    }
}