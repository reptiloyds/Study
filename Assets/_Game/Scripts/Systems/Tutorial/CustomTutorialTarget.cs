using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.View;
using UnityEngine;

namespace _Game.Scripts.Systems.Tutorial
{
    public class CustomTutorialTarget : BaseView
    {
        [SerializeField] private TutorialTarget _target;
        [SerializeField] private Vector3 _offset;

        public TutorialTarget Target => _target;
        public Vector3 Offset => _offset;
    }
}