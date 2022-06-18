using _Game.Scripts.Components.Base;
using _Game.Scripts.Enums;
using _Game.Scripts.Systems;
using _Game.Scripts.Systems.Base;
using _Game.Scripts.Systems.Save;
using Zenject;

namespace _Game.Scripts.Components.Loader
{
    /// <summary>
    /// Класс для загрузки сейвов в момент загрузки игры
    /// </summary>
    public class SaveLoaderComponent : BaseComponent
    {
        [Inject] private SaveSystem _save;
        [Inject] private AppEventProvider _appEventProvider;

        public override void Start()
        {
            _save.Load();
            _appEventProvider.TriggerEvent(AppEventType.Tutorial, GameEvents.SaveLoaded);
            End();
            base.Start();
        }
    }
}