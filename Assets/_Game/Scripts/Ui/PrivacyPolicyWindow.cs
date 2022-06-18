using _Game.Scripts;
using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.Systems.Base;
using _Game.Scripts.Tools;
using _Game.Scripts.Ui.Base;
using TMPro;
using UnityEngine;
using Zenject;

public class PrivacyPolicyWindow : BaseWindow
{
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _text;
    
    [SerializeField] private BaseButton _continueButton;
    [SerializeField] private BaseButton _termsButton;
    [SerializeField] private BaseButton _privacyButton;

    [Inject] private ProjectSettings _projectSettings;
    [Inject] private WindowsSystem _windows;
    
    public override void Init()
    {
        _continueButton.Callback += () => _windows.CloseWindow(this);
        _termsButton.Callback += () => Application.OpenURL(_projectSettings.TermsLink);
        _privacyButton.Callback += () => Application.OpenURL(_projectSettings.PrivacyLink);
        base.Init();
    }

    public override void UpdateLocalization()
    {
        _title.text = "PRIVACY_TITLE".ToLocalized();
        _text.text = "PRIVACY_TEXT".ToLocalized();
        _termsButton.SetText("TERMS_BUTTON_TEXT".ToLocalized());
        _privacyButton.SetText("PRIVACY_BUTTON_TEXT".ToLocalized());
        base.UpdateLocalization();
    }
}