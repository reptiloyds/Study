using _Game.Scripts.Components.Base;
using _Game.Scripts.Systems.Base;
using _Game.Scripts.Ui.Base;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.Components.Loader
{
    /// <summary>
    /// Класс для подтверждения получения согласия с политикой конфиденциальности
    /// </summary>
    public class PrivacyPolicyComponent : BaseComponent
    {
        [Inject] private WindowsSystem _windows;
        
        public override void Start()
        {
            if (PlayerPrefs.HasKey("privacy_policy"))
            {
                PlayerPrefs.SetInt("privacy_policy", 1);
            }
            else
            {
                var window = _windows.OpenWindow<PrivacyPolicyWindow>();
                window.Closed += OnClosedPrivacyWindow;
            }

            base.Start();
        }

        private void OnClosedPrivacyWindow(BaseWindow window)
        {
            window.Closed -= OnClosedPrivacyWindow;
            End();
        }
    }
}