using _Game.Scripts.Enums;
using _Game.Scripts.Factories;
using _Game.Scripts.Interfaces;
using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.Tools;
using _Game.Scripts.View.CollectableItems;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.View.Points
{
    public class TrashPointView : BasePointView, IStorage
    {
        [SerializeField] private InteractiveView _inputPoint;
        [SerializeField] private Animation _animation;

        [Inject] private CollectableItemsFactory _items;

        public ItemStorageType StorageType { get; set; }
        public Transform Transform { get; set; }
        public Transform ItemsContainer { get; set; }
        public Vector3 ItemsOffset { get; set; }
        public int Columns { get; set; }
        public int Rows { get; set; }
        
        public override void SetConfig(BasePointConfig config)
        {
            Transform = transform;
            _inputPoint.Activate();
            _inputPoint.OnTriggerEnterAction += OnInputTriggerEnter;
            _inputPoint.OnTriggerExitAction += OnInputTriggerExit;
        }

        private void OnInputTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == GameLayers.PLAYER_LAYER)
            {
                _animation.Play("open");
            }
        }
        
        private void OnInputTriggerExit(Collider other)
        {
            if (other.gameObject.layer == GameLayers.PLAYER_LAYER)
            {
                _animation.Play("close");
            }
        }
    }
}