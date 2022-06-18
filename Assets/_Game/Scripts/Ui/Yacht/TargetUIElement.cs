using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.Tools;
using _Game.Scripts.Ui.Base;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Game.Scripts.Ui.Yacht
{
    public class TargetUIElement : BaseUIView
    {
        [SerializeField] private List<TrainTaskUI> _trainTaskUis;

        private TaskConfig _taskConfig;
        public TaskConfig Config => _taskConfig;
        
        public class Pool : MonoMemoryPool<TargetUIElement>
        {
            protected override void Reinitialize(TargetUIElement item)
            {
                item.Reset();
            }
        }

        private void Reset()
        {
            foreach (var trainUi in _trainTaskUis)
            {
                trainUi.Deactivate();
                trainUi.Clear();
            }
        }

        public void SetTask(TaskConfig taskConfig)
        {
            _taskConfig = taskConfig;
            foreach (var trainConfig in _taskConfig.TrainConfigs)
            {
                var trainUi = _trainTaskUis.FirstOrDefault(item => item.Type == trainConfig.Type && item.TrainConfig == null);
                if (trainUi != null)
                {
                    trainUi.SetTrainConfig(trainConfig);
                    trainUi.Activate();
                }
            }
        }

        public void Complete(TrainConfig trainConfig)
        {
            var trainTaskUI = _trainTaskUis.FirstOrDefault(item => item.TrainConfig == trainConfig);
            if (trainTaskUI != null)
            {
                trainTaskUI.Complete();
            }
        }
    }
}