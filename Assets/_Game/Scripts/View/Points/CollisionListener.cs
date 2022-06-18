using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.Tools;
using _Game.Scripts.View.Line;
using UnityEngine;

namespace _Game.Scripts.View.Points
{
    public enum CollisionListenerType
    {
        None,
        Animal,
        Grid,
    }

    public class CollisionListener : BaseView
    {
        [SerializeField] private CollisionListenerType _type;
        [SerializeField] private bool _startPoint;
        [SerializeField] private bool _block; 
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private MeshRenderer _outlineRenderer;
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Material _connectedMaterial;
        [SerializeField] private Material _unconnectedMaterial;
        [SerializeField] private GameObject _outline;
        [SerializeField] private List<BaseLineView> _baseLineViews;
        [SerializeField] private SpriteRenderer _target;

        private Color _color;
        private BaseLineView _currentLineView;
        private bool _targetComplete;


        private bool _connected;

        private readonly List<CollisionListener> _collisions = new();
        
        public bool Connected => _connected;
        public bool HaveCollision => _collisions.Count > 0;

        public event Action OnConnect;
        public event Action OnDisconnect;

        public event Action<CollisionListener> OnCollisionEnter; 
        public event Action<CollisionListener> OnCollisionExit; 

        private bool _canDrawLine;
        private bool _escapePoint;
        
        public bool StartPoint => _startPoint;
        public bool Block => _block;
        public bool CanDrawLine => _canDrawLine;
        public Color Color => _color;

        private void OnValidate()
        {
            if (_block)
            {
                OutlineDisable();
            }
            else
            {
                OutlineEnable();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_type == CollisionListenerType.Animal)
            {
                if (other.gameObject.layer == GameLayers.COLLISION_LISTENER_LAYER)
                {
                    var collisionListener = other.gameObject.GetComponent<CollisionListener>();
                    if (collisionListener is {Connected: false} && !_collisions.Contains(collisionListener) && collisionListener._type != _type)
                    {
                        _collisions.Add(collisionListener);
                        collisionListener.Connect();
                        OnCollisionEnter?.Invoke(this);
                    }
                }   
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_type == CollisionListenerType.Animal)
            {
                if (other.gameObject.layer == GameLayers.COLLISION_LISTENER_LAYER)
                {
                    var collisionListener = other.gameObject.GetComponent<CollisionListener>();
                    if (collisionListener != null)
                    {
                        _collisions.Remove(collisionListener);
                        collisionListener.Disconnect();
                        OnCollisionExit?.Invoke(this);
                    }
                }   
            }
        }
        
        // public void ConnectClosest()
        // {
        //     var closestCollision = GetClosestCollision();
        //     if (closestCollision != null)
        //     {
        //         closestCollision.Connect();   
        //     }
        //     Connect();
        // }
        //
        // public void DisconnectClosest()
        // {
        //     var closestCollision = GetClosestCollision();
        //     if (closestCollision != null)
        //     {
        //         closestCollision.Disconnect();   
        //     }
        //     Disconnect();
        // }
        //
        // private CollisionListener GetClosestCollision()
        // {
        //     var minDistance = float.MaxValue;
        //     CollisionListener closestCollision = null;
        //     foreach (var collision in _collisions)
        //     {
        //         var distance = Vector3.Distance(transform.position, collision.transform.position);
        //         if (distance < minDistance)
        //         {
        //             minDistance = distance;
        //             closestCollision = collision;
        //         }
        //     }
        //
        //     return closestCollision;
        // }
        
        public void Connect()
        {
            _connected = true;
            _meshRenderer.sharedMaterial = _connectedMaterial;

            // OutlineDisable();

            OnConnect?.Invoke();
        }

        public void Disconnect()
        {
            _connected = false;

            _meshRenderer.sharedMaterial = _unconnectedMaterial;
            
            // OutlineEnable();

            OnDisconnect?.Invoke();
        }

        public void SetCollisionListenerType(CollisionListenerType type)
        {
            _type = type;
        }

        public void SetGridItem(GridItem gridItem)
        {
            if (_block)
            {
                OutlineDisable();
            }
        }

        private void ChangeColor(Color color)
        {
            _color = color;
            
            _outlineRenderer.material.color = _color;
        }
        
        public void CompleteTarget()
        {
            _targetComplete = false;
            _target.Deactivate();
        }

        public void MakeEscapePoint(TrainConfig trainConfig)
        {
            _escapePoint = true;
            _target.Activate();
            _target.color = trainConfig.Color;
        }
        
        public void MakeSimplePoint()
        {
            _escapePoint = false;
            _target.Deactivate();
        }
        
        public void ActivateLineView(LineType lineType)
        {
            DeactivateAllLine();
            var baseLineView = _baseLineViews.FirstOrDefault(item => item.LineType == lineType);
            if(baseLineView == null) return;
            _currentLineView = baseLineView;
            _currentLineView.Activate();
        }

        public void DeactivateAllLine()
        {
            foreach (var baseLineView in _baseLineViews)
            {
                baseLineView.Deactivate();
            }
        }

        public void Rotate(List<Alignment> neighborAlignments)
        {
            _currentLineView.Rotate(neighborAlignments);
        }

        public void SetLineColor(Color color)
        {
            foreach (var lineView in _baseLineViews)
            {
                lineView.SetColor(color);
            }
        }

        public void OutlineEnable()
        {
            _outline.Activate();
        }

        public void OutlineDisable()
        {
            _outline.Deactivate();
        }

        public void GenerateTarget(TrainConfig trainConfig)
        {
            _target.color = trainConfig.Color;
            _target.Activate();
        }

        public void DisableDrag()
        {
            _canDrawLine = false;
        }

        public void EnableDrag()
        {
            _canDrawLine = true;
        }

        public void BlockTile()
        {
            _block = true;
        }

        public void UnblockTile()
        {
            _block = false;
        }
    }
}