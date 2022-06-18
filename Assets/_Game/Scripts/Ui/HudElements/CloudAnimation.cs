using System.Collections.Generic;
using _Game.Scripts.View;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Scripts.UI.HudElements
{
    public class CloudAnimation : BaseView
    {
        [SerializeField] private List<RectTransform> _clouds;
        [SerializeField] private Animation _animation;

        [Button]
        public void TestShow()
        {
            ShowClouds();
        }

        [Button]
        public void TestHide()
        {
            HideClouds();
        }

        public void ShowClouds()
        {
            foreach (var cloud in _clouds)
            {
                cloud.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
            }
            _animation.Play("CloudsClose");
        }

        public void HideClouds()
        {
            _animation.Play("CloudsOpen");
        }
    }
}