using System.Collections.Generic;
using _Game.Scripts.Systems.InputSystems;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.View.DrawLine
{
    public class Line : BaseView
    {
        [SerializeField] private LineRenderer _renderer;
        [SerializeField] private EdgeCollider2D _collider;

        private readonly List<Vector2> _points = new();

        public class Pool : MonoMemoryPool<Line>
        {
            protected override void Reinitialize(Line item)
            {
                item.Reset();
            }
        }

        protected override void Reset()
        {
            _renderer.positionCount = 0;
            base.Reset();
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
            _collider.transform.position -= transform.position;
        }

        public void AddPosition(Vector2 position)
        {
            if(!CanAppend(position)) return;

            _points.Add(position);
            
            _renderer.positionCount++;
            _renderer.SetPosition(_renderer.positionCount - 1, position);

            _collider.points = _points.ToArray();
        }

        private bool CanAppend(Vector2 position)
        {
            if (_renderer.positionCount == 0) return true;

            return Vector2.Distance(_renderer.GetPosition(_renderer.positionCount - 1), position) >
                   DrawLineInputSystem.RESOLUTION;
        }
    }
}