using System.Linq;
using _Game.Scripts.Balance;
using _Game.Scripts.Enums;
using _Game.Scripts.Factories;
using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.Systems;
using _Game.Scripts.Systems.Base;
using _Game.Scripts.Ui.Base;
using _Game.Scripts.View.Units;
using TMPro;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.Ui.SkillsWindow
{
    public class SkillsWindowItem : BaseUIView
    {
        [SerializeField] private GameParamType _type;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private TextMeshProUGUI _bonusText;
        [SerializeField] private BaseButton _button;

        [Inject] private GameParamFactory _params;
        [Inject] private GameSystem _game;
        [Inject] private UIFactory _uiFactory;
        [Inject] private GameBalanceConfigs _balance;

        private float _price;

        private GameParam _levelParam;
        private SkillConfig _config;
        
        public void Init()
        {
            _button.SetCallback(OnPressedButton);
        }

        private void OnPressedButton()
        {
            if (!_game.IsEnoughCurrency(GameParamType.Soft, _price))
            {
                _uiFactory.SpawnMessage("No enough gold!");
                return;
            }
            
            _game.SpendCurrency(GameParamType.Soft, _price);
            
            switch (_type)
            {
                case GameParamType.Capacity:
                    _levelParam = _params.GetParam<PlayerView>(GameParamType.CapacityLevel);
                    _levelParam.Change(1);
                    break;
            }

            Redraw();
        }

        public void Redraw()
        {
            _levelParam = _params.GetParam<GameSystem>(GameParamType.Level);
            switch (_type)
            {
                case GameParamType.Capacity:
                    _config = _balance.DefaultBalance.Skills.FirstOrDefault(b => b.Type == _type);
                    if (_config != null)
                    {
                        _price = _config.PriceStep * _levelParam.Value;
                        _priceText.text = $"<sprite name=Soft>{_price}";
                    }
                    _bonusText.text = "Capacity: +1";
                    break;
            }
            
            _levelText.text = $"Level {_levelParam.Value}";
        }
    }
}