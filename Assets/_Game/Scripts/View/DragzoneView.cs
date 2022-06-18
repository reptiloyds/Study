using UnityEngine;

namespace _Game.Scripts.View
{
    [RequireComponent(typeof(Collider))]
    public class DragzoneView : BaseView
    {
        [SerializeField] private float _yOffset;

        public float YOffset => _yOffset;
    }
}