using System;
using System.Globalization;
using _Game.Scripts.Enums;
using _Game.Scripts.Factories;
using _Game.Scripts.Systems;
using _Game.Scripts.Systems.Base;
using _Game.Scripts.Tools;
using _Game.Scripts.Ui.Base;
using TMPro;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.Ui
{
    public class MainWindow : BaseWindow
    {
        [SerializeField] private TextMeshProUGUI _softText;
        [SerializeField] private TextMeshProUGUI _hardText;
        [SerializeField] private TextMeshProUGUI _levelText;
        
        [SerializeField] private BaseButton _playButton;

        [Inject] private GameParamFactory _params;
        [Inject] private LevelSystem _levels;

        private GameParam _soft;
        private GameParam _hard;
        private GameParam _level;

        public override void Init()
        {
            _playButton.SetCallback(OnPressedPlay);

            base.Init();
        }

        private void OnPressedPlay()
        {
            Close();
            _levels.PlayLevel();
        }

        private void UpdateLevelText()
        {
            _levelText.text = $"LEVEL {_level.Value}";
        }

        public override void Open(params object[] list)
        {
            _soft = _params.GetParam<GameSystem>(GameParamType.Soft);
            _soft.UpdatedEvent += OnUpdateSoft;
                
            _hard = _params.GetParam<GameSystem>(GameParamType.Hard);
            _hard.UpdatedEvent += OnUpdateHard;

            _level = _params.GetParam<GameSystem>(GameParamType.Level);
            _level.UpdatedEvent += UpdateLevelText;
            
            UpdateCurrency();
            UpdateLevelText();
            
            base.Open(list);
        }

        private void UpdateCurrency()
        {
            OnUpdateSoft();
            OnUpdateHard();
        }

        private void OnUpdateSoft()
        {
            _softText.text = _soft.Value.ToFormattedString();
        }

        private void OnUpdateHard()
        {
            _hardText.text = _hard.Value.ToString(CultureInfo.CurrentCulture);
        }
       
        public override void Close()
        {
            _soft.UpdatedEvent -= OnUpdateSoft;
            _hard.UpdatedEvent -= OnUpdateHard;
            _level.UpdatedEvent -= UpdateLevelText;
            base.Close();
        }
    }
}