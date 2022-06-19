using _Game.Scripts.Factories;
using _Game.Scripts.Interfaces;
using _Game.Scripts.View.DrawLine;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.Systems.InputSystems
{
    public class DrawLineInputSystem : ITickableSystem
    {
        [Inject] private GameCamera _camera;
        [Inject] private BallFactory _ballFactory;
        [Inject] private LineFactory _lineFactory;

        public const float RESOLUTION = 0.1f;

        private Line _currentLine;
        
        public void Tick(float deltaTime)
        {
            Vector2 mousePosition = _camera.UnityCam.ScreenToWorldPoint(Input.mousePosition);
            if (Input.GetMouseButtonDown(1))
            {
                SpawnBall(mousePosition);
            }

            if (Input.GetMouseButtonDown(0))
            {
                SpawnLine(mousePosition);
            }

            if (Input.GetMouseButton(0))
            {
                DrawLine(mousePosition);
            }
        }

        private void SpawnBall(Vector2 position)
        {
            var ball = _ballFactory.SpawnBall();
            ball.SetPosition(position);
        }
        
        private void SpawnLine(Vector2 position)
        {
            _currentLine = _lineFactory.SpawnLine();
            _currentLine.SetPosition(position);
        }

        private void DrawLine(Vector2 position)
        {
            _currentLine.AddPosition(position);
        }
    }
}