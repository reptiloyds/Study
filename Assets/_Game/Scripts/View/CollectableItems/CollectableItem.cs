using System;
using _Game.Scripts.Enums;
using _Game.Scripts.Interfaces;
using _Game.Scripts.Systems.Base;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.View.CollectableItems
{
    public class CollectableItem : BaseView
    {
        public Action<CollectableItem> OnCollected;
        public Action<CollectableItem, bool> OnMoved;
        
        [SerializeField] private VisualView _viewConfig;
        [SerializeField] private ItemConfig _itemConfig;
        [SerializeField] private BoxCollider _boxCollider;

        private ItemBehaviorType _currentBehavior;
        private Sequence _sequence;
        private bool _despawnOnMoveEnd = false;
        private IStorage _from;
        private IStorage _to;
        private bool _move;

        public IStorage Parent { get; private set; }
        public GameParamType Type { get; private set; }
        public Vector3 FuturePosition { get; private set; }
        public ItemConfig ItemConfig => _itemConfig;
        public float YScale { get; private set; }

        public bool Move => _move;

        public class Pool : MonoMemoryPool<IStorage, ItemBehaviorType, GameParamType, CollectableItem>
        {
            protected override void Reinitialize(IStorage parent, ItemBehaviorType behavior, GameParamType type, CollectableItem item)
            {
                item.Reset(parent, behavior, type);
            }
        }

        private void Reset(IStorage parent, ItemBehaviorType behavior, GameParamType type)
        {
            _despawnOnMoveEnd = false;
            transform.localScale = Vector3.one;
            _viewConfig.Init();
            _viewConfig.Show(type);
            YScale = _viewConfig.CurrentConfig.YScale;

            Parent = parent;
            _currentBehavior = behavior;
            Type = type;

            EnableCollider(false);
            PlayBehavior(behavior);
            
            base.Reset();
        }

        public void SetDespawn(bool value)
        {
            _despawnOnMoveEnd = value;
        }

        public void SetTransition(IStorage from, IStorage to)
        {
            _from = from;
            _to = to;
        }

        public (IStorage from, IStorage to) GetTransition()
        {
            return (_from, _to);
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetLocalPosition(Vector3 position)
        {
            transform.localPosition = position;
        }

        public void SetFuturePosition(Vector3 position)
        {
            FuturePosition = position;
        }

        public void PlayBehavior(ItemBehaviorType behaviorType, Vector3 pos = default, float delay = 0)
        {
            if (_currentBehavior != ItemBehaviorType.None)
            {
                _sequence?.Kill();
            }

            switch (behaviorType)
            {
                case ItemBehaviorType.Jump:
                    Jump(Parent.Transform.position);
                    break;
                
                case ItemBehaviorType.Move:
                    FuturePosition = pos;
                    MoveTo(pos, delay);
                    break;
                
                case ItemBehaviorType.MoveJump:
                    FuturePosition = pos;
                    MoveJumpTo(pos, delay);
                    break;
                case ItemBehaviorType.Scale:
                    transform.localRotation = Quaternion.identity;
                    break;
            }
        }
        
        private void Jump(Vector3 pos)
        {
            _move = true;
            SetPosition(pos);
            
            var rndX = UnityEngine.Random.Range(-_itemConfig.DropRange, _itemConfig.DropRange);
            var rndZ = UnityEngine.Random.Range(-_itemConfig.DropRange, _itemConfig.DropRange);
            var target = new Vector3(pos.x + rndX, 0.05f, pos.z + rndZ);
            
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            _sequence.Append(transform.DOJump(target, _itemConfig.JumpForce, 1, _itemConfig.JumpTime));
            _sequence.Join(transform.DORotate(new Vector3(0, UnityEngine.Random.Range(0, 360), 0), _itemConfig.JumpTime))
                    .OnComplete(() =>
                    {
                        _boxCollider.enabled = true;
                        _move = false;
                        OnMoved?.Invoke(this, _despawnOnMoveEnd);
                        _currentBehavior = ItemBehaviorType.None;
                    });
        }

        private void MoveJumpTo(Vector3 pos, float delay)
        {
            _move = true;
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            _sequence.Append(transform.DOLocalJump(pos, 0.5f, 1, 0.2f)
                .SetEase(Ease.Linear)
                .SetDelay(delay)
                .OnComplete(() =>
                {
                    transform.localRotation = Quaternion.identity;
                    _move = false;
                    OnMoved?.Invoke(this, _despawnOnMoveEnd);
                    _currentBehavior = ItemBehaviorType.None;
                }));
        }
        
        public void MoveTo(Vector3 pos, float delay)
        {
            _move = true;
            _sequence?.Kill();
            _sequence = null;
            _sequence = DOTween.Sequence();
            _sequence.Append(transform.DOMove(pos, 0.5f)
                .SetEase(Ease.Linear)
                .SetDelay(delay)
                .OnComplete(() =>
                {
                    transform.localRotation = Quaternion.identity;
                    _move = false;
                    OnMoved?.Invoke(this, _despawnOnMoveEnd);
                    _currentBehavior = ItemBehaviorType.None;
                }));
        }
        
        public void LocalMoveTo(Vector3 pos, float delay)
        {
            _sequence?.Kill();
            _sequence = null;
            _sequence = DOTween.Sequence();
            _sequence.Append(transform.DOLocalMove(pos, 0.5f)
                .SetEase(Ease.Linear)
                .SetDelay(delay)
                .OnComplete(() =>
                {
                    transform.localRotation = Quaternion.identity;
                    OnMoved?.Invoke(this, _despawnOnMoveEnd);
                    _currentBehavior = ItemBehaviorType.None;
                }));
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != GameLayers.PLAYER_LAYER) return;
            EnableCollider(false);
            OnCollected?.Invoke(this);
        }

        public void EnableCollider(bool flag)
        {
            _boxCollider.enabled = flag;
        }

        public void SetItemParent(IStorage parent)
        {
            Parent = parent;
            transform.SetParent(parent.ItemsContainer);
        }
    }

    public enum ItemBehaviorType
    {
        None,
        Jump,
        Move,
        MoveJump,
        Scale
    }
    
    [Serializable]
    public enum ItemStorageType
    {
        None,
        Tower,
        Center,
        Wall
    }

    [Serializable]
    public class ItemConfig
    {
        [Header("Moving")]
        public float DropRange = 1f;
        public float JumpForce = 1f;
        public float JumpTime = 1f;
        public float PositionStep = 2f;
    }
}