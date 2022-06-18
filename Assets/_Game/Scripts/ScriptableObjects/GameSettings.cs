using System;
using _Game.Scripts.Interfaces;
using _Game.Scripts.Systems.Save;
using Sirenix.Serialization;
using Zenject;

namespace _Game.Scripts.ScriptableObjects
{
    /// <summary>
    /// Используется для хранения внутриигровых параметров
    /// </summary>
    public class GameSettings
    {
        public event Action SoundChangedEvent, NotificationsEvent;
		
        private bool _muteMusic;
        private bool _muteSound;
        private bool _disablePushNotifications = true;

        [Inject] private SaveSystem _save;
		
        public bool IsMuteMusic => _muteMusic;
        public bool IsMuteSound => _muteSound;
        public bool IsDisablePushNotifications => _disablePushNotifications;

        public bool MuteMusic
        {
            get => _muteMusic;
            set => SetMuteMusic(value);
        }
		
        public bool MuteSound
        {
            get => _muteSound;
            set => SetMuteSound(value);
        }
		
        public bool DisablePushNotifications
        {
            get => _disablePushNotifications;
            set => SetDisablePushNotification(value);
        }

        private void SetDisablePushNotification(bool value)
        {
            _disablePushNotifications = value;
            _save.Save();
            NotificationsEvent?.Invoke();
        }

        private void SetMuteMusic(bool mute)
        {
            _muteMusic = mute;
            _save.Save();
            SoundChangedEvent?.Invoke();
        }
		
        private void SetMuteSound(bool mute)
        {
            _muteSound = mute;
            _save.Save();
            SoundChangedEvent?.Invoke();
        }

        public void Save()
        {
            
        }

        public void CopyFrom(object source)
        {
            var source1 = (GameSettings)source;
            _muteMusic = source1._muteMusic;
            _muteSound = source1._muteSound;
            _disablePushNotifications = source1._disablePushNotifications;
        }
    }
}