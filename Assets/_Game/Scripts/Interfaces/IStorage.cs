using _Game.Scripts.View.CollectableItems;
using UnityEngine;

namespace _Game.Scripts.Interfaces
{
    public interface IStorage
    {
        //Общие настройки мест хранения
        public ItemStorageType StorageType { get; set; }
        public Transform Transform { get; set; }
        public Transform ItemsContainer { get; set; }
        
        //Настройки сетки
        public int Columns { get; set; }
        public int Rows { get; set; }
    }
}