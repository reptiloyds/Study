using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Balance.Attributes;
using _Game.Scripts.Enums;
using _Game.Scripts.Tools;
using _Game.Scripts.View.Line;
using _Game.Scripts.View.Points;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "BalanceConfig", menuName = "_Game/BalanceConfig", order = 0)]
    [TableSheetName("Constants")]
    public class BalanceConfig : ScriptableObject
    {
        public float StartSoft;
        public int MaxLevels;
        public float CollectDelay;
        public float YRotation;
        [Space] [Title("AnimalSetting")]
        public float AnimalSpeed;
        [Space]

        public CameraMoveConfig ChangeCameraPointConfig;
        public CameraMoveConfig CameraMoveConfig;
        public CameraMoveConfig TutorialCameraMoveConfig;
        
        public List<LevelConfig> Levels;
        public List<UnitConfig> Units;
        public List<BasePointConfig> Points;
        public List<SkillConfig> Skills;
        public List<TutorialConfig> Tutorial;
    }

    [Serializable]
    public class SkillConfig
    {
        public GameParamType Type;
        public float PriceStep;
        public float SkillStep;
    }

    [Serializable]
    public class LevelConfig
    {
        public int Id;
    }
    
    [Serializable]
    public class UnitConfig
    {
        public UnitType UnitType;
        public List<ParamConfig> ParamConfigs;
    }

    [Serializable]
    public enum UnitType
    {
        Player,
        AIUnit,
        Waiter
    }

    [Serializable]
    public class ParamConfig
    {
        public GameParamType ParamType;
        public float BaseValue;
    }
    
    [Serializable]
    public class BasePointConfig
    {
        public PointType Type;
        public List<CurrencyConfig> Prices;

        [Header("Params")]
        public List<PointLevelConfig> Levels;

        public ParamConfig GetParam(int level, GameParamType paramType)
        {
            var levelConfig = Levels.FirstOrDefault(l => l.Level == level) ?? Levels.LastValue();
            var param = levelConfig.Params.FirstOrDefault(x => x.ParamType == paramType) ?? new ParamConfig
            {
                ParamType = paramType,
                BaseValue = 1.0f
            };

            return param;
        }
    }
    
    [Serializable]
    public class PointLevelConfig
    {
        public int Level;
        public List<ParamConfig> Params;
    }

    [Serializable]
    public class CurrencyConfig
    {
        public int Level;
        public List<ParamConfig> Prices;
    }

    [Serializable]
    public class TutorialConfig
    {
        public int Region;
        public int StepId;
        public TutorialStepAction StepAction;
        public string StepActionParam1;
        public string AnalyticsStep;
        public string StepActionParam2;
    }

    [Serializable]
    public enum TutorialStepAction
    {
        None, 
        StartTutorial,
    }
    
    [Serializable]
    public class Reward
    {
        public GameParamType Type;
        public int Id;
        public float Amount;
    }
    
    [Serializable]
    public enum TutorialTarget
    {
        None,
        GridSpawnPoint,
        PlaceItemPoint,
        TutorialHouse,
        Deactivate,
        SpawnGrid,
        SetPeoplePoint1,
        SetPeoplePoint2,
        SetPeoplePoint3,
        SetPeoplePoint4,
        RemovePeoplePoint1,
        RemovePeoplePoint2,
        RemovePeoplePoint3,
        BlockTile1,
        BlockTile2,
    }

    [Serializable]
    public class CameraMoveConfig
    {
        public Ease Ease;
        public float CameraMoveTime;
        public float CameraFOV;
    }

    [Serializable]
    public class TaskConfig
    {
        public int TaskId;
        public List<TrainConfig> TrainConfigs;
    }

    [Serializable]
    public class TrainConfig
    {
        public TaskType Type;
        public int TrainId;
        [ShowIf("Type", TaskType.SetTrain)] public int PeopleCount;
        [ShowIf("Type", TaskType.SetTrain)] public int PatternId;
        public Color Color;
    }

    public enum TaskType
    {
        None,
        SetTrain,
        RemoveTrain,
    }

    public enum TrainMoveType
    {
        None,
        BySequenceNavmesh,
        TogetherNavmesh,
        TogetherMoveTowards,
    }
}