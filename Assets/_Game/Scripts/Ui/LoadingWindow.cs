using _Game.Scripts.Systems.Base;
using _Game.Scripts.Ui.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;
using Zenject;

namespace _Game.Scripts.Ui
{
    public class LoadingWindow : BaseWindow
    {
        [SerializeField] private ProceduralImage _progress;
        [SerializeField] private TextMeshProUGUI _progressText;

        [Inject]
        public void Construct(LoadingSystem loading)
        {
            loading.OnUpdateProgress += UpdateProgress;
        }

        private void UpdateProgress(float value)
        {
            _progress.fillAmount = value;
            _progressText.text = $"{value * 100}%";
        }
    }
}