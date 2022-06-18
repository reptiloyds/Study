using System;
using _Game.Scripts.Interfaces;
using _Game.Scripts.View.Animal;
using _Game.Scripts.View.Points;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.Systems
{
    public class InputSystem : ITickableSystem
    {
        [Inject] private GameCamera _gameCamera;
     
        private ParallelCheck _parallelCheck;
     
        private bool _drag;
        private bool _blockInput;
        private Vector3 _offset;
        private float _zCoord;

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
     
            if (_drag)
            {
            }
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
            if(Physics.Raycast(_gameCamera.UnityCam.ScreenPointToRay(Input.mousePosition), out var raycastHit, 100, GameLayers.ANIMAL_MASK))
            {
                _drag = true;
                var animal = raycastHit.transform.GetComponent<BaseAnimal>();
                if (animal != null)
                {
                    animal.TryMove();
                }
            }
                 
            PointDown?.Invoke();
        }

        private void PointerUp()
        {
            _drag = false;

            PointUp?.Invoke();
        }
    }
}