using System.Linq;
using System.Runtime.CompilerServices;
using _Game.Scripts.Balance;
using _Game.Scripts.Enums;
using _Game.Scripts.Factories;
using _Game.Scripts.Interfaces;
using _Game.Scripts.Systems.Base;
using _Game.Scripts.View.Points.Buildings;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.Systems
{
    public class EventBehaviourSystem : IGameProgress
    {
        [Inject] private PointsFactory _pointsFactory;

        private GameFlags _gameFlags;
        private GameBalanceConfigs _balance;
        private GameProgressFactory _progress;
        private LevelSystem _levelSystem;
        
        private GameProgress _freeMerge;

        public EventBehaviourSystem(LevelSystem levelSystem, GameProgressFactory gameProgressFactory, GameBalanceConfigs balance, GameFlags gameFlags)
        {
            _gameFlags = gameFlags;
            _levelSystem = levelSystem;
            _progress = gameProgressFactory;
            _balance = balance;

            _levelSystem.OnGameStart += OnGameStart;
            _levelSystem.OnLoadedLevel += PlayEventsDelay;
            _levelSystem.OnDestroyLevel += PauseEventsDelay;
        }

        public void OnGameStart()
        {
            _levelSystem.OnGameStart -= OnGameStart;
            Init();
        }

        private void Init()
        {
            CreateProgresses();
            PauseEventsDelay();
        }

        private void CreateProgresses()
        {
        }

        private void PauseEventsDelay()
        {
        }

        private void PlayEventsDelay()
        {
        }
    }
}