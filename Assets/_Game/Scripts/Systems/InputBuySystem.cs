using System;
using _Game.Scripts.Interfaces;
using _Game.Scripts.View.Points;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.Systems
{
    public class InputBuySystem : ITickableSystem
    {
        [Inject] private GameCamera _gameCamera;
        private bool _blockInput;

        public event Action PointDown;
        public event Action PointUp;

        public void InputOff()
        {
            _blockInput = true;
        }

        public void InputOn()
        {
            _blockInput = false;
        }
        
        public void Tick(float deltaTime)
        {
            if(_blockInput) return;

            CheckInput();
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
            PointDown?.Invoke();
        }

        private void PointerUp()
        {
            if(Physics.Raycast(_gameCamera.UnityCam.ScreenPointToRay(Input.mousePosition), out var raycastHit, 100, GameLayers.BUY_MASK))
            {
                var objectView = raycastHit.transform.GetComponent<BuyPointView>();
                if (objectView != null)
                {
                    objectView.Buy();
                }
            }
            
            PointUp?.Invoke();
        }
    }
}