using System;
using _Game.Scripts.Core;
using _Game.Scripts.Interfaces;
using _Game.Scripts.Systems.Base;
using _Game.Scripts.Tools;
using _Game.Scripts.View.Units;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.Systems
{
    public class PlayerInputSystem : ITickableSystem
    {
        [Inject] private WindowsSystem _windows;
        [Inject] private PlayerView _player;
        
        private Vector2 _startInputJoystick;
        private Vector2 _inputCurrent;
        private Vector2 _inputDelta;
		
        private bool _pressed;
        private bool _blockInput;
        
        public event Action PointDown;
        public event Action PointUp;

        public bool IsPressed => _pressed;
        
        private FloatingJoystick _joystick;

        [Inject]
        private void Construct(SceneData sceneData)
        {
            _joystick = sceneData.Joystick;
        }
        
        public void InputOff()
        {
            _blockInput = true;
            _player.Stop();
            _joystick.Deactivate();
        }

        public void InputOn()
        {
            _joystick.Activate();
            _blockInput = false;
        }
        
        public void Tick(float deltaTime)
        {
            if(_blockInput) return;
            if (_windows.IsWindowOpened())
            {
                if (_player.State == UnitState.Moving)
                {
                    _player.Stop();
                    _pressed = false;
                }
                return;
            }
            
            CheckInput();
            if (_pressed)
            {
                Press(deltaTime);
            }
            else
            {
                _inputCurrent = Vector2.zero;
            }
            
            _player.MoveUnit(_inputCurrent);
        }
        
        private void CheckInput()
        {			
            if (Input.GetMouseButtonDown(0))
            {
                PointerDown();
            }
            if (Input.GetMouseButtonUp(0))
            {
                PointerUp();
            }
        }
        
        private void PointerDown()
        {
            _pressed = true;
            _startInputJoystick = Input.mousePosition;
            PointDown?.Invoke();
        }
		
        private void Press(float deltaTime)
        {
            // _inputCurrent = Input.mousePosition;
            // _inputCurrent = _startInputJoystick - _inputCurrent;
            // _inputCurrent *= deltaTime;
            // _inputCurrent.x = -_inputCurrent.x;
            // _inputCurrent.y = -_inputCurrent.y;
            _inputCurrent = _joystick.Direction;
        }
		
        private void PointerUp()
        {
            _pressed = false;
            PointUp?.Invoke();
        }
    }
}