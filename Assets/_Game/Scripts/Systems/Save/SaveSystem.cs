using System;
using _Game.Scripts.Enums;
using _Game.Scripts.Factories;
using _Game.Scripts.Interfaces;
using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.Systems.Base;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.Systems.Save
{
    /// <summary>
    /// Данный класс для загрузки и сохранения данных
    /// </summary>
    public class SaveSystem : IGameProgress
    {
        [Inject] private ProjectSettings _projectSettings;
        [Inject] private GameParamFactory _params;
        [Inject] private GameProgressFactory _progress;
        [Inject] private PointsFactory _pointsFactory;
        [Inject] private GameFlags _gameFlags;
        [Inject] private GameCamera _gameCamera;

        private const string KEY = "snapshot";
        private const float _autosaveDelay = 3;
        
        private Snapshot _snapshot;

        public event Action SaveLoaded;

        // public SaveSystem(LevelSystem levelSystem)
        // {
        //     levelSystem.OnLoadedLevel += OnLoadedLevel;
        // }
        //
        // private void OnLoadedLevel()
        // {
        //     LoadPointsData();
        // }

        public void Init()
        {
            _snapshot = new Snapshot();
            var autoSave = _progress.CreateProgress(this, GameParamType.Timer, _autosaveDelay);
            autoSave.CompletedEvent += Save;
        }

        public void Save()
        {
            SaveData();
            var json = JsonUtility.ToJson(_snapshot);
            PlayerPrefs.SetString(KEY, json);
            PlayerPrefs.Save();
        }

        private void SaveData()
        {
            _snapshot.SaveGameData(_params);
            _snapshot.SavePointsData(_pointsFactory);
            _snapshot.SaveGameFlags(_gameFlags);
            _snapshot.SaveCameraData(_gameCamera);
        }

        public void Load()
        {
            if (!PlayerPrefs.HasKey(KEY)) return;
            
            if (_projectSettings.ClearSaves)
            { 
                DeleteSave();
                return;
            }
            
            try
            {
                var json = PlayerPrefs.GetString(KEY);
                var snapshot = JsonUtility.FromJson<Snapshot>(json);
                LoadData(snapshot);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"(0004) Error. Load preview save. {e}");
                DeleteSave();
            }
        }

        private void LoadData(Snapshot snapshot)
        {
            _snapshot.LoadGameFlags(snapshot.FlagData, _gameFlags);
            if (!_gameFlags.Has(GameFlag.TutorialFinished))
            {
                snapshot.PointsData.Clear();
                snapshot.GameData.Clear();
                snapshot.OnBeforeSerialize();
                Save();
            }
            
            _snapshot.LoadGameData(snapshot.GameData, _params);
        }

        public void LateLoadData()
        {
            if (!PlayerPrefs.HasKey(KEY)) return;
            
            if (_projectSettings.ClearSaves)
            { 
                DeleteSave();
                return;
            }
            
            try
            {
                var json = PlayerPrefs.GetString(KEY);
                var snapshot = JsonUtility.FromJson<Snapshot>(json);
                _snapshot.LoadPointsData(snapshot.PointsData, _pointsFactory);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"(0004) Error. Load preview save. {e}");
                DeleteSave();
            }
        }

        public void LateLoadCamera()
        {
            if (!PlayerPrefs.HasKey(KEY)) return;
            
            if (_projectSettings.ClearSaves)
            { 
                DeleteSave();
                return;
            }
            
            try
            {
                var json = PlayerPrefs.GetString(KEY);
                var snapshot = JsonUtility.FromJson<Snapshot>(json);
                _snapshot.LoadCameraData(snapshot.CameraData, _gameCamera);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"(0004) Error. Load preview save. {e}");
                DeleteSave();
            }
        }

        private void DeleteSave()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
    }
}