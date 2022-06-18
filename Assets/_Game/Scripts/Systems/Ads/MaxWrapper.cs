using _Game.Scripts.ScriptableObjects;
using Zenject;

namespace _Game.Scripts.Systems.Ads
{
    public class MaxWrapper : AdWrapper
    {
        [Inject] private ProjectSettings _projectSettings;
        
        public override void Init()
        {
        }

        public override void ShowReward()
        {
        }

        public override void ShowInter()
        {
        }

        public override void ShowBanner()
        {
        }
        
        public override void HideBanner()
        {
        }

        public override bool RewardAvailable()
        {
            return true;
        }

        public override bool InterAvailable()
        {
            return true;
            
        }

        public override void ShowDebugger()
        {
        }
    }
}