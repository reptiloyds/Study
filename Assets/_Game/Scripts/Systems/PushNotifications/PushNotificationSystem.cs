using System.Collections.Generic;
using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.Tools;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
using Zenject;

namespace _Game.Scripts.Systems.PushNotifications
{
    public class PushNotificationSystem
    {
        [Inject] private GameSettings _settings;
        
        private const int _offlineId = 10000;
        private readonly IPushNotification _notifications;
        
        private readonly List<int> _identifiers = new List<int>
        {
            _offlineId
        };

        public PushNotificationSystem()
        {
#if UNITY_ANDROID
            _notifications = new PushNotificationAndroid();
#elif UNITY_IPHONE || UNITY_IOS
            _notifications = new PushNotificationIos();
#endif
            
            _notifications.Init();
        }
        
        public void UpdateNotifications(bool active)
        {
            if (_settings.DisablePushNotifications || active) return;

            foreach (var identifier in _identifiers)
            {
                var time = GetTime(identifier);
                if (time <= 60) return;
#if UNITY_ANDROID
                var notificationStatus = AndroidNotificationCenter.CheckScheduledNotificationStatus(identifier);
                switch (notificationStatus)
                {
                    case NotificationStatus.Scheduled:
                        
                        _notifications.SendNotification(identifier, 
                            $"{identifier}_PUSH_TITLE".ToLocalized(), 
                            $"{identifier}_PUSH_TEXT".ToLocalized(),
                            time);
                        break;
                    
                    case NotificationStatus.Delivered:
                        AndroidNotificationCenter.CancelNotification(identifier);
                        break;
                    
                    case NotificationStatus.Unknown:
                        _notifications.SendNotification(identifier, 
                            $"{identifier}_PUSH_TITLE".ToLocalized(), 
                            $"{identifier}_PUSH_TEXT".ToLocalized(),
                            time);
                        break;
                }
#elif UNITY_IPHONE || UNITY_IOS
                _notifications.SendNotification(identifier, 
                    $"{identifier}_PUSH_TITLE".ToLocalized(), 
                    $"{identifier}_PUSH_TEXT".ToLocalized(),
                    time);
#endif
            }
        }

        private int GetTime(int identifier)
        {
            switch (identifier)
            {
                case _offlineId:
                    return -1;//return data?.GetParam(GameParamType.MaxOfflineTime).IntValue ?? -1;

                default:
                    return -1;
            }
        }
    }
}