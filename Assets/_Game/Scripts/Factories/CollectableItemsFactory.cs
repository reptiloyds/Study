using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Balance;
using _Game.Scripts.Enums;
using _Game.Scripts.Interfaces;
using _Game.Scripts.Systems;
using _Game.Scripts.Systems.Base;
using _Game.Scripts.Tools;
using _Game.Scripts.View.CollectableItems;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.Factories
{
    public class CollectableItemsFactory : IGameProgress, IInvoke
    {
        public Action OnUpdateUIItem;
        public Action<CollectableItem> OnDespawnedItem;

        [Inject] private LevelSystem _levels;
        [Inject] private CollectableItem.Pool _itemPool;
        [Inject] private GameParamFactory _params;
        [Inject] private GameBalanceConfigs _balance;
        [Inject] private GameSystem _gameSystem;

        private readonly List<CollectableItem> _items = new();
        private readonly List<List<CollectableItem>> _relatedItems = new();

        private List<CollectableItem> _tempItems = new();

        private const float MOVE_ITEMS_SPEED = 0.15f;
        private const float INCREASE_ITEMS_SPEED = 0.95f;
        private int _currentCollectIteration = 0;
        public event Action<CollectableItem, IStorage, IStorage> OnTransactionFromTo;

        public void Init()
        {
            _levels.OnDestroyLevel += OnDestroyLevel;
        }

        private void OnDestroyLevel()
        {
            var list = _items.ToList();
            foreach (var item in list)
            {
                RemoveItem(item);
            }
        }

        public CollectableItem SpawnItem(IStorage parent, GameParamType type)
        {
            var item = _itemPool.Spawn(parent, ItemBehaviorType.Scale, type);
            item.SetItemParent(parent);
            item.SetLocalPosition(Vector3.zero);
            _items.Add(item);
            return item;
        }
        
        public CollectableItem SpawnItem(IStorage parent, GameParamType type, Vector3 pos)
        {
            var item = _itemPool.Spawn(parent, ItemBehaviorType.Scale, type);
            item.SetItemParent(parent);
            item.SetPosition(pos);
            _items.Add(item);
            return item;
        }

        private List<CollectableItem> Sort(List<CollectableItem> items)
        {
            return items.OrderBy(item => item.transform.position.y).ToList();
        }

        public void MoveItemsFromTo(IStorage from, IStorage to, GameParamType type, int count)
        {
            var items = type == GameParamType.None 
                ? _items.FindAll(i => i.Parent == from) 
                : _items.FindAll(i => i.Parent == from && i.Type == type);

            var delay = MOVE_ITEMS_SPEED;

            items = Sort(items);

            var iteration = count > items.Count ? items.Count : count;
            if(items.Count == 0) return;
            for (var i = 0; i < iteration; i++)
            {
                var currentItem = items.LastValue();
                MoveItem(ItemBehaviorType.MoveJump, currentItem, to, true, delay);
                items.Remove(currentItem);
                delay += MOVE_ITEMS_SPEED;
            }
        }

        private void MoveItem(ItemBehaviorType type, CollectableItem item, IStorage newParent, bool despawn = false, float delay = 0)
        {
            if (item.Move)
            {
                return;
            }
            var pos = GetPosition(item, newParent);
            item.SetTransition(item.Parent, newParent);
            item.SetItemParent(newParent);
            item.PlayBehavior(type, pos, delay);
            item.SetDespawn(despawn);
            item.OnMoved += OnItemMoved;
        }

        private void OnItemMoved(CollectableItem item, bool despawn)
        {
            var transition = item.GetTransition();
            item.OnMoved -= OnItemMoved;
            if (despawn)
            {
                RemoveItem(item);
                OnDespawnedItem?.Invoke(item);   
            }
            
            OnTransactionFromTo?.Invoke(item, transition.from, transition.to);
        }

        public void RemoveItem(CollectableItem item)
        {
            _items.Remove(item);
            _itemPool.Despawn(item);
        }
        
        public Vector3 GetPosition(CollectableItem item, IStorage parent, 
            ItemStorageType storageType = ItemStorageType.None, bool shiftPosition = false)
        {
            var type = storageType == ItemStorageType.None ? parent.StorageType : storageType;
            switch (type)
            {
                case ItemStorageType.None:
                    return Vector3.zero;
                
                case ItemStorageType.Tower:
                    return GetPositionFromTower(item, parent);
                
                case ItemStorageType.Center:
                    return GetPositionFromCenter(item, parent);
                
                case ItemStorageType.Wall:
                    return GetPositionFromWall(item, parent, shiftPosition);
            }

            return Vector3.zero;
        }

        public List<CollectableItem> GetParentItems(IStorage parent)
        {
            return _items.Where(item => item.Parent == parent).ToList();
        }

        public List<CollectableItem> GetItemFromParent(IStorage parent, GameParamType type)
        {
            return type != GameParamType.None 
                ? _items.FindAll(i => i.Parent == parent as IStorage && i.Type == type) 
                : _items.FindAll(i => i.Parent == parent as IStorage);
        }

        private Vector3 GetPositionFromTower(CollectableItem item, IStorage parent)
        {
            var items = _items.FindAll(i => i.Parent == parent);
            if (items.Count == 0)
            {
                return Vector3.zero;
            }
            else
            {
                var lastItem = items.LastValue();
                foreach (var element in items)
                {
                    if (element.FuturePosition.y > lastItem.FuturePosition.y)
                    {
                        lastItem = element;
                    }
                }
                
                return lastItem.FuturePosition + Vector3.up * ((lastItem.YScale + item.YScale)/2);
            }
        }
        
        private Vector3 GetPositionFromTower(CollectableItem item, int id)
        {
            if (id == 0)
            {
                return Vector3.zero;
            }
            return Vector3.up * ((item.YScale + item.YScale) / 2) * id;
        }

        private Vector3 GetPositionFromCenter(CollectableItem item, IStorage parent)
        {
            var items = _items.FindAll(i => i.Parent == parent as IStorage && i.Type == item.Type);
            var step = item.ItemConfig.PositionStep;
            var position = parent.ItemsContainer.position;

            var count = parent.Columns * parent.Rows;
            var column = 0;
            var row = 1;
            
            var centerColumns = Math.Ceiling((float)parent.Columns / 2);
            var centerRows = Math.Ceiling((float)parent.Rows / 2);
            
            for (int i = 0; i < count; i++)
            {
                column++;
                if (column > parent.Columns)
                {
                    column = 1;
                    row++;
                }
                
                if (i < items.Count) continue;
        
                float columnZ = 0;
                if(column < centerColumns)
                {
                    columnZ = step * column;
                }
                else if(column > centerColumns)
                {
                    columnZ = (float) (step * -(column-centerColumns));
                }
                
                float rowX = 0;
                if(row < centerRows)
                {
                    rowX = step * row;
                }
                else if(row > centerRows)
                {
                    rowX = step * -row;
                }

                return new Vector3(rowX, position.y, columnZ);
            }
            
            return Vector3.zero;
        }
        
        private Vector3 GetPositionFromWall(CollectableItem item, IStorage parent, bool shiftPosition = false)
        {
            var items = _items.FindAll(i => i.Parent == parent as IStorage && i.Type == item.Type);
            var itemsCount = shiftPosition ? items.Count - 1 : items.Count; 
            var step = item.ItemConfig.PositionStep;
            var count = parent.Columns * parent.Rows;
            var column = 0;
            var row = 0;
            
            for (int i = 0; i < count; i++)
            {
                row++;
                if (row > parent.Rows)
                {
                    row = 1;
                    column++;
                }
                
                if (i < itemsCount) continue;

                float rowX = 0;
                float columnZ = 0;

                if (i == 0)
                {
                    rowX = 0;
                    columnZ = 0;
                }
                else
                {
                    rowX = -step * column;
                    columnZ = -step * (row - 1);
                }

                return new Vector3(rowX, 0, columnZ);
            }
            
            return Vector3.zero;
        }
    }
}