using _Game.Scripts.Interfaces;

namespace _Game.Scripts.View.Animations
{
    public class AnimationEventsSender : BaseView, IAnimationEventsSender
    {
        private IAnimationEventsListener _listener;
        
        public void AssignListener(IAnimationEventsListener listener)
        {
            _listener = listener;
        }

        public void AddEvent()
        {
        }
    }
}