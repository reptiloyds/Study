using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Tools;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.ScriptableObjects
{
    /// <summary>
    /// Используется для кэширования префабов
    /// </summary>
    [CreateAssetMenu(fileName = "Prefabs", menuName = "_Game/Prefabs", order = 1)]
    public class Prefabs : ScriptableObjectInstaller
    {
        [SerializeField] private string[] _prefabsPath;
        [SerializeField] private List<MonoBehaviour> _prefabs;
        
        private static Dictionary<Type, MonoBehaviour> _cachedPrefabsSingle;
        private static Dictionary<Type, List<MonoBehaviour>> _cachedPrefabsGroups;

        private static Prefabs _instance;
        public static Prefabs Instance
        {
            get
            {
                if (_instance == null) _instance = Resources.Load<Prefabs>(nameof(Prefabs));
                return _instance;
            }
        }
        
        public void Initialize()
        {
            CachePrefabs();
        }
        
#if UNITY_EDITOR
        [ContextMenu("Parse prefabs")]
#if ODIN_INSPECTOR
        [Button("Parse prefabs")]
#endif
        public void ParsePrefabs()
        {
            _prefabs = UnityEditorTools.Find<MonoBehaviour>(_prefabsPath, UnityEditorTools.FilterTypes.Prefab);
            _prefabs.Sort((b1, b2) => string.CompareOrdinal(b1.GetType().Name, b2.GetType().Name));

            CachePrefabs();
        }
#endif
        
        private void CachePrefabs()
        {
            _cachedPrefabsSingle = new Dictionary<Type, MonoBehaviour>();
            _cachedPrefabsGroups = new Dictionary<Type, List<MonoBehaviour>>();

            if (_prefabs.Any(p => p == null))
            {
                Debug.LogError("Prefabs asset has null entries");
                _prefabs.RemoveAll(p => p == null);
            }

            foreach (var prefab in _prefabs)
            {
                var type = prefab.GetType();
                if (_cachedPrefabsGroups.ContainsKey(type))
                {
                    _cachedPrefabsGroups[type].Add(prefab);
                    continue;
                }

                if (!_cachedPrefabsSingle.ContainsKey(type))
                {
                    _cachedPrefabsSingle.Add(type, prefab);
                    continue;
                }

                var existing = _cachedPrefabsSingle[type];
                _cachedPrefabsSingle.Remove(type);
                _cachedPrefabsGroups.Add(type, new List<MonoBehaviour> {existing, prefab});
            }
        }
        
        public bool HasPrefab<T>(Predicate<T> predicate = default) where T : MonoBehaviour
        {
            var type = typeof(T);
            return _cachedPrefabsSingle.ContainsKey(type) || _cachedPrefabsGroups.ContainsKey(type);
        }
        
        public T GetPrefab<T>() where T : MonoBehaviour
        {
            var prefab = LoadPrefab<T>();
            return prefab;
        }
        
        public T CopyPrefab<T>(Transform parent = null, string name = "", bool activate = true) where T : MonoBehaviour
        {
            var prefab = LoadPrefab<T>();
            return prefab == null ? null : prefab.Copy(parent, name, activate);
        }
        
        public T LoadPrefab<T>(Predicate<T> predicate = default) where T : MonoBehaviour
        {
            if (predicate != default) return LoadAllPrefabs<T>().Find(predicate);

            var type = typeof(T);
            return _cachedPrefabsSingle.ContainsKey(type) ? _cachedPrefabsSingle[type] as T :
                _cachedPrefabsGroups.ContainsKey(type) ? _cachedPrefabsGroups[type][0] as T :
                default;
        }

        private List<T> LoadAllPrefabs<T>() where T : MonoBehaviour
        {
            var type = typeof(T);

            return _cachedPrefabsGroups.ContainsKey(type)
                ? _cachedPrefabsGroups[type].Cast<T>().ToList()
                : _cachedPrefabsSingle.ContainsKey(type)
                    ? new List<T> {_cachedPrefabsSingle[type] as T}
                    : new List<T>();
        }
    }
}