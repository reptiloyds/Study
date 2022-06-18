using _Game.Scripts.Enums;
using _Game.Scripts.Systems.Base;
using _Game.Scripts.Ui.Base;
using TMPro;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.Ui
{
    public class VictoryWindow : BaseWindow
    {
        [SerializeField] private TextMeshProUGUI _rewardText;

        [SerializeField] private BaseButton _playButton;

        [Inject] private LevelSystem _levels;
        [Inject] private GameSystem _game;

        private int _soft;
        
        public override void Init()
        {
            _playButton.SetCallback(OnPressedPlay);
            base.Init();
        }

        private void OnPressedPlay()
        {
            _game.AddCurrency(GameParamType.Soft, _soft);
            _game.IncLevel();
            Close();
            _levels.LoadNextLevel();
        }

        public override void Open(params object[] list)
        {
            _soft = 0;
            if (list.Length > 0) _soft = (int) list[0];

            _rewardText.text = $"+{_soft}";
            
            base.Open(list);
        }
    }
}