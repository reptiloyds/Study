using _Game.Scripts.Components.Base;
using _Game.Scripts.Factories;
using _Game.Scripts.Systems.Base;
using _Game.Scripts.Systems.Save;
using _Game.Scripts.Systems.Tutorial;
using Zenject;

namespace _Game.Scripts.Components.Loader
{
    /// <summary>
    /// Класс для инициализации систем перед загрузкой игры
    /// </summary>
    public class InitGameSystems : BaseComponent
    {
        [Inject] private TutorialSystem _tutorial;
        [Inject] private WindowsSystem _windows;
        [Inject] private UIFactory _uiFactory;
        [Inject] private GameSystem _game;
        [Inject] private SaveSystem _save;

        public override void Start()
        {
            _game.Init();
            _save.Init();
            _tutorial.Init();
            _uiFactory.Init();

            base.Start();
            End();
        }
    }
}