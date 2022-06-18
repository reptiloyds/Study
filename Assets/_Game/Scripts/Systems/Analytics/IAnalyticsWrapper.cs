using System;
using _Game.Scripts.Enums;

namespace _Game.Scripts.Systems.Analytics
{
    public interface IAnalyticsWrapper
    {
        public IAnalyticsWrapper Init(Action OnInitialized);
        public void SendEvent(GameEvents eventType, params object[] parameters);
    }
}