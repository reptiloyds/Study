using System;
using System.Collections.Generic;
using _Game.Scripts.Enums;
using Zenject;

namespace _Game.Scripts.Systems.Analytics
{
    /// <summary>
    /// Класс для интеграции с SDK аналитик
    /// </summary>
    public class AnalyticsSystem
    {
        public Action OnConnected;
        
        private List<IAnalyticsWrapper> _analyticsWrappers;
        
        public AnalyticsSystem()
        {
            _analyticsWrappers = new List<IAnalyticsWrapper>
            {
                new GameAnalyticsWrapper(),
                new AdjustWrapper(),
            };
        }

        public void Connect()
        {
            foreach (var wrapper in _analyticsWrappers)
            {
                wrapper.Init(OnConnected);
            }
        }

        public void SendEvent(GameEvents gameEvent, params object[] list)
        {
            foreach (var wrapper in _analyticsWrappers)
            {
                wrapper.SendEvent(gameEvent, list);
            }
        }
    }
}