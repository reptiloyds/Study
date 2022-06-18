using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Enums;
using _Game.Scripts.Factories;
using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.Systems;
using _Game.Scripts.Systems.Base;
using _Game.Scripts.Systems.Save.SaveStructures;
using _Game.Scripts.Tools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Purchasing;
using Zenject;

namespace _Game.Scripts.View.Points
{
    public class BasePointView : BaseView, IInvoke
    {
        [SerializeField] private PointType _type;
        [SerializeField] private int _startLevel;
        [SerializeField] private bool _overrideConfig;
        [SerializeField] private bool _dependency;

        [ShowIf("_dependency")] [SerializeField]
        private List<BasePointView> _dependencyList;

        [ShowIf("_overrideConfig")] [SerializeField]
        private BasePointConfig _localConfig;

        private int _id;

        [Inject] private AppEventProvider _appEventProvider;
        [Inject] protected GameProgressFactory Progresses;
        [Inject] private GameSystem _gameSystem;

        protected BasePointConfig _config;
        protected bool _isBought;
        protected int _level;
        protected int _region;
        protected List<PointPrice> Prices = new();
        
        public PointType Type => _type;
        public bool IsBought => _isBought;
        public int ID => _id;
        public int Level => _level;
        public int Region => _region;

        public event Action<BasePointView> OnBought;

        public void SetId(int id)
        {
            _id = id;
        }

        public void SetRegion(int region)
        {
            _region = region;
        }

        public int GetRemainingMoney()
        {
            var price = Prices.FirstOrDefault(item => item.Level == _level);
            if (price == null)
            {
                return 0;
            }
            else
            {
                return (int)price.Current;
            }
        }
        
        public virtual void SetConfig(BasePointConfig config)
        {
            _level = _startLevel;
            _config = _overrideConfig ? _localConfig : config;
            FillUpgradePrices(_config);
            Subscribe();
            Check();
        }
        
        private void Subscribe()
        {
            if(!_dependency) return;
            foreach (var dependency in _dependencyList)
            {
                if(dependency.IsBought) continue;
                dependency.OnBought += PointOnBought;
            }
        }

        private void PointOnBought(BasePointView basePointView)
        {
            Check();
        }

        private void Check()
        {
            if (_dependencyList.All(item => item.IsBought))
            {
                this.Activate();
            }
            else
            {
                this.Deactivate();
            }
        }

        private void Unsubscribe()
        {
            foreach (var dependency in _dependencyList)
            {
                dependency.OnBought -= PointOnBought;
            }
        }

        public virtual void LoadSave(PointData pointData)
        {
            _region = pointData.Region;
            _level = pointData.Level;
            _isBought = pointData.IsBought;
            
            var currentPrice = Prices.FirstOrDefault(item => item.Level == _level);
            if (currentPrice != null)
            {
                currentPrice.Current = pointData.RemainingMoney;
            }
            
            if (_isBought)
            {
                CompleteBuy(false);
            }
            else
            {
                Block();
            }

            UpdateInfo();
        }

        protected virtual void Block()
        {
            
        }

        protected virtual void UpdateInfo()
        {
            
        }

        private void FillUpgradePrices(BasePointConfig config)
        {
            foreach (var priceConfig in config.Prices)
            {
                if(priceConfig.Level < _startLevel) continue;
                foreach (var levelPrice in priceConfig.Prices)
                {
                    Prices.Add(new PointPrice
                    {
                        Level = priceConfig.Level,
                        Type = levelPrice.ParamType,
                        Target = levelPrice.BaseValue
                    });
                }
            }
        }

        public virtual void Play()
        {
        }

        public virtual void Tick(float deltaTime)
        {
            
        }

        public virtual bool Buy(GameParamType type, float value)
        {
            var returnValue = false;
            var currency = Prices.FirstOrDefault(p => p.Level == _level && p.Type == type);
            if (currency != null)
            {
                currency.Current += value;
                currency.IsCompleted = currency.Current >= currency.Target;
            }
            else
            {
                return returnValue;
            }

            if (PriceReached(_level))
            {
                _level++;
                CompleteBuy();
                returnValue = true;
            }

            return returnValue;
        }

        protected virtual void CompleteBuy(bool byPlayer = true)
        {
            if (byPlayer)
            {
                _appEventProvider.TriggerEvent(AppEventType.Analytics, GameEvents.BuyPoint, _type, _id, _gameSystem.Level);
            }
            _isBought = true;
            OnBought?.Invoke(this);
        }

        protected string GetPriceText(int level)
        {
            var text = level == 0 ? "Buy" : "Upgrade";
            var all = Prices.Where(p => p.Level == level);
            var pointPrices = all as PointPrice[] ?? all.ToArray();
            
            if (level > pointPrices.Length)
            {
                return "MAX";
            }
            
            foreach (var price in pointPrices.Where(p => !p.IsCompleted))
            {
                text += $"\n{price.Target - price.Current}<sprite name={price.Type}>";
            }

            return text;
        }
        
        protected string GetPriceText()
        {
            var text = "Buy";
            var all = Prices;
            // var pointPrices = all as PointPrice[] ?? all.ToArray();

            foreach (var price in all.Where(p => !p.IsCompleted))
            {
                text += $"\n{price.Target - price.Current}<sprite name={price.Type}>";
            }

            return text;
        }

        protected bool IsCap(int level)
        {
            var all = Prices.Where(p => p.Level == level);
            var pointPrices = all as PointPrice[] ?? all.ToArray();

            return level > pointPrices.Length;
        }

        protected bool PriceReached(int level)
        {
            return Prices.Where(price => price.Level == level).All(price => price.IsCompleted);
        }

        protected bool PriceReached()
        {
            return Prices.All(price => price.IsCompleted);
        }

        public override void OnDestroy()
        {
            Unsubscribe();
            
            base.OnDestroy();
        }

        protected class PointPrice
        {
            public int Level;
            public GameParamType Type;
            public float Target;
            public float Current;
            public bool IsCompleted;
        }
    }
}