using _Game.Scripts.Components.Base;
using _Game.Scripts.Factories;
using _Game.Scripts.Systems.Base;
using Zenject;

namespace _Game.Scripts.Components.Loader
{
    public class InitGameUI : BaseComponent
    {
        [Inject] private WindowsSystem _windows;
        [Inject] private UIFactory _uiFactory;

        public override void Start()
        {
            _windows.InitWindows();
            _windows.InitGamePlayElements();
            _windows.UpdateLocalization();
            
            _uiFactory.Init();
            
            End();
        }
    }
}