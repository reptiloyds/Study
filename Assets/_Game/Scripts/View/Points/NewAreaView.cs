using System;
using _Game.Scripts.Enums;
using _Game.Scripts.Factories;
using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.Tools;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.View.Points
{
    public class NewAreaView : BasePointView
    {
        [SerializeField] private InteractiveView _interactive;
        [SerializeField] private PointStorageView _pointStorageView;
        [SerializeField] private GameObject _body;
        [SerializeField] private GameObject _preBuy;
        [SerializeField] private Animation _buyAnimation;

        [Inject] private CollectableItemsFactory _collectableItemsFactory;
        [Inject] private PointsFactory _pointsFactory;

        public override void SetConfig(BasePointConfig config)
        {
            base.SetConfig(config);
            
            _preBuy.Activate();
            _body.Deactivate();
            _interactive.OnTriggerEnterAction += OnTriggerEnterAction;
            _interactive.OnTriggerExitAction += OnTriggerExitAction;
            UpdateInfo();
            
            _pointStorageView.Init();
        }

        public override void OnDestroy()
        {
            _interactive.OnTriggerEnterAction -= OnTriggerEnterAction;
            _interactive.OnTriggerExitAction -= OnTriggerExitAction;
            
            base.OnDestroy();
        }

        private void OnTriggerEnterAction(Collider other)
        {
            if (other.gameObject.layer == GameLayers.PLAYER_LAYER)
            {
                if (!_isBought)
                {
                    SpendCurrency();
                }
            }
        }

        private void OnTriggerExitAction(Collider other)
        {
            if (other.gameObject.layer == GameLayers.PLAYER_LAYER)
            {
            }
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
                    //_pointsFactory.BuyPoint(this, _pointStorageView, price.Type, (int)delta);
                }
            }
        }

        protected override void CompleteBuy(bool byPlayer = true)
        {
            _body.Activate();
            _preBuy.Deactivate();
            _interactive.Deactivate();

            if (byPlayer)
            {
                _interactive.PlayVfx();
                _buyAnimation.Play();
            }
            
            base.CompleteBuy(byPlayer);
        }

        public override bool Buy(GameParamType type, float value)
        {
            var returnValue = base.Buy(type, value);
            UpdateInfo();

            return returnValue;
        }

        protected override void UpdateInfo()
        {
            _interactive.SetText(GetPriceText());
            base.UpdateInfo();
        }
    }
}