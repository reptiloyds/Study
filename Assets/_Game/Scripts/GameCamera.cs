using System;
using System.Collections.Generic;
using _Game.Scripts.Balance;
using _Game.Scripts.Enums;
using _Game.Scripts.Systems.Base;
using _Game.Scripts.Systems.Save;
using _Game.Scripts.Systems.Save.SaveStructures;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace _Game.Scripts
{
    public class GameCamera : MonoBehaviour
    {
        private GameBalanceConfigs _balance;
        private LevelSystem _levels;

        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        [SerializeField] private Transform _parent;

        [Inject] private AppEventProvider _appEventProvider;
        [Inject] private DiContainer _container;

        private Vector3 _savePosition;
        private Vector3 _saveRotation;
        
        private float _lastFOV;
        private Vector3 _lastPosition;
        private Vector3 _comeBackPosition;
        private Vector3 _comeBackRotation;
        private Vector3 _lastRotation;

        private List<CameraMoveElement> _moveSequence = new();

        public Camera UnityCam { get; set; }

        public CinemachineVirtualCamera VirtualCamera => _virtualCamera;

        public Vector3 SavePosition => _savePosition;
        public Vector3 SaveRotation => _saveRotation;

        public Transform Parent => _parent;

        public event Action CameraMoved;
        
        [Inject]
        public void Construct(LevelSystem levels, GameBalanceConfigs balance)
        {
            UnityCam = GetComponentInChildren<Camera>();
            
            _levels = levels;
            _levels.OnStartSession += OnStartSession;
            _balance = balance;

            _savePosition = _virtualCamera.transform.position;
            _saveRotation = _virtualCamera.transform.rotation.eulerAngles;
        }

        private void OnStartSession()
        {
            _levels.OnStartSession -= OnStartSession;
            _container.Resolve<SaveSystem>().LateLoadCamera();
        }

        public void ChangeSaveData(Vector3 position, Vector3 rotation, bool forceMove = false)
        {
            _savePosition = position;
            _saveRotation = rotation;


            if (forceMove)
            {
                _virtualCamera.transform.position = _savePosition;
                _virtualCamera.transform.rotation = Quaternion.Euler(_saveRotation);
            }
            else
            {
                var config = _balance.DefaultBalance.ChangeCameraPointConfig;
                var sequence = DOTween.Sequence();
            
                sequence.Append(_virtualCamera.transform.DOMove(_savePosition, config.CameraMoveTime / 2).SetEase(config.Ease));
                sequence.Append(_virtualCamera.transform.DORotate(_saveRotation, config.CameraMoveTime / 2).SetEase(config.Ease));   
            }
        }

        public void LoadSave(CameraData cameraData)
        {
            _savePosition = cameraData.Position;
            _saveRotation = cameraData.Rotation;

            _virtualCamera.transform.position = _savePosition;
            _virtualCamera.transform.rotation = Quaternion.Euler(_saveRotation);
        }

        public void Follow(Transform target, bool lookAt = true)
        {
            _virtualCamera.Follow = target;
		
            if (lookAt)
            {
                LookAt(target);
            }
        }

        public void TutorialMoveTo(Transform target, bool rotate = true, bool savePosition = true)
        {
            if (savePosition)
            {
                _lastPosition = _virtualCamera.transform.position;
                _lastRotation = _virtualCamera.transform.rotation.eulerAngles;   
            }

            var sequence = DOTween.Sequence();

            _lastFOV = _virtualCamera.m_Lens.FieldOfView;
			
            sequence.Append(DOTween.To(x => _virtualCamera.m_Lens.FieldOfView = x, _lastFOV,
                _balance.DefaultBalance.TutorialCameraMoveConfig.CameraFOV, _balance.DefaultBalance.TutorialCameraMoveConfig.CameraMoveTime).SetEase(_balance.DefaultBalance.TutorialCameraMoveConfig.Ease));
            sequence.Join(_virtualCamera.transform.DOMove(target.position,
                _balance.DefaultBalance.TutorialCameraMoveConfig.CameraMoveTime).SetEase(_balance.DefaultBalance.TutorialCameraMoveConfig.Ease));
            if (rotate)
            {
                sequence.Join(_virtualCamera.transform.DORotate(target.rotation.eulerAngles,
                    _balance.DefaultBalance.TutorialCameraMoveConfig.CameraMoveTime).SetEase(Ease.Linear));
            }
            sequence.OnComplete(() =>
            {
                CameraMoved?.Invoke();
                _appEventProvider.TriggerEvent(AppEventType.Tutorial, GameEvents.CameraMoved);
            });
        }
        
        public void TutorialRotateTo(Transform target)
        {
            var sequence = DOTween.Sequence();
            sequence.Join(_virtualCamera.transform.DORotate(target.rotation.eulerAngles, 0.5f).SetEase(Ease.Linear));
        }

        public void TutorialReturnCamera()
        {
            var sequence = DOTween.Sequence();
        
            sequence.Append(DOTween.To(x => _virtualCamera.m_Lens.FieldOfView = x, _balance.DefaultBalance.TutorialCameraMoveConfig.CameraFOV,
                _lastFOV, _balance.DefaultBalance.TutorialCameraMoveConfig.CameraMoveTime).SetEase(_balance.DefaultBalance.TutorialCameraMoveConfig.Ease));
            sequence.Join(_virtualCamera.transform.DOMove(_lastPosition,
                _balance.DefaultBalance.TutorialCameraMoveConfig.CameraMoveTime).SetEase(_balance.DefaultBalance.TutorialCameraMoveConfig.Ease));
            sequence.Join(_virtualCamera.transform.DORotate(_lastRotation,
                _balance.DefaultBalance.TutorialCameraMoveConfig.CameraMoveTime).SetEase(_balance.DefaultBalance.TutorialCameraMoveConfig.Ease));
            sequence.OnComplete(() =>
            {
                CameraMoved?.Invoke();
                _appEventProvider.TriggerEvent(AppEventType.Tutorial, GameEvents.CameraMoved);
            });
        }
        
        public void MoveTo(Transform cameraPoint)
        {
            _moveSequence.Clear();
            
            _lastPosition = _virtualCamera.transform.position;
            _lastRotation = _virtualCamera.transform.rotation.eulerAngles;
            
            _lastFOV = _virtualCamera.m_Lens.FieldOfView;

            var moveTime = _balance.DefaultBalance.CameraMoveConfig.CameraMoveTime;

            var sequence = DOTween.Sequence();

            sequence.Append(_virtualCamera.transform.DORotate(new Vector3(_virtualCamera.transform.rotation.eulerAngles.x,
                    _virtualCamera.transform.rotation.eulerAngles.y -_balance.DefaultBalance.YRotation, _virtualCamera.transform.rotation.eulerAngles.z),
                moveTime*0.3f).SetEase(_balance.DefaultBalance.CameraMoveConfig.Ease).OnComplete(() =>
            {
                _comeBackPosition = _virtualCamera.transform.position;
                _comeBackRotation = _virtualCamera.transform.rotation.eulerAngles;
            }));

            sequence.Append(_virtualCamera.transform.DOMove(cameraPoint.position,
                moveTime*0.7f).SetEase(_balance.DefaultBalance.CameraMoveConfig.Ease));
            
            sequence.Join(_virtualCamera.transform.DORotate(cameraPoint.rotation.eulerAngles,
                moveTime*0.7f).SetEase(_balance.DefaultBalance.CameraMoveConfig.Ease));

            sequence.Join(DOTween.To(x => _virtualCamera.m_Lens.FieldOfView = x, _lastFOV,
                    _balance.DefaultBalance.CameraMoveConfig.CameraFOV,
                    moveTime*0.7f)
                .SetEase(_balance.DefaultBalance.CameraMoveConfig.Ease));

            sequence.OnComplete(() => CameraMoved?.Invoke());
        }
        
        public void ReturnCamera()
        {
            var sequence = DOTween.Sequence();

            var moveTime = _balance.DefaultBalance.CameraMoveConfig.CameraMoveTime;
            
            
            sequence.Append(DOTween.To(x => _virtualCamera.m_Lens.FieldOfView = x, _balance.DefaultBalance.CameraMoveConfig.CameraFOV,
                _lastFOV, moveTime*0.7f).SetEase(_balance.DefaultBalance.CameraMoveConfig.Ease));
            
            sequence.Join(_virtualCamera.transform.DOMove(_comeBackPosition,
                moveTime*0.7f).SetEase(_balance.DefaultBalance.CameraMoveConfig.Ease));
            
            sequence.Join(_virtualCamera.transform.DORotate(_comeBackRotation,
                moveTime*0.7f).SetEase(_balance.DefaultBalance.CameraMoveConfig.Ease));
            
            sequence.Append(_virtualCamera.transform.DORotate(_lastRotation,
                moveTime*0.3f).SetEase(_balance.DefaultBalance.CameraMoveConfig.Ease));
            sequence.OnComplete(() =>
            {
                CameraMoved?.Invoke();
            });
        }

        public void LookAt(Transform target)
        {
            _virtualCamera.LookAt = target;
        }
    }

    public struct CameraMoveElement
    {
        public Vector3 Position;
        public Vector3 Rotation;
    }
}