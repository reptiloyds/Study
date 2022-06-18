using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Game.Scripts.View.Line
{
    public enum Alignment
    {
        None,
        Right,
        Left,
        Forward,
        Backward,
    }
    
    public enum LineType
    {
        None,
        SimpleLine,
        PointLine,
        RotationLine,
        Point,
    }
    
    public class BaseLineView : BaseView
    {
        [SerializeField] private LineType _lineType;
        [SerializeField] private List<LineRotationConfig> _lineRotationConfigs;
        [SerializeField] private Transform _body;
        [SerializeField] private List<MeshRenderer> _meshRenderers;

        public LineType LineType => _lineType;

        public virtual void Rotate(List<Alignment> neighborAlignments)
        {
            var compareResult = _lineRotationConfigs.FirstOrDefault(item => item.Compare(neighborAlignments));
            if (compareResult == null)
            {
                Debug.LogError($"Can`t find LineRotationConfig type of {neighborAlignments}");
            }
            else
            {
                _body.transform.localRotation = Quaternion.Euler(0, compareResult.YRotation, 0);
            }
        }

        public virtual void SetColor(Color color)
        {
            foreach (var meshRenderer in _meshRenderers)
            {
                meshRenderer.material.color = color;
            }
        }
    }

    [Serializable]
    public class LineRotationConfig
    {
        [SerializeField] private List<Alignment> _neighborAlignments;
        [SerializeField] private float _yRotation;
        
        public float YRotation => _yRotation;

        public bool Compare(List<Alignment> neighborAlignments)
        {
            var result = false;
            if (neighborAlignments.Count != _neighborAlignments.Count) return result;
            foreach (var neighbor in neighborAlignments)
            {
                if (!_neighborAlignments.Contains(neighbor))
                {
                    result = false;
                    break;
                }
                result = true;
            }

            return result;
        }
    }
}