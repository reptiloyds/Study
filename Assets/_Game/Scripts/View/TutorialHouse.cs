using UnityEngine;

namespace _Game.Scripts.View
{
    public class TutorialHouse : BaseView
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _cameraPoint;
        [SerializeField] private Transform _firstItemPosition;
        [SerializeField] private Transform _secondItemPosition;

        [SerializeField] private Transform _carSpawnPoint;
        [SerializeField] private Transform _carEndPoint;
        
        public Transform FirstItemPosition => _firstItemPosition;
        public Transform SecondItemPosition => _secondItemPosition;
        public Transform CarSpawnPoint => _carSpawnPoint;
        public Transform CarEndPoint => _carEndPoint;
        public Transform CameraPoint => _cameraPoint;

        public void PlayAnimation()
        {
            _animator.Play("Look Around");
        }
    }
}