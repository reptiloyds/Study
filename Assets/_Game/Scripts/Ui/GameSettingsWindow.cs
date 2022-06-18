using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.Tools;
using _Game.Scripts.Ui.Base;
using TMPro;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.Ui
{
	public class GameSettingsWindow : BaseWindow
	{
		[SerializeField] private TextMeshProUGUI _title;
		[SerializeField] private TextMeshProUGUI _musicText;
		[SerializeField] private TextMeshProUGUI _soundText;
		[SerializeField] private TextMeshProUGUI _notificationsText;
		[SerializeField] private TextMeshProUGUI _rateGameText;
		[SerializeField] private TextMeshProUGUI _purchasesText;
		
		[SerializeField] private BaseButton _musicButton;
		[SerializeField] private BaseButton _soundButton;
		[SerializeField] private BaseButton _termsButton;
		[SerializeField] private BaseButton _privacyButton;
		[SerializeField] private BaseButton _notificationsButton;
		[SerializeField] private BaseButton _adDebuggerButton;
		[SerializeField] private BaseButton _cheatsButton;
		[SerializeField] private BaseButton _rateUsButton;
		[SerializeField] private BaseButton _restorePurchasesButton;
		
		[Inject] private ProjectSettings _projectSettings;
		[Inject] private GameSettings _gameSettings;
		
		private int _tapsAdDebugger;
		
		public override void Init()
		{
			_musicButton.SetCallback(OnPressedMusicButton);
			_soundButton.SetCallback(OnPressedSoundButton);
			_notificationsButton.SetCallback(OnPressedNotifications);
			_adDebuggerButton.SetCallback(OnPressedAdDebugger);
			_rateUsButton.SetCallback(OnPressedRateUs);
			
			_termsButton.Callback += () => Application.OpenURL(_projectSettings.TermsLink);
			_privacyButton.Callback += () => Application.OpenURL(_projectSettings.PrivacyLink);
			
			base.Init();
		}

		public override void UpdateLocalization()
		{
			_title.SetText("TASK_LIST".ToLocalized());
			_musicText.SetText("MUSIC".ToLocalized());
			_soundText.SetText("SOUND".ToLocalized());
			_notificationsText.SetText("NOTIFICATIONS".ToLocalized());
			_rateGameText.SetText("RATE_GAMES".ToLocalized());
			_purchasesText.SetText("PURCHASES".ToLocalized());
			
			base.UpdateLocalization();
		}

		private void OnPressedMusicButton()
		{
			_gameSettings.MuteMusic = !_gameSettings.IsMuteMusic;
		}
		
		private void OnPressedSoundButton()
		{
			_gameSettings.MuteSound = !_gameSettings.IsMuteSound;
		}
		
		private void OnPressedNotifications()
		{
			_gameSettings.DisablePushNotifications = !_gameSettings.IsDisablePushNotifications;
		}
		
		private void OnPressedAdDebugger()
		{
			if (!_projectSettings.DevBuild) return;
			_tapsAdDebugger++;
			if (_tapsAdDebugger < 3) return;
			_tapsAdDebugger = 0;
		}
		
		private void OnPressedRateUs()
		{
#if UNITY_ANDROID
			Application.OpenURL(_projectSettings.GooglePlayLink);
#elif UNITY_IPHONE || UNITY_IOS

            Application.OpenURL(_settings.AppStoreLink);
#endif
			PlayerPrefs.SetString("IsAppRated", "");
		}
	}
}