using System;
using _Game.Scripts.Enums;
using com.adjust.sdk;
using UnityEngine;

namespace _Game.Scripts.Systems.Analytics
{
    public class AdjustWrapper : IAnalyticsWrapper
    {
        public IAnalyticsWrapper Init(Action OnInitialized)
        {
            InitAdjust("8b9co0fc8zk0");
            return this;
        }
        
        private void InitAdjust(string adjustAppToken)
        {
            var adjustConfig = new AdjustConfig(
                adjustAppToken,
                AdjustEnvironment.Production,
                true
            );
            adjustConfig.setLogLevel(AdjustLogLevel.Info);
            adjustConfig.setSendInBackground(true);
            new GameObject("Adjust").AddComponent<Adjust>();

            Adjust.start(adjustConfig);
        }

        public void SendEvent(GameEvents eventType, params object[] parameters)
        {
       
        }
    }
}