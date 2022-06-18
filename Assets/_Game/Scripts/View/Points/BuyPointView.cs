using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Enums;
using _Game.Scripts.Interfaces;
using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.Systems.Base;
using _Game.Scripts.Systems.Save.SaveStructures;
using _Game.Scripts.Tools;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.View.Points
{
    public class BuyPointView : BasePointView
    {
        [SerializeField] private GameObject _buyBody;
        [SerializeField] private TextMeshPro _text;
        [SerializeField] private Animation _showAnimation;
        [SerializeField] private Animation _notEnoughMoney;
        [SerializeField] private ParticleSystem _buyVfx;
        [SerializeField] private bool _changeCameraTransform;
        [ShowIf("_changeCameraTransform")] [SerializeField]
        private Transform _cameraPoint;
        
        private bool _unlocked;
        private List<IBuyElement> _buyElements = new();

        [Inject] private GameSystem _gameSystem;
        [Inject] private GameCamera _gameCamera;
        private LevelSystem _levelSystem;

        [Inject]
        private void Construct(LevelSystem levelSystem)
        {
            _levelSystem = levelSystem;
            _levelSystem.OnStartSession += OnStartSession;
        }

        private void OnStartSession()
        {
            if (_isBought)
            {
                RestoreAll();
            }
            else
            {
                BlockAll();
            }
        }

        public override void LoadSave(PointData pointData)
        {
            base.LoadSave(pointData);
        }

        public override void OnDestroy()
        {
            _levelSystem.OnStartSession -= OnStartSession;
            
            base.OnDestroy();
        }

        public override void SetConfig(BasePointConfig config)
        {
            base.SetConfig(config);

            _buyElements = GetComponentsInChildren<IBuyElement>().ToList();
            
            UpdateInfo();
        }

        protected override void UpdateInfo()
        {
            _text.text = GetPriceText();
            
            base.UpdateInfo();
        }

        public void Buy()
        {
            var currentPrice = Prices.FirstOrDefault(item => item.Level == _level);
            if(currentPrice == null) return;
            if (_gameSystem.IsEnoughCurrency(GameParamType.Soft, currentPrice.Target))
            {
                 base.Buy(GameParamType.Soft, currentPrice.Target);
                 _gameSystem.SpendCurrency(GameParamType.Soft, currentPrice.Target);
            }
            else
            {
                _notEnoughMoney.Play();
            }
        }

        protected override void CompleteBuy(bool byPlayer = true)
        {
            base.CompleteBuy(byPlayer);
            _buyBody.Deactivate();
            if (byPlayer)
            {
                BuyAll();
                if (_changeCameraTransform)
                {
                    _gameCamera.ChangeSaveData(_cameraPoint.position, _cameraPoint.rotation.eulerAngles);
                }
            }
            else
            {
                // RestoreAll();
            }
        }

        // protected override void Block()
        // {
        //     BlockAll();
        //     
        //     base.Block();
        // }

        private void RestoreAll()
        {
            foreach (var buyElement in _buyElements)
            {
                buyElement.Restore();
            }
        }

        private void BuyAll()
        {
            foreach (var buyElement in _buyElements)
            {
                buyElement.Buy();
            }

            _showAnimation.Play();
            _buyVfx.Play();
        }

        private void BlockAll()
        {
            foreach (var buyElement in _buyElements)
            {
                buyElement.Block();
            }
        }
    }
}