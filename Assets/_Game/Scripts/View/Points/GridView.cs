using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Interfaces;
using _Game.Scripts.ScriptableObjects;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Game.Scripts.View.Points
{
    public class GridView : BasePointView, IObjectParent
    {
        [SerializeField] private int _itemsCount;
        [SerializeField] protected List<GridItem> _gridItems;
        // [SerializeField] protected List<PatternConfig> _patternConfigs = new();

        private GridItem _startGridItem;

        private bool _init;
        

        public override void Init()
        {
            if(_init) return;
            _init = true;

            foreach (var gridItem in _gridItems)
            {
                gridItem.Init();
            }

            _startGridItem = _gridItems.FirstOrDefault(item => item.StartPoint);
            if (_startGridItem == null)
            {
                Debug.LogError($"Can`t find Start GridItem");
            }

            base.Init();
        }

        public CollisionListener GetStartCollisionListener()
        {
            return _gridItems.FirstOrDefault(item => item.StartPoint)?.CollisionListener;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        [Button]
        public void SetGridType()
        {
            foreach (var gridItem in _gridItems)
            {
                gridItem.SetGridType();
            }
        }

        [Button]
        public void RecalculateCount()
        {
            _gridItems.Clear();
            _gridItems = GetComponentsInChildren<GridItem>(true).ToList();
            var activeCount = 0;
            foreach (var gridItem in _gridItems)
            {
                if (activeCount < _itemsCount)
                {
                    gridItem.Enable();
                    activeCount++;
                }
                else
                {
                    gridItem.Disable();   
                }
            }
        }

        [Button]
        public void ShuffleGridItems()
        {
            var count = _gridItems.Count - 1;
            for (var i = 0; i < count; i++)
            {
                var j = Random.Range(0, i);
                if (j != i)
                {
                    _gridItems[i] = _gridItems[j];
                    _gridItems[j] = _gridItems[i + 1];   
                }
            }
        }

        [Button]
        public void CalculateNeighbors()
        {
            var activeItems = _gridItems.Where(item => item.Active).ToList();
            foreach (var gridItem in activeItems)
            {
                gridItem.CalculateNeighbor(activeItems);
            }
        }
        
        [Button]
        public void ClearNeighbors()
        {
            foreach (var gridItem in _gridItems)
            {
                gridItem.ClearNeighbor();
            }
        }

        public void Clear()
        {
            var activeGrids = _gridItems.Where(item => item.Active).ToList();
            foreach (var gridItem in activeGrids)
            {
                gridItem.Clear();
            }
        }

        public bool CheckComplete()
        {
            return true;
        }
    }

    [Serializable]
    public class PatternConfig
    {
        public int Id;
        public List<CollisionListener> CollisionListeners;
    }
}