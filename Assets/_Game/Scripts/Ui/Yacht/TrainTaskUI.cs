using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.Tools;
using _Game.Scripts.Ui.Base;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.Ui.Yacht
{
    public class TrainTaskUI : BaseUIView
    {
        [SerializeField] private TaskType _taskType;
        [SerializeField] private Image _image;
        [SerializeField] private GameObject _completeObject;

        private TrainConfig _trainConfig;

        public TrainConfig TrainConfig => _trainConfig;
        public TaskType Type => _taskType;

        public void SetTrainConfig(TrainConfig trainConfig)
        {
            _trainConfig = trainConfig;
            _image.color = _trainConfig.Color;
        }

        public void Clear()
        {
            _trainConfig = null;
            _completeObject.Deactivate();
        }

        public void Complete()
        {
            _completeObject.Activate();
        }
    }
}