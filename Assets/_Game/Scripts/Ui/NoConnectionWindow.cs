using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.Systems;
using _Game.Scripts.Systems.Base;
using _Game.Scripts.Tools;
using _Game.Scripts.Ui.Base;
using TMPro;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.Ui
{
    public class NoConnectionWindow : BaseWindow
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private BaseButton _button;
        
        [Inject] private ConnectionSystem _connection;
        
        public override void Init()
        {
            _button.SetCallback(OnPressedCheck);
            base.Init();
        }

        public override void UpdateLocalization()
        {
            _title.text = "NOCONNECTION_TITLE".ToLocalized();
            _text.text = "NOCONNECTION_TEXT".ToLocalized();
            _button.SetText("NOCONNECTION_BUTTON_TEXT".ToLocalized());
            base.UpdateLocalization();
        }

        private void OnPressedCheck()
        {
            _connection.Connected += OnConnected;
            _connection.RunCheckConnection();
        }
        
        private void OnConnected(bool success)
        {
            if (!success) return;
            _connection.Connected -= OnConnected;
        }
    }
}