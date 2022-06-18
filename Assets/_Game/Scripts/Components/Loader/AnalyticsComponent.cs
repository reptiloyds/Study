using _Game.Scripts.Components.Base;
using _Game.Scripts.Systems.Analytics;
using Zenject;

namespace _Game.Scripts.Components.Loader
{
    /// <summary>
    /// Класс для подключения аналитики
    /// </summary>
    public class AnalyticsComponent : BaseComponent
    {
        [Inject] private AnalyticsSystem _analytics;
        
        public override void Start()
        {
            _analytics.OnConnected += OnConnected;
            _analytics.Connect();
            base.Start();
        }

        private void OnConnected()
        {
            End();
        }
    }
}