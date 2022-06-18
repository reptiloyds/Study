using System;

namespace _Game.Scripts.Systems.Ads
{
    public abstract class AdWrapper
    {
        public Action<AdSystem.AdResult, int> OnRewardEnded;
        public Action<AdSystem.AdResult, int> OnInterEnded;
        public abstract void Init();
        public abstract void ShowReward();
        public abstract void ShowInter();
        public abstract void ShowBanner();
        public abstract void HideBanner();
        public abstract bool RewardAvailable();
        public abstract bool InterAvailable();
        public abstract void ShowDebugger();
    }
}