using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using _Game.Scripts.Balance;
using _Game.Scripts.Enums;
using _Game.Scripts.Factories;
using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.Systems;
using _Game.Scripts.Systems.Base;
using _Game.Scripts.Tools;
using _Game.Scripts.Ui.Base;
using _Game.Scripts.UI.HudElements;
using _Game.Scripts.Ui.Yacht;
using TMPro;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.Ui
{
    public class GameWindow : BaseWindow
    {
        [SerializeField] private TextMeshProUGUI _softText;
        [SerializeField] private TextMeshProUGUI _hardText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private RectTransform _taskContainer;

        [SerializeField] private Transform _resourcesContainer;
        [SerializeField] private ResourceItem[] _resourceItems;

        [Inject] private GameParamFactory _params;
        [Inject] private LevelSystem _levelSystem;
        [Inject] private AppEventProvider _appEventProvider;
        [Inject] private GameBalanceConfigs _balance;
        [Inject] private UIFactory _uiFactory;
        [Inject] private GameSystem _gameSystem;

        private GameParam _soft;
        private GameParam _hard;

        public event Action MergeButtonPressed;
        public event Action BackButtonPressed;

        public override void Init()
        {
            _levelSystem.OnStartSession += OnStartSession;
            _levelSystem.OnDestroyLevel += OnDestroyLevel;

            base.Init();
        }

        private void OnStartSession()
        {
        }

        private void OnDestroyLevel()
        {
            var targetUIElements = new List<TargetUIElement>(_uiFactory.GetTargetUIElements());
            foreach (var targetUIElement in targetUIElements)
            {
                _uiFactory.RemoveTargetUI(targetUIElement);
            }
            Close();
        }

        public override void Open(params object[] list)
        {
            _soft = _params.GetParam<GameSystem>(GameParamType.Soft);
            _soft.UpdatedEvent += OnUpdateSoft;
                
            _hard = _params.GetParam<GameSystem>(GameParamType.Hard);
            _hard.UpdatedEvent += OnUpdateHard;

            UpdateCurrency();

            base.Open();
        }

        private void UpdateCurrency()
        {
            OnUpdateSoft();
            OnUpdateHard();
        }
        
        private void OnUpdateSoft()
        {
            _softText.text = ((int)_soft.Value).ToString();
        }

        private void OnUpdateHard()
        {
            _hardText.text = _hard.Value.ToString(CultureInfo.CurrentCulture);
        }
        
    }
}