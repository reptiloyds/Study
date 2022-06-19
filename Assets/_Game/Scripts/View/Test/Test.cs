using System;
using _Game.Scripts.Systems.Tools;
using UnityEngine;

namespace _Game.Scripts.View.Test
{
    public class Test : BaseView
    {
        [SerializeField] private RectTransform _followTarget;
        [SerializeField] private Camera _camera;

        private void LateUpdate()
        {
            transform.position = ToolsSystem.GetWorldPositionOfCanvasElement(_camera, _followTarget);
        }
    }
}