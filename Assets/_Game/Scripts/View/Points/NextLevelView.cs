using _Game.Scripts.Interfaces;
using _Game.Scripts.Systems.Base;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.View.Points
{
    public class NextLevelView : BasePointView, IBuyElement
    {
        [SerializeField] private float _nextLevelDelay;

        [Inject] private GameSystem _gameSystem;
        [Inject] private LevelSystem _levelSystem;
        
        public void Buy()
        {
            DOTween.Sequence().AppendInterval(_nextLevelDelay).OnComplete(() =>
            {
                _gameSystem.IncLevel();
                _levelSystem.LoadNextLevel();
            });
        }

        public void Block()
        {
            
        }

        public void Restore()
        {
            
        }
    }
}