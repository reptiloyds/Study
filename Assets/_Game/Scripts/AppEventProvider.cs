using _Game.Scripts.Enums;
using _Game.Scripts.Systems.Analytics;
using _Game.Scripts.Systems.Tutorial;
using Zenject;

namespace _Game.Scripts
{
    /// <summary>
    /// Класс для передачи событий между классами 
    /// </summary>
    public class AppEventProvider
    {
        [Inject] private AnalyticsSystem _analytics;
        [Inject] private TutorialSystem _tutorial;
        
        public void TriggerEvent(AppEventType type, GameEvents gameEvent, params object[] list)
        {
            switch (type)
            {
                case AppEventType.Analytics:
                    _analytics.SendEvent(gameEvent, list);
                    break;
                case AppEventType.Tutorial:
                    if (gameEvent == GameEvents.SaveLoaded)
                    {
                        _tutorial.OnAction(gameEvent, list);
                        return;
                    }
                    if (_tutorial.IsPlaying) _tutorial.OnAction(gameEvent, list);
                    break;
            }
        }
        
        public void TriggerEvent(GameEvents gameEvent, params object[] list)
        {
            _analytics.SendEvent(gameEvent, list);
        }
    }
}