using _Game.Scripts.Enums;
using _Game.Scripts.Systems.Base;
using _Game.Scripts.Ui.Base;
using _Game.Scripts.Ui.SkillsWindow;
using UnityEngine;

namespace _Game.Scripts.Systems.GamePlayElements
{
    public class MainWindowButton : BaseGamePlayElement
    {
        [SerializeField] private BaseButton _button;

        public BaseButton Button => _button;

        public override void Init(WindowsSystem windows)
        {
            if (_button != null) _button.SetCallback(OnPressedButton);

            base.Init(windows);
        }

        private void OnPressedButton()
        {
            switch (Type)
            {
                case GamePlayElement.SkillsButton:
                    Windows.OpenWindow<SkillsWindow>();
                    break;
            }
        }
    }
}