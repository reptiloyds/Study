using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Core;
using _Game.Scripts.Enums;
using _Game.Scripts.Interfaces;
using _Game.Scripts.Systems.GamePlayElements;
using _Game.Scripts.Tools;
using _Game.Scripts.Ui.Base;

namespace _Game.Scripts.Systems.Base
{
    public class WindowsSystem : ITickableSystem
    {
        public event Action<BaseWindow> WindowOpenedEvent, WindowClosedEvent;
        private readonly SceneData _sceneData;

        private readonly List<BaseWindow> _all = new();
        private readonly List<BaseWindow> _windowsStack = new();
        private readonly List<BaseGamePlayElement> _gamePlayElements = new();

        private BaseWindow _openedWindow;

        public WindowsSystem(SceneData sceneData)
        {
            _sceneData = sceneData;
        }
        
        public void InitWindows()
        {
            var windows = _sceneData.UI.GetComponentsInChildren<BaseWindow>(true);
            foreach (var window in windows)
            {
                AddWindow(window);
            }
        }

        public void InitGamePlayElements()
        {
            var elements = _sceneData.UI.GetComponentsInChildren<BaseGamePlayElement>(true);
            foreach (var element in elements)
            {
                element.Init(this);
                _gamePlayElements.Add(element);
            }
        }
        
        private void AddWindow(BaseWindow window)
        {
            _all.Add(window);
            window.Init();
        }
        
        private void AddWindow<T>() where T : BaseWindow
        {
            var window = _sceneData.GetComponentInChildren<T>(true);
            if (window == null) return;
            window.Init();
            _all.Add(window);
        }

        public BaseWindow OpenWindow<T>(params object[] list)
        {
            _openedWindow = _all.FirstOrDefault(w => w.GetType() == typeof(T));
            if (_openedWindow == null) return null;
            if (_openedWindow.IsOpened) return null;

            if (_openedWindow.AddToOpenedStack)
            {
                if (_windowsStack.Count != 0) return _openedWindow;
                _windowsStack.Add(_openedWindow);
                _openedWindow.Closed += OnCloseButtonPressed;
                _openedWindow.Open(list);
                WindowOpenedEvent?.Invoke(_openedWindow);
            }
            else
            {
                if(!_openedWindow.ToOpenOverStack) CloseAllWindows();
                _openedWindow.Closed += OnCloseButtonPressed;
                _openedWindow.Open(list);
                WindowOpenedEvent?.Invoke(_openedWindow);
            }

            return _openedWindow;
        }

        private void OnCloseButtonPressed(BaseWindow window)
        {
            window.Closed -= OnCloseButtonPressed;
            if (_windowsStack.Contains(window)) _windowsStack.Remove(window);
            if (_windowsStack.Count > 0)
            {
                _windowsStack[0].Open();
            }
            WindowClosedEvent?.Invoke(window);
        }

        private void CloseAllWindows()
        {
            var count = _windowsStack.Count;
            for (var i = 0; i < count; i++)
            {
                CloseWindow(_windowsStack[0]);
            }
            // foreach (var window in _windowsStack)
            // {
            //     CloseWindow(window);
            // }
        }

        public void CloseWindow<T>()
        {
            var window = _all.FirstOrDefault(w => w.GetType() == typeof(T));
            CloseWindow(window);
        }

        public void CloseWindow(BaseWindow window)
        {
            if (window == null) return;
            if (window.IsClosed) return;
            if (window == _openedWindow) _openedWindow = null;
            
            window.Close();
            
            WindowClosedEvent?.Invoke(window);
            
            if (_windowsStack.Contains(window)) _windowsStack.Remove(window);
            if (_windowsStack.Count > 0)
            {
                _windowsStack[0].Open();
            }
        }

        public BaseWindow GetWindow<T>()
        {
            return _all.FirstOrDefault(w => w.GetType() == typeof(T));
        }

        public void UpdateLocalization()
        {
            foreach (var window in _all)
            {
                window.UpdateLocalization();
            }
        }

        public void Tick(float deltaTime)
        {
            _openedWindow?.Tick(deltaTime);
        }

        public BaseGamePlayElement GetGamePlayElement(GamePlayElement element)
        {
            return _gamePlayElements.Find(e => e.Type == element);
        }

        public virtual void MoveToOverlay(params GamePlayElement[] elements)
        {
            foreach (var element in elements)
            {
                var gamePlayElement = _gamePlayElements.Find(e => e.Type == element);
                if (gamePlayElement == null) continue;
                
                if (gamePlayElement.transform.parent == _sceneData.WindowsOverlay) continue;
                gamePlayElement.transform.SetParent(_sceneData.WindowsOverlay, true);
                gamePlayElement.Activate();
            }
        }
        
        public virtual void ReturnFromOverlay(params GamePlayElement[] elements)
        {
            foreach (var element in elements)
            {
                var gamePlayElement = _gamePlayElements.Find(e => e.Type == element);
                if (gamePlayElement != null)
                {
                    gamePlayElement.RestorePosition();
                }
            }
        }

        public bool IsWindowOpened()
        {
            return _windowsStack.Count > 0;
        }
    }
}