using UnityEngine;

namespace _Game.Scripts.Ui.Base
{
    public class BaseUIView : MonoBehaviour
    {
        private bool _cached;
        private RectTransform _cachedRectTransform;
        
        public RectTransform RectTransform => _cached ? _cachedRectTransform : CacheInfo();

        private RectTransform CacheInfo()
        {
            if (_cached) return _cachedRectTransform;

            _cachedRectTransform = GetComponent<RectTransform>();
            _cached = true;

            return _cachedRectTransform;
        }
    }
}