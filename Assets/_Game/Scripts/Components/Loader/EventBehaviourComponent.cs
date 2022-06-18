using _Game.Scripts.Components.Base;
using _Game.Scripts.Systems;
using Zenject;

namespace _Game.Scripts.Components.Loader
{
    public class EventBehaviourComponent : BaseComponent
    {
        //[Inject] private EventBehaviourSystem _eventBehaviourSystem;

        public override void Start()
        {
            //_eventBehaviourSystem.Init();
            base.Start();
        }
    }
}