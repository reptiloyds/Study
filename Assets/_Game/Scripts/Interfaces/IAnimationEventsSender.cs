namespace _Game.Scripts.Interfaces
{
	public interface IAnimationEventsSender
	{
		void AssignListener(IAnimationEventsListener listener);
		void AddEvent();
	}
}