using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.ScriptableObjects
{
    /// <summary>
    /// Используется для хранения ресурсов
    /// </summary>
    [CreateAssetMenu(fileName = "GameResource", menuName = "_Game/GameResource", order = 1)]
    public class GameResources : ScriptableObjectInstaller
    {
        [SerializeField] private List<Sprite> _sprites;
        [SerializeField] private Sprite _placeholder;
        
        private static GameResources _instance;
        public static GameResources Instance
        {
            get
            {
                if (_instance == null) _instance = Resources.Load<GameResources>(nameof(GameResources));
                return _instance;
            }
        }
        
        public Sprite GetSprite<T>(T type) where T: Enum
        {
            var result = _sprites.FirstOrDefault(s => s.name.Equals(type.ToString()));
            return result == null ? _placeholder : result;
        }
        
        public Sprite GetSprite(string key)
        {
            var result = _sprites.FirstOrDefault(s => s.name == key);
            return result == null ? _placeholder : result;
        }
        
        [Button("Parse")]
        public void Parse()
        {
            _sprites = Resources.LoadAll<Sprite>("Sprites").ToList();
            
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }
}