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
            
            // if(_mesh) _mesh.sharedMesh = _configs.Length > id ? _configs[id].Mesh : _configs[^1].Mesh;
            // if(_meshRender) _meshRender.materials = _configs.Length > id ? _configs[id].Materials : _configs[^1].Materials;
            // if (_skinnedMesh)
            // {
            //     _skinnedMesh.sharedMesh = _configs.Length > id ? _configs[id].Mesh : _configs[^1].Mesh;
            //     _skinnedMesh.materials = _configs.Length > id ? _configs[id].Materials : _configs[^1].Materials;
            // }
        }
        
        // public void Show(int id = 0)
        // {
        //     if (_configs.Length == 0)
        //     {
        //         this.Deactivate();
        //         return;
        //     }
        //     
        //     if(_mesh) _mesh.sharedMesh = _configs.Length > id ? _configs[id].Mesh : _configs[^1].Mesh;
        //     if(_meshRender) _meshRender.materials = _configs.Length > id ? _configs[id].Materials : _configs[^1].Materials;
        //     if (_skinnedMesh)
        //     {
        //         _skinnedMesh.sharedMesh = _configs.Length > id ? _configs[id].Mesh : _configs[^1].Mesh;
        //         _skinnedMesh.materials = _configs.Length > id ? _configs[id].Materials : _configs[^1].Materials;
        //     }
        // }
        //
        // public void Show(int id, bool selected = false)
        // {
        //     if (_configs.Length == 0)
        //     {
        //         this.Deactivate();
        //         return;
        //     }
        //
        //     if (selected && id == 0) id = 1;
        //
        //     if(_mesh) _mesh.sharedMesh = _configs.Length > id ? _configs[id].Mesh : _configs[_configs.Length - 1].Mesh;
        //     if(_meshRender) _meshRender.materials =
        //         _configs.Length > id ? _configs[id].Materials : _configs[_configs.Length - 1].Materials;
        //     if (_skinnedMesh)
        //     {
        //         _skinnedMesh.sharedMesh = _configs.Length > id ? _configs[id].Mesh : _configs[^1].Mesh;
        //         _skinnedMesh.materials = _configs.Length > id ? _configs[id].Materials : _configs[^1].Materials;
        //     }
        //     
        //     if (selected)
        //     {
        //         var materials = _meshRender.materials;
        //         for (int i = 0; i < _meshRender.materials.Length; i++)
        //         {
        //             // var material = new Material(_resources.SelectedItemMaterial);
        //             // materials[i] = material;
        //         }
        //
        //         _meshRender.materials = materials;
        //     }
        //     
        //     if(!_mesh) return;
        //     if(_mesh.sharedMesh && !isActiveAndEnabled)this.Activate();
        //     if(!_mesh.sharedMesh && isActiveAndEnabled)this.Deactivate();
        // }
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