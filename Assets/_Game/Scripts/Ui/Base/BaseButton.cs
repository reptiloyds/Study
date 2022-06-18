using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Game.Scripts.Ui.Base
{
	[ExecuteInEditMode]
	public class BaseButton : BaseUIView
	{
		public static event Action<int> ClickSoundEvent = delegate { };
		public static event Action AnyButtonClickedEvent = delegate { };

		[SerializeField] protected Button _button;
		[SerializeField] protected int _clickSound = -1;
		
		[Header("ViewSettings")]
		[SerializeField] protected Image _image;
		[SerializeField] protected Image _back;
		[SerializeField] protected Image _front1;
		[SerializeField] protected Image _front2;

		[Header("ColorSettings")]
		[SerializeField] private Color _interactableColorBack;
		[SerializeField] private Color _interactableColorFront1;
		[SerializeField] private Color _interactableColorFront2;
		[SerializeField] private Color _unInteractableColorBack;
		[SerializeField] private Color _unInteractableColorFront1;
		[SerializeField] private Color _unInteractableColorFront2;
		
		[SerializeField] protected TextMeshProUGUI[] _texts;

		private const float CLICK_TIME = 0.4f;
		private float _downTime;
		
		private BaseButton _currentOverlapped;
		private BaseButton _allowedTutorialButton;
		
		public Action Callback;
		private bool Interactable => _button.interactable;

#if UNITY_EDITOR
		protected void Awake()
		{
			if (Application.isPlaying) return;

			if (_image == null) _image = GetComponent<Image>();
			if (_button == null) _button = GetComponentInChildren<Button>();
			_texts = GetComponentsInChildren<TextMeshProUGUI>();
		}
#endif

		protected void OnEnable()
		{
			if (_button == null) _button = GetComponent<Button>();
			if (_button == null) return;
			_button.onClick.RemoveAllListeners();
			_button.onClick.AddListener(OnClick);
		}
		
		public void SetText(string text)
		{
			if (_texts.Length < 1) return;
			_texts[0].text = text;
		}
		
		public void SetText(int element, string text)
		{
			if (_texts.Length < element) return;
			_texts[element].text = text;
		}

		public void SetTextColor(int element, Color color)
		{
			if (_texts.Length < element) return;
			_texts[element].color = color;
		}
		
		public BaseButton SetSprite(Sprite sprite)
		{
			_image.sprite = sprite;
			return this;
		}

		public void SetColors(Color image, Color back = default, Color front1 = default, Color front2 = default)
		{
			_image.color = image;
			if (back != default) _back.color = back;
			if (front1 != default) _front1.color = front1;
			if (front2 != default) _front2.color = front2;
		}

		public BaseButton SetCallback(Action callback)
		{
			Callback = callback;
			return this;
		}

		public void SetInteractable(bool interactable)
		{
			_button.interactable = interactable;
			if (interactable)
			{
				_back.color = _interactableColorBack;
				if (_front1 != null) _front1.color = _interactableColorFront1;
				if (_front2 != null) _front2.color = _interactableColorFront2;
			}
			else
			{
				_back.color = _unInteractableColorBack;
				if (_front1 != null) _front1.color = _unInteractableColorFront1;
				if (_front2 != null) _front2.color = _unInteractableColorFront2;
			}
		}

		public void SetInteractableColor()
		{
			_back.color = _unInteractableColorBack;
			if (_front1 != null) _front1.color = _unInteractableColorFront1;
			if (_front2 != null) _front2.color = _unInteractableColorFront2;
		}

		private void OnClick()
		{
			if (!Interactable) return;

			if (_allowedTutorialButton != null)
			{
				if (_allowedTutorialButton != this) return;
				_allowedTutorialButton = null;
			}

			Callback?.Invoke();
			AnyButtonClickedEvent?.Invoke();

			if (_clickSound == -1) _clickSound = 1;
			ClickSoundEvent?.Invoke(_clickSound);
		}

		public void SimulateClick()
		{
			OnClick();
		}
	}
} 