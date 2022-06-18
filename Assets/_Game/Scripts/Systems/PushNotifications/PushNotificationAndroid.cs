using UnityEngine;
using Application = UnityEngine.Application;
#if UNITY_ANDROID
using System;
using Unity.Notifications.Android;
using UnityEngine.Device;
#endif

namespace _Game.Scripts.Systems.PushNotifications
{
    public class PushNotificationAndroid : IPushNotification
    {
        private string _channelId;
        
        public void Init()
        {
            _channelId = Application.identifier;
            
#if UNITY_ANDROID
            var channel = new AndroidNotificationChannel
            {
                Id = _channelId,
                Name = Application.productName,
                Importance = Importance.High,
                Description = "Generic notifications",
            };
            
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
#endif
        }

        public void SendNotification(int id, string title, string text, int timer)
        {
#if UNITY_ANDROID
            var notification = CreateNotification(id, title, text, timer);
            AndroidNotificationCenter.SendNotificationWithExplicitID(notification, _channelId, id);
#endif
        }

        public void UpdateScheduledNotification(int id, string title, string text, int timer)
        {
#if UNITY_ANDROID
            var notification = CreateNotification(id, title, text, timer);
            AndroidNotificationCenter.UpdateScheduledNotification(id, notification, _channelId);
#endif
        }
        
#if UNITY_ANDROID
        public AndroidNotification CreateNotification(int id, string title, string text, int timer)
        {
            return new AndroidNotification
            {
                Title = title,
                Text = text,
                FireTime = DateTime.Now.AddSeconds(timer),
                SmallIcon = "small_icon",
                LargeIcon = "large_icon"
            };
        }
#endif
    }
}