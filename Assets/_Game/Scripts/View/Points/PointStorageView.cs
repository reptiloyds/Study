using _Game.Scripts.Enums;
using _Game.Scripts.Interfaces;
using _Game.Scripts.Systems;
using _Game.Scripts.View.CollectableItems;
using UnityEngine;

namespace _Game.Scripts.View.Points
{
    public class PointStorageView : BaseView, IStorage, IInvoke
    {
        [SerializeField] private GameParamType _paramType;
        [SerializeField] private ItemStorageType _storageType;
        [SerializeField] private Transform _inputItemContainer;
        [SerializeField] private int _inputStorageRows;
        [SerializeField] private int _inputStorageColumns;
        
        public ItemStorageType StorageType { get; set; }
        public Transform Transform { get; set; }
        public Transform ItemsContainer { get; set; }
        public int Columns { get; set; }
        public int Rows { get; set; }
        public GameParamType ParamType => _paramType;

        public override void Init()
        {
            StorageType = _storageType;
            Transform = transform;
            ItemsContainer = _inputItemContainer;
            Columns = _inputStorageRows;
            Rows = _inputStorageColumns;
        }
    }
}