using _Game.Scripts.Components.Base;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif

namespace _Game.Scripts.Components.Loader
{
	/// <summary>
	/// Класс для проверки статуса подтверждения отлеживания рекламного идентификатора
	/// </summary>
    public class ATTrackingStatusComponent : BaseComponent
    {
        public override void Start()
        {
#if UNITY_ANDROID || UNITY_EDITOR
            End();
#elif UNITY_IOS || UNITY_IPHONE
			var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
	        if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
	        {
		        ATTrackingStatusBinding.RequestAuthorizationTracking(End);
	        }
	        else
	        {
		        End();
	        }
#endif
	        base.Start();
        }
    }
}     