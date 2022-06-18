using _Game.Scripts.Systems.Base;
using _Game.Scripts.Ui.Base;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.Ui
{
    public class EndLevelWindow : BaseWindow
    {
        [SerializeField] private BaseButton _continueButton;
        [SerializeField] private BaseButton _restartButton;

        [Inject] private GameSystem _gameSystem;
        [Inject] private LevelSystem _levelSystem;
        
        public override void Init()
        {
            _continueButton.SetCallback(Continue);
            _restartButton.SetCallback(Restart);

            base.Init();
        }

        private void Continue()
        {
            _gameSystem.IncLevel();
            _levelSystem.LoadNextLevel();
        }

        private void Restart()
        {
            _levelSystem.LoadNextLevel();
        }
    }
}