using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Enums;
using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.Tools;
using _Game.Scripts.View;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.Ui
{
    public class BubbleView : BaseView
    {
        [SerializeField] private RectTransform _container;
        [Inject] private GameCamera _gameCamera;
        private List<BubbleItem> _bubbleItems = new();

        public override void Init()
        {
            _bubbleItems = GetComponentsInChildren<BubbleItem>(true).ToList();
            
            base.Init();
        }

        public void SetRequireList(List<RequireItem> requireItems)
        {
            Clear();
            foreach (var requireItem in requireItems)
            {
                var bubbleItem = _bubbleItems.FirstOrDefault(item => item.IsFree);
                if (bubbleItem != null)
                {
                    bubbleItem.SetType(requireItem.Type);
                    bubbleItem.SetCount(requireItem.Count);
                    bubbleItem.Activate();
                }
            }
        }

        public void RemoveItem(GameParamType type)
        {
            var bubbleItem = _bubbleItems.FirstOrDefault(item => item.Type == type);
            if (bubbleItem != null)
            {
                bubbleItem.DecreaseCount();
            }
        }
        
        public class Pool : MonoMemoryPool<Vector3, BubbleView>
        {
            protected override void Reinitialize(Vector3 position,
                BubbleView bubbleView)
            {
                bubbleView.Reset(position);
            }
        }

        private new void Reset(Vector3 position)
        {
            transform.position = position;
            RotateToCamera();
            
            base.Reset();
        }

        private void RotateToCamera()
        {
            transform.LookAt(transform.position + _gameCamera.VirtualCamera.transform.forward);
        }
        
        private void Clear()
        {
            foreach (var bubbleitem in _bubbleItems)
            {
                bubbleitem.Clear();
            }
        }
    }
}