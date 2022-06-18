using _Game.Scripts.Factories;
using _Game.Scripts.Systems.Base;
using _Game.Scripts.Ui;
using Zenject;

namespace _Game.Scripts.Systems
{
    public class EndLevelSystem
    {
        [Inject] private WindowsSystem _windows;
        [Inject] private PointsFactory _points;
        [Inject] private LevelSystem _levelSystem;

        public void PlayVictory()
        {
            InvokeSystem.StartInvoke(ShowWindow, 1f);
        }
        
        public void PlayDefeat()
        {
            InvokeSystem.StartInvoke(RestartLevel, 1f);
        }

        private void ShowWindow()
        {
            // _windows.OpenWindow<VictoryWindow>(_points.GetPlayerBaseHp());
        }

        private void RestartLevel()
        {
            _levelSystem.RestartLevel();
        }
    }
}