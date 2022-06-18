using _Game.Scripts.View.Animations;

namespace _Game.Scripts.Interfaces
{
	public interface IAnimationEventsListener
	{
		void ExecuteEvent(AnimationEventType eventType);
	}
}