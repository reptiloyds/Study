using System;
using System.Collections.Generic;
using _Game.Scripts.Enums;
using _Game.Scripts.Factories;
using _Game.Scripts.Systems.Base;
using _Game.Scripts.Systems.Save.SaveStructures;
using _Game.Scripts.Ui;
using UnityEngine;

namespace _Game.Scripts.Systems.Save
{
	public class Snapshot: ISerializationCallbackReceiver
    {
        public DateTime Time;
        public GameData GameData;
        public PointsData PointsData;
        public FlagData FlagData;
        public CameraData CameraData;

        [SerializeField] private string _gameDataStr = string.Empty;
        [SerializeField] private string _pointsDataStr = string.Empty;
        [SerializeField] private string _flagsDataStr = string.Empty;
        [SerializeField] private string _cameraDataStr = string.Empty;

        public void SaveGameData(GameParamFactory paramFactory)
        {
            var dataParams = paramFactory.GetSavedParams<GameSystem>();
            GameData.Params = new List<GameParamsData>();
            foreach (var param in dataParams)
            {
                GameData.Params.Add(new GameParamsData{Type = param.Type, Value = param.Value});
            }
        }

        public void SavePointsData(PointsFactory pointsFactory)
        {
            var points = pointsFactory.GetPoints();
            PointsData.Points = new List<PointData>();
            foreach (var point in points)
            {
                PointsData.Points.Add(new PointData{Region = point.Region, Id = point.ID, Level = point.Level, Type = point.Type, RemainingMoney = point.GetRemainingMoney(), IsBought = point.IsBought});
            }
        }

        public void SaveGameFlags(GameFlags gameFlags)
        {
            var flags = gameFlags.GetAllFlags();
            FlagData.Flags = new List<GameFlag>();
            FlagData.Flags.AddRange(flags);
        }

        public void SaveCameraData(GameCamera gameCamera)
        {
            CameraData.Position = gameCamera.SavePosition;
            CameraData.Rotation = gameCamera.SaveRotation;
        }

        public void OnBeforeSerialize()
        {
            _gameDataStr = JsonUtility.ToJson(GameData);
            _pointsDataStr = JsonUtility.ToJson(PointsData);
            _flagsDataStr = JsonUtility.ToJson(FlagData);
            _cameraDataStr = JsonUtility.ToJson(CameraData);
        }

        public void OnAfterDeserialize()
        {
            GameData = JsonUtility.FromJson<GameData>(_gameDataStr);
            PointsData = JsonUtility.FromJson<PointsData>(_pointsDataStr);
            FlagData = JsonUtility.FromJson<FlagData>(_flagsDataStr);
            CameraData = JsonUtility.FromJson<CameraData>(_cameraDataStr);
        }

        public void LoadGameData(GameData source, GameParamFactory paramFactory)
        {
            foreach (var sourceParam in source.Params)
            {
                var param = paramFactory.GetParam<GameSystem>(sourceParam.Type);
                param?.SetValue(sourceParam.Value);
            }
        }

        public void LoadPointsData(PointsData source, PointsFactory pointsFactory)
        {
            foreach (var sourcePoint in source.Points)
            {
                var point = pointsFactory.GetPoint(sourcePoint.Id, sourcePoint.Type, sourcePoint.Region);
                point?.LoadSave(sourcePoint);
            }
        }

        public void LoadGameFlags(FlagData source, GameFlags gameFlags)
        {
            foreach (var flag in source.Flags)
            {
                gameFlags.Set(flag);
            }
        }

        public void LoadCameraData(CameraData source, GameCamera camera)
        {
            camera.LoadSave(source);
        }
    }
}