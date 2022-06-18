using System.Collections.Generic;
using _Game.Scripts.Tools;
using _Game.Scripts.Ui.Base;
using DG.Tweening;
using UnityEngine;

namespace _Game.Scripts.Systems.Tutorial
{
    public class TutorialArrow : BaseUIView
    {
        [SerializeField] private RectTransform _rect; 
        private RectTransform _defaultParent;
        private Tween _tween;
        private int _positionId;
        private List<Vector3> _positions;

        public void Init()
        {
            _defaultParent = transform.parent.GetComponent<RectTransform>();
            Hide();
        }
        
        public void Show(RectTransform element)
        {
            RectTransform.SetParent(element, false);

            var anchoredPosition = new Vector2();
            var scale = element.localScale;
            var rotate = element.rotation;
			
            if (element.TryGetComponent(out CustomTutorialArrowAnchor customTutorialArrowAnchor))
            {
                anchoredPosition = customTutorialArrowAnchor.Anchor;
                scale = customTutorialArrowAnchor.Scale;
                rotate = customTutorialArrowAnchor.Rotation;
            }
		
            RectTransform.anchoredPosition = anchoredPosition;
            RectTransform.localScale = scale;
            RectTransform.rotation = rotate;
            this.Activate();
        }

        public void MoveBySequence(List<Vector3> positions)
        {
            _positions = positions;
            _positionId = 0;
            _rect.position = _positions[_positionId];
            NextStep();
            this.Activate();
        }

        private void NextStep()
        {
            _positionId++;
            if (_positionId >= _positions.Count)
            {
                _positionId = 0;
                _rect.position = _positions[_positionId];
                NextStep();
            }
            else
            {
                var distance = Vector3.Distance(_positions[_positionId], _positions[_positionId - 1]);
                _tween = _rect.DOMove(_positions[_positionId], distance / 100).SetEase(Ease.Linear).OnComplete(NextStep);
            }
        }

        public void StopSequence()
        {
            _tween?.Kill();
            this.Deactivate();
        }

        public void ShowWorldSpace()
        {
            Show(_defaultParent);
        }

        public void Hide()
        {
            this.Deactivate();
        }
    }
}