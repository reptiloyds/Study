using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Tools;
using _Game.Scripts.View.Line;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.View.Points
{
    public class GridItem : BaseView
    {
        [SerializeField] private List<NeighborConfig> _neighbors;
        [SerializeField] private CollisionListener _collisionListener;
        [SerializeField] private bool _active;

        private const float _neighbirRadius = 0.9f;
        private bool _isBusy;

        public event Action ChangeState;
        public bool Active => _active;
        public bool IsBusy => _isBusy;
        public bool StartPoint => _collisionListener.StartPoint;

        public CollisionListener CollisionListener => _collisionListener;

        public override void Init()
        {
            _collisionListener.SetGridItem(this);
            
            base.Init();
        }

        public void SetGridType()
        {
            CollisionListener.SetCollisionListenerType(CollisionListenerType.Grid);
        }

        public void Enable()
        {
            this.Activate();
            _active = true;
        }

        public void Disable()
        {
            this.Deactivate();
            _active = false;
        }

        public Alignment CalculateNeighborAlignment(GridItem gridItem)
        {
            var neighborConfig = _neighbors.FirstOrDefault(item => item.Neighbor == gridItem);
            return neighborConfig?.Alignment ?? Alignment.None;
        }

        public void CalculateNeighbor(List<GridItem> gridItems)
        {
            var closestItems = new List<GridItem>();
            foreach (var gridItem in gridItems)
            {
                if(gridItem == this) continue;
                if(gridItem.CollisionListener.Block) continue;
                var distance = Vector3.Distance(transform.position, gridItem.transform.position);
                if (distance <= _neighbirRadius)
                {
                    closestItems.Add(gridItem);
                }
            }

            foreach (var closestItem in closestItems)
            {
                if (Mathf.Abs(closestItem.transform.position.x - transform.position.x) < 0.1f)
                {
                    if (closestItem.transform.position.z > transform.position.z)
                    {
                        var forward = _neighbors.FirstOrDefault(item => item.Alignment == Alignment.Forward);
                        forward.Neighbor = closestItem;
                    }
                    else
                    {
                        var backward = _neighbors.FirstOrDefault(item => item.Alignment == Alignment.Backward);
                        backward.Neighbor = closestItem;
                    }
                }
                else if (Mathf.Abs(closestItem.transform.position.z - transform.position.z) < 0.1f)
                {
                    if (closestItem.transform.position.x > transform.position.x)
                    {
                        var right = _neighbors.FirstOrDefault(item => item.Alignment == Alignment.Right);
                        right.Neighbor = closestItem;
                    }
                    else
                    {
                        var left = _neighbors.FirstOrDefault(item => item.Alignment == Alignment.Left);
                        left.Neighbor = closestItem;
                    }
                }
            }
        }

        public void ClearNeighbor()
        {
            foreach (var neighbor in _neighbors)
            {
                neighbor.Neighbor = null;
            }
        }

        public void MakeFree()
        {
            _isBusy = false;
        }

        public void MakeBusy()
        {
            _isBusy = true;
        }

        public bool HasFreeNeighbor()
        {
            return _neighbors.Any(item => item.Neighbor is {IsBusy: false});
        }

        public NeighborConfig GetNeighborByAlignment(Alignment alignment)
        {
            var neighbor = _neighbors.FirstOrDefault(item => item.Alignment == alignment);
            return neighbor;
        }

        public NeighborConfig GetFirstFreeNeighbor()
        {
            var neighbor = _neighbors.FirstOrDefault(item => item.Neighbor is {IsBusy: false});
            return neighbor;
        }

        public NeighborConfig GetRandomFreeNeighbor()
        {
            var neighbor = _neighbors.Where(item => item.Neighbor is {IsBusy: false}).RandomValue();
            return neighbor;
        }

        public void Clear()
        {
        }
    }

    [Serializable]
    public class NeighborConfig
    {
        public Alignment Alignment;
        public GridItem Neighbor;
    }
}