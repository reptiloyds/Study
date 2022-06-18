using _Game.Scripts.Tools;
using _Game.Scripts.Ui.Base;
using TMPro;
using UnityEngine;

namespace _Game.Scripts.Ui
{
    public class TutorialWindow : BaseWindow
    {
        [SerializeField] private TextMeshProUGUI _tipText;

        public void Open(string text)
        {
            _tipText.text = text;
            if (!isActiveAndEnabled) this.Activate();
        }
        
        public override void Close()
        {
            this.Deactivate();
        }
    }
}