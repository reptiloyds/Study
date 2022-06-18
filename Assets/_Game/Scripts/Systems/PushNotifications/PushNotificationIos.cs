using System;
#if UNITY_IPHONE || UNITY_IOS
using Unity.Notifications.iOS;
#endif
using UnityEngine;

namespace _Game.Scripts.Systems.PushNotifications
{
    public class PushNotificationIos : IPushNotification
    {
        public void Init()
        {
        }

        public void SendNotification(int id, string title, string text, int timer)
        {
#if UNITY_IPHONE || UNITY_IOS
            var notification = CreateIosNotification(id, title, text, timer);
            iOSNotificationCenter.RemoveScheduledNotification(notification.Identifier);
            iOSNotificationCenter.RemoveDeliveredNotification(notification.Identifier);
            iOSNotificationCenter.ScheduleNotification(notification);
#endif
        }

        public void UpdateScheduledNotification(int id, string title, string text, int timer)
        {
#if UNITY_IPHONE || UNITY_IOS
            var notification = CreateIosNotification(id, title, text, timer);
            iOSNotificationCenter.RemoveScheduledNotification(notification.Identifier);
            iOSNotificationCenter.RemoveDeliveredNotification(notification.Identifier);
            iOSNotificationCenter.ScheduleNotification(notification);
#endif
        }

#if UNITY_IPHONE || UNITY_IOS
        private iOSNotification CreateNotification(int id, string title, string text, int timer)
        {
            var time = DateTime.Now.AddSeconds(timer);
            
            var timeTrigger = new iOSNotificationTimeIntervalTrigger
            {
                TimeInterval = new TimeSpan(time.Hour, time.Minute, time.Second),
                Repeats = false
            };
            
            return new iOSNotification
            {
                Identifier = id.ToString(),
                Title = title,
                Subtitle = text,
                ShowInForeground = true,
                ForegroundPresentationOption = PresentationOption.Alert | PresentationOption.Sound,
                CategoryIdentifier = "notifications",
                ThreadIdentifier = Application.productName,
                Trigger = timeTrigger,
            };
        }
#endif
    }
}