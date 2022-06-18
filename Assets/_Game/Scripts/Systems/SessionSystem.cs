using _Game.Scripts.Interfaces;
using _Game.Scripts.Systems.Base;
using _Game.Scripts.Ui;
using _Game.Scripts.View.Points;
using Zenject;

namespace _Game.Scripts.Systems
{
    public class SessionSystem : ITickableSystem
    {
        [Inject] private WindowsSystem _windowsSystem;
        private LevelSystem _levelSystem;
        private GameWindow _gameWindow;

        private GridView _gridView;

        public SessionSystem(LevelSystem levelSystem)
        {
            _levelSystem = levelSystem;
            
            _levelSystem.OnStartSession += OnStartSession;
            _levelSystem.OnDestroyLevel += OnDestroyLevel;
        }

        private void OnStartSession()
        {
        }
        

        private void OnDestroyLevel()
        {
        }

        private void NextLevel()
        {
            _windowsSystem.OpenWindow<EndLevelWindow>();
        }

        public void Tick(float deltaTime)
        {
        }

        public void CompleteTarget()
        {
        }
    }
}