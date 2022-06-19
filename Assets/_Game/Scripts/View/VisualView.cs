using System;
using System.Linq;
using _Game.Scripts.Enums;
using _Game.Scripts.Tools;
using ModestTree;
using UnityEngine;

namespace _Game.Scripts.View
{
    public class VisualView : MonoBehaviour
    {
        [SerializeField] private VisualConfig[] _configs;
        [SerializeField] private MeshFilter _mesh;
        [SerializeField] private SkinnedMeshRenderer _skinnedMesh;
        [SerializeField] private MeshRenderer _meshRender;

        public VisualConfig CurrentConfig { get; private set; }

        public void Init()
        {
            if (_mesh == null) _mesh = GetComponent<MeshFilter>();
            if (_skinnedMesh == null) _skinnedMesh = GetComponent<SkinnedMeshRenderer>();
            if (_meshRender == null) _meshRender = GetComponent<MeshRenderer>();
        }
        
        public void Show(GameParamType type)
        {
            if (_configs.Length == 0)
            {
                this.Deactivate();
                return;
            }

            CurrentConfig = _configs.FirstOrDefault(c => c.ParamType == type);
            var id = _configs.IndexOf(CurrentConfig);
            if (id < 0) return;

            foreach (var config in _configs)
            {
                config.GameObject.Deactivate();
            }
            
            CurrentConfig.GameObject.Activate();
        }
    }
    
    [Serializable]
    public class VisualConfig
    {
        public GameParamType ParamType;
        public Mesh Mesh;
        public GameObject GameObject;
        public Material[] Materials;
        public float YScale;
    }
}