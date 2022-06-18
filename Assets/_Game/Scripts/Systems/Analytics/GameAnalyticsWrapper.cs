using System;
using _Game.Scripts.Enums;
using GameAnalyticsSDK;
using LionStudios.Suite.Analytics;
using UnityEngine;

namespace _Game.Scripts.Systems.Analytics
{
    public class GameAnalyticsWrapper : IAnalyticsWrapper
    {
        public IAnalyticsWrapper Init(Action OnInitialized = null)
        {
            GameAnalytics.Initialize();
            LionAnalytics.GameStart();
            OnInitialized?.Invoke();
            return this;
        }

        public void SendEvent(GameEvents eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case GameEvents.RewardRequest:
                    break;
                
                case GameEvents.RewardShowed:
                    break;
                
                case GameEvents.RewardClicked:
                    break;
                
                case GameEvents.RewardCanceled:
                    break;
                
                case GameEvents.RewardErrorLoaded:
                    break;
                
                case GameEvents.RewardErrorDisplay:
                    break;
                
                case GameEvents.InterRequest:
                    break;
                
                case GameEvents.InterShowed:
                    break;
                
                case GameEvents.InterErrorLoaded:
                    break;
                
                case GameEvents.InterErrorDisplay:
                    break;
                
                case GameEvents.BannerShowed:
                    break;
                
                case GameEvents.BannerHidden:
                    break;
                
                case GameEvents.LevelStart:
                    var message = $"level_start:{(int) parameters[0]}";
                    GameAnalytics.NewDesignEvent(message);
                    break;
                
                case GameEvents.LevelFinish:
                    break;
                
                case GameEvents.BuyPoint:
                    var newMessage = $"buy_point:{parameters[0]}:id{parameters[1]}:level{parameters[2]}";
                    GameAnalytics.NewDesignEvent(newMessage);
                    break;
            }
        }
    }
}