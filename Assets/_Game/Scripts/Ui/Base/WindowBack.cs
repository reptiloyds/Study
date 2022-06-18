using UnityEngine;

namespace _Game.Scripts.Ui.Base
{
	public class WindowBack : BaseUIView
	{
		private BaseButton _button;
		private CloseWindowButton _closeWindowButton;
		
		public void Init(CloseWindowButton closeButton)
		{
			_button = GetComponent<BaseButton>();
			_button.SetCallback(OnPressedClose);
			
			_closeWindowButton = closeButton;
		}

		private void OnPressedClose()
		{
			_closeWindowButton.SimulateClick();
		}
	}
}