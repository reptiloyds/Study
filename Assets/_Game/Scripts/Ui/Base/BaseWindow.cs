using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Core;
using _Game.Scripts.Tools;
using _Game.Scripts.Ui.Base.Animations;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.Ui.Base
{
    public class BaseWindow : BaseUIView
    {
        private enum WindowState
        {
            Opened,
            Closed,
            PlayingAnim
        }
        
        private enum GuiAnimType
        {
            Open,
            Close
        }
        
        public Action<BaseWindow> Opened;
        public Action<BaseWindow> Closed;
     
        [SerializeField] private bool _addToOpenedStack = true;
        [SerializeField] private bool _toOpenedOverStack;
        [SerializeField] private bool _ignoreAnimations;

        [Inject] protected SceneData SceneData;
        
        private const string INNER_WINDOW = "Window";
        
        private WindowState _state;
        private RectTransform _windowRect;
        
        private CloseWindowButton _closeButton;
        private WindowBack _windowBack;
        private List<BaseUIAnimation> _windowAnimations = new();
        
        public bool AddToOpenedStack => _addToOpenedStack;
        public bool ToOpenOverStack => _toOpenedOverStack;
        public bool IsOpened => _state == WindowState.Opened;
        public bool IsClosed => _state == WindowState.Closed;

        public virtual void Init()
        {
            _state = WindowState.Closed;
            
            _closeButton = GetComponentInChildren<CloseWindowButton>(true);
            _windowBack = GetComponentInChildren<WindowBack>(true);

            if (!_ignoreAnimations)
            {
                _windowAnimations = GetComponentsInChildren<BaseUIAnimation>().ToList();
                if (_windowAnimations.Count == 0)
                {
                    var windowsAnimation = SceneData.GetComponentInChildren<ScaleGuiAnim>();
                    var defaultAnimation = gameObject.AddComponent<ScaleGuiAnim>().CopyFrom(windowsAnimation);
                    _windowAnimations.Add(defaultAnimation);
                }

                foreach (var windowAnimation in _windowAnimations)
                {
                    windowAnimation.Init();
                }
            }
            
            foreach (Transform child in transform)
            {
                if (child.name != INNER_WINDOW) continue;
                _windowRect = child as RectTransform;
                break;
            }
            
            if (_closeButton != null) _closeButton.SetCallback(Close);
            if (_windowBack != null && _closeButton != null) _windowBack.Init(_closeButton);
            
            this.Deactivate();
        }
        
        public virtual void Open(params object[] list)
        {
            if (_windowAnimations.Count == 0)
            {
                this.Activate();
                OnOpened();
            }
            else
            {
                this.Activate();
                SetState(WindowState.PlayingAnim);
                foreach (var windowAnim in _windowAnimations)
                {
                    windowAnim.PlayOpenAnimation(_windowRect, OnOpened);   
                }
            }
        }

        private void OnOpened()
        {
            SetState(WindowState.Opened);
            Opened?.Invoke(this);
        }

        public virtual void Close()
        {
            if (_windowAnimations.Count == 0)
            {
                OnClosed();
            }
            else
            {
                SetState(WindowState.PlayingAnim);
                foreach (var windowAnim in _windowAnimations)
                {
                    windowAnim.PlayCloseAnimation(_windowRect, OnClosed);   
                }
            }
        }

        private void OnClosed()
        {
            this.Deactivate();
            SetState(WindowState.Closed);
            Closed?.Invoke(this);
        }

        private void SetState(WindowState state)
        {
            _state = state;
        }

        public virtual void UpdateLocalization()
        {
        }

        public virtual void Tick(float deltaTime)
        {
        }
    }
}