#if UNITY_ANDROID
    using Unity.Notifications.Android;
#endif

#if UNITY_IPHONE || UNITY_IOS
    using Unity.Notifications.iOS;
#endif

namespace _Game.Scripts.Systems.PushNotifications
{
    public interface IPushNotification
    {
        void Init();
        void SendNotification(int id, string title, string text, int timer);
        void UpdateScheduledNotification(int id, string title, string text, int timer);
        
#if UNITY_ANDROID
        AndroidNotification CreateNotification(int id, string title, string text, int timer)
        {
            return new AndroidNotification();
        }
#endif
        
#if UNITY_IPHONE || UNITY_IOS
        iOSNotification CreateNotification(int id, string title, string text, int timer)
        {
            return new iOSNotification();
        }
#endif
    }
}