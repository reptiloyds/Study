using System;
using _Game.Scripts.Enums;
using _Game.Scripts.Factories;
using _Game.Scripts.Systems.Base;
using Zenject;

namespace _Game.Scripts.Systems.Ads
{
    public class AdSystem
    {
        public enum AdResult
        {
            Watched,
            Clicked,
            Canceled,
            ErrorLoaded,
            ErrorDisplay
        }

        public Action<bool> OnCompleted;
        
        [Inject] private GameFlags _flags;
        [Inject] private AppEventProvider _appEventProvider;
        [Inject] private GameSystem _game;
        [Inject] private GameParamFactory _params;

        private readonly AdWrapper _currentWrapper;
        private AdPlacement _currentPlacement;
        
        public AdSystem()
        {
            _currentWrapper = new MaxWrapper();
            _currentWrapper.Init();
            _currentWrapper.OnRewardEnded += OnRewardEnded;
            _currentWrapper.OnInterEnded += OnInterEnded;
        }
        
        public void ShowDebugger()
        {
            _currentWrapper.ShowDebugger();
        }

        public void ShowBanner()
        {
            _currentWrapper.ShowBanner();
            _appEventProvider.TriggerEvent(AppEventType.Analytics, GameEvents.BannerShowed);
        }

        public void HideBanner()
        {
            _currentWrapper.HideBanner();
            _appEventProvider.TriggerEvent(AppEventType.Analytics, GameEvents.BannerHidden);
        }
        
        public void ShowInter(AdPlacement placement)
        {
            _currentPlacement = placement;
            _appEventProvider.TriggerEvent(AppEventType.Analytics, GameEvents.InterRequest, _currentPlacement);
            
#if UNITY_EDITOR
            return;
#endif
            
            if (_flags.Has(GameFlag.AllAdsRemoved) || !_flags.Has(GameFlag.TutorialFinished))
            {
                return;
            }

            if (!_currentWrapper.RewardAvailable())
            {
                return;
            }
            
            _currentWrapper.ShowInter();
        }
        
        public void ShowReward(AdPlacement placement)
        {
            _currentPlacement = placement;
            _appEventProvider?.TriggerEvent(AppEventType.Analytics, GameEvents.RewardRequest, _currentPlacement);
            
#if UNITY_EDITOR
            return;
#endif
            
            if (_flags.Has(GameFlag.AllAdsRemoved) || !_flags.Has(GameFlag.TutorialFinished))
            {
                return;
            }

            if (!_currentWrapper.RewardAvailable())
            {
                return;
            }
            
            _currentWrapper.ShowReward();
        }
        
        private void OnRewardEnded(AdResult result, int code)
        {
            switch (result)
            {
                case AdResult.Canceled:
                    _appEventProvider?.TriggerEvent(AppEventType.Analytics, GameEvents.RewardCanceled, _currentPlacement);
                    OnCompleted?.Invoke(false);
                    break;
                
                case AdResult.ErrorLoaded:
                    _appEventProvider?.TriggerEvent(AppEventType.Analytics, GameEvents.RewardErrorLoaded, _currentPlacement);
                    OnCompleted?.Invoke(false);
                    break;
                
                case AdResult.ErrorDisplay:
                    _appEventProvider?.TriggerEvent(AppEventType.Analytics, GameEvents.RewardErrorDisplay, _currentPlacement);
                    OnCompleted?.Invoke(false);
                    break;
                
                case AdResult.Clicked:
                    _appEventProvider?.TriggerEvent(AppEventType.Analytics, GameEvents.RewardClicked, _currentPlacement);
                    ClaimReward();
                    OnCompleted?.Invoke(true);
                    break;
                
                case AdResult.Watched:
                    _appEventProvider?.TriggerEvent(AppEventType.Analytics, GameEvents.RewardShowed, _currentPlacement);
                    ClaimReward();
                    OnCompleted?.Invoke(true);
                    break;
            }
        }

        private void ClaimReward()
        {
            switch (_currentPlacement)
            {
            }
        }

        private void OnInterEnded(AdResult result, int code)
        {
            switch (result)
            {
                case AdResult.Clicked:
                case AdResult.Watched:
                case AdResult.Canceled:
                    _appEventProvider?.TriggerEvent(AppEventType.Analytics, GameEvents.InterShowed, _currentPlacement);
                    break;
                
                case AdResult.ErrorLoaded:
                    _appEventProvider?.TriggerEvent(AppEventType.Analytics, GameEvents.InterErrorLoaded, _currentPlacement);
                    break;
                
                case AdResult.ErrorDisplay:
                    _appEventProvider?.TriggerEvent(AppEventType.Analytics, GameEvents.InterErrorDisplay, _currentPlacement);
                    break;
            }
        }
    }
}