using UnityEngine;

namespace _Game.Scripts.View
{
    public class LevelView : MonoBehaviour
    {
        [SerializeField] private int _id;
        [SerializeField] private Transform _cameraPoint;

        public int Id => _id;

        public Transform CameraPoint => _cameraPoint;
    }
}