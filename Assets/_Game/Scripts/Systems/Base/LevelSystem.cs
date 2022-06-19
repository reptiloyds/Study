using System;
using _Game.Scripts.Balance;
using _Game.Scripts.Core;
using _Game.Scripts.Enums;
using _Game.Scripts.Factories;
using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.Systems.Save;
using _Game.Scripts.Tools;
using _Game.Scripts.Ui;
using _Game.Scripts.View;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.Systems.Base
{
    /// <summary>
    /// Класс для загрузки/уничтожения уровней
    /// </summary>
    public class LevelSystem
    {
        public Action OnLoadedLevel;
        public Action OnPlayLevel;
        public Action OnDestroyLevel;
        public Action OnGameStart;
        public Action OnStartSession;
            
        [Inject] private ProjectSettings _projectSettings;
        [Inject] private SceneData _root;
        [Inject] private Prefabs _prefabs;
        [Inject] private AppEventProvider _appEventProvider;
        [Inject] private DiContainer _container;
        [Inject] private WindowsSystem _windows;
        [Inject] private GameBalanceConfigs _balance;
        [Inject] private GameCamera _gameCamera;

        public LevelView CurrentLevel { get; private set; }

        private int _level;

        public void LoadNextLevel()
        {
            if (_projectSettings.DevBuild)
            {
                LoadLevel(1);
            }
            else
            {
                _level = _container.Resolve<GameSystem>().Level;
                LoadLevel(_level);   
            }
        }

        private void LoadLevel(int id)
        {
            if (CurrentLevel != null) DestroyLevel();

            if (_projectSettings.DevBuild)
            {
                CurrentLevel = _root.GetComponentInChildren<LevelView>(true);
                if (CurrentLevel == null)
                {
                    CurrentLevel = _prefabs.LoadPrefab<LevelView>(config => config.Id == id);
                    if (CurrentLevel == null)
                    {
                        Debug.LogWarning($"(0001) Level with index {id} not found");
                        return;
                    }
                    CurrentLevel = _container.InstantiatePrefab(CurrentLevel, _root.transform).GetComponent<LevelView>();
                }
                else
                {
                    _container.BindInterfacesAndSelfTo<LevelView>().FromInstance(CurrentLevel).AsSingle().NonLazy();
                }
            }
            else
            {
                var levelId = _level > _balance.DefaultBalance.MaxLevels
                    ? _balance.DefaultBalance.MaxLevels
                    : _level;
                
                CurrentLevel = _prefabs.LoadPrefab<LevelView>(config => config.Id == levelId);
                if (CurrentLevel == null)
                {
                    Debug.LogWarning($"(0001) Level with index {id} not found");
                    return;
                }
                
                CurrentLevel = _container.InstantiatePrefab(CurrentLevel, _root.transform).GetComponent<LevelView>();
            }
            
            CurrentLevel.Activate();
            
            // _gameCamera.ChangeSaveData(CurrentLevel.CameraPoint.position, CurrentLevel.CameraPoint.rotation.eulerAngles, true);
            // _gameCamera.VirtualCamera.transform.SetParent(CurrentLevel.CameraPoint);

            _appEventProvider.TriggerEvent(GameEvents.LevelStart, CurrentLevel.Id);

            OnLoadedLevel?.Invoke();
            OnStartSession?.Invoke();
            
            // if (id > 1) _windows.GetWindow<MainWindow>().Open();
            _windows.OpenWindow<GameWindow>();
        }
        
        public void GameStart()
        {
            OnGameStart?.Invoke();
        }

        public void PlayLevel()
        {
            OnPlayLevel?.Invoke();
            // _windows.OpenWindow<GameWindow>();
        }
        
        public void RestartLevel()
        {
            OnDestroyLevel?.Invoke();
            InvokeSystem.Clear();
            OnLoadedLevel?.Invoke();
            // _windows.GetWindow<MainWindow>().Open();
        }

        private void DestroyLevel()
        {
            OnDestroyLevel?.Invoke();
            InvokeSystem.Clear();
            _container.Unbind<LevelView>();
            CurrentLevel.gameObject.Destroy();
            CurrentLevel = null;
        }
    }
}