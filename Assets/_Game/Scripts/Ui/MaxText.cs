using System.Linq;
using _Game.Scripts.Factories;
using _Game.Scripts.Interfaces;
using _Game.Scripts.Systems.Base;
using _Game.Scripts.Tools;
using _Game.Scripts.View;
using _Game.Scripts.View.CollectableItems;
using _Game.Scripts.View.Units;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.Ui
{
    public class MaxText : BaseView, ITickableSystem
    {
        //[Inject] private CollectableItemsFactory _itemsFactory;
        // [Inject] private PlayerView _player;
        private LevelSystem _levelSystem;
        private CollectableItem _lastItem;
        private bool _move;

        [Inject]
        private void Construct(LevelSystem levelSystem)
        {
            _levelSystem = levelSystem;
            _levelSystem.OnGameStart += OnGameStart;
            Hide();
        }

        private void OnGameStart()
        {
            _levelSystem.OnGameStart -= OnGameStart;
            // _itemsFactory.OnTransactionFromTo += OnTransitionFromTo;
        }
        
        private void OnTransitionFromTo(CollectableItem item, IStorage from, IStorage to)
        {
            // if (from == _player as IStorage || to == _player as IStorage)
            // {
            //     CheckItemsCount();
            // }
        }
        
        private void CheckItemsCount()
        {
            // var items = _itemsFactory.GetParentItems(_player);
            // if(items.Count == 0) return;
            // _lastItem = items.OrderByDescending(x => x.transform.position.y).First();
            // if (_player.IsFull(items.Count) && _lastItem != null)
            // {
            //     Show();
            // }
            // else
            // {
            //     Hide();
            // }
        }

        private void Show()
        {
            Move();
            this.Activate();
            _move = true;
        }

        private void Hide()
        {
            this.Deactivate();
            _lastItem = null;
            _move = false;
        }

        private void Move()
        {
            if (_move)
            {
                transform.position = _lastItem.transform.position + Vector3.up * 0.3f;
            }
        }

        public override void OnDestroy()
        {
            // _itemsFactory.OnTransactionFromTo -= OnTransitionFromTo;
            
            base.OnDestroy();
        }

        public void Tick(float deltaTime)
        {
            Move();
        }
    }
}