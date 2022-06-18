using _Game.Scripts.Components.Base;
using _Game.Scripts.Systems.Base;
using Zenject;

namespace _Game.Scripts.Components.Loader
{
    public class LevelLoaderComponent : BaseComponent
    {
        [Inject] private LevelSystem _levels;

        public override void Start()
        {
            _levels.OnLoadedLevel += OnLoadedLevel;
            _levels.LoadNextLevel();
            _levels.GameStart();
            base.Start();
        }

        private void OnLoadedLevel()
        {
            _levels.OnLoadedLevel -= OnLoadedLevel;
            End();
        }
    }
}