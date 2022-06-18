using System;
using System.Collections.Generic;
using _Game.Scripts.Enums;
using _Game.Scripts.Systems.Tutorial;
using _Game.Scripts.Ui.ResourceBubbles;
using _Game.Scripts.View.Other;
using UnityEngine;

namespace _Game.Scripts.Core
{
    public class SceneData : MonoBehaviour
    {
        [SerializeField] private GameCamera _camera;
        [SerializeField] private FloatingJoystick _joystick;
        [SerializeField] private Transform _orderContainer;
        [SerializeField] private Transform _connectContainer;
        
        [Header("UI")]
        [SerializeField] private Transform _ui;
        [SerializeField] private Transform _windowsOverlay;
        [SerializeField] private WorldSpaceCanvas _worldSpaceCanvas;
        [SerializeField] private List<ResourceBubbleSetup> _resourceBubbleSetups;
        [SerializeField] private Transform _maxText;
        [SerializeField] private TutorialArrow _tutorialArrow;
        [SerializeField] private RectTransform _mainCanvas;

        [Header("Settings")]
        [SerializeField] private Vector3 _itemFocusOffset;

        public GameCamera Camera => _camera;
        public Transform UI => _ui;
        public Transform WindowsOverlay => _windowsOverlay;
        public Vector3 ItemFocusOffset => _itemFocusOffset;
        public List<ResourceBubbleSetup> ResourceBubbleSetups => _resourceBubbleSetups;
        public Transform WorldSpaceCanvas => _worldSpaceCanvas.transform;
        public FloatingJoystick Joystick => _joystick;
        public Transform OrderContainer => _orderContainer;

        public Transform MaxText => _maxText;
        public TutorialArrow Arrow => _tutorialArrow;
        public RectTransform MainCanvas => _mainCanvas;
        public Transform ConnectContainer => _connectContainer;
    }
    
    [Serializable]
    public class CoinConfig
    {
        public float Speed = 2.5f;
        public float DropRange = 1f;
        public float JumpForce = 1f;
        public float JumpTime = 1f;
        public float Timer = 3;
    }
}