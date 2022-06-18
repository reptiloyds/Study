using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.Tools;
using _Game.Scripts.View;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Game.Scripts.UI.WorldSpace
{
    public class Bubble : BaseView
    {
        [SerializeField] private RectTransform _textContainer;
        [SerializeField] private RectTransform _textBg;
        [SerializeField] private Image _textContainerImg;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private float _maxTextWidth = 75f;
        [SerializeField] private Color _oneColorIcon;
        
        private Vector3 _offsetPosition;
        private ContentSizeFitter _contentSizeFitter;
        private Animation _textImgAnimation;

        private bool _updatable;
        private float _timer;

        [Inject] private GameResources _gameResources;
        
        public class Pool : MonoMemoryPool<Bubble>
        {
            protected override void Reinitialize(Bubble bubble)
            {
                bubble.Reset();
            }
        }

        public override void Init()
        {
            transform.localScale = Vector3.one;
            this.Activate();
        }

        protected override void Reset()
        {
            this.Deactivate();
            base.Reset();
        }

        public void Show(string spriteName, bool oneColored = true)
        {
            var sprite = _gameResources.GetSprite(spriteName);
            Show(sprite, oneColored);
        }

        public void Show(Sprite img, bool oneColored = true)
        {
            _textImgAnimation ??= _textContainerImg.GetComponent<Animation>();
            _textContainerImg.color = oneColored ? _oneColorIcon : Color.white;
            _text.text = "";
            _textImgAnimation.enabled = !oneColored;
            _textContainerImg.Activate();
            _textContainerImg.sprite = img;
            _textContainer.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            _textContainer.DOScale(1f, 0.3f);
            _textBg.sizeDelta = new Vector2(12f, 12f);
        }
        

        public void ShowText(string message)
        {
            _contentSizeFitter ??= _text.GetComponent<ContentSizeFitter>();
            _textContainerImg.Deactivate();
            _text.text = message;
            _textContainer.SetActive(true);
            _textContainer.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            _textContainer.DOScale(1f, 0.3f);
            var width = LayoutUtility.GetPreferredWidth(_text.rectTransform);
            var height = 0f;
            if (width < _maxTextWidth)
            {
                height = 6.6f;
                _contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
            else
            {
                width = _maxTextWidth;
                _contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                _text.rectTransform.sizeDelta = new Vector2(_maxTextWidth, 6f);
                height = LayoutUtility.GetPreferredHeight(_text.rectTransform);
            }
            _textBg.sizeDelta = new Vector2( width + 10f,
                height + 6f);
        }
    }
}
