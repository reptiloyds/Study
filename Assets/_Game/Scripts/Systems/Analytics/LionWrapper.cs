using System;
using _Game.Scripts.Enums;
using LionStudios.Suite.Analytics;

namespace _Game.Scripts.Systems.Analytics
{
    public class LionWrapper : IAnalyticsWrapper
    {
        public IAnalyticsWrapper Init(Action OnInitialized)
        {
            return this;
        }

        public void SendEvent(GameEvents eventType, params object[] parameters)
        {
            switch (eventType)
            {
            }
        }
    }
}