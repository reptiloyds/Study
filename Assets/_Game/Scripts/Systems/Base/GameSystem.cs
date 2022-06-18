using System;
using _Game.Scripts.Balance;
using _Game.Scripts.Enums;
using _Game.Scripts.Factories;
using _Game.Scripts.Interfaces;
using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.Systems.Tutorial;
using _Game.Scripts.Ui;
using Zenject;

namespace _Game.Scripts.Systems.Base
{
    public class GameSystem : IGameParam, IGameProgress, ITickableSystem
    {
        private int _lastVisitedDayOfYear;
        
        [Inject] private ProjectSettings _projectSettings;
        [Inject] private GameFlags _flags;
        [Inject] private GameBalanceConfigs _balance;
        [Inject] private LoadingSystem _loader;
        [Inject] private LevelSystem _levels;
        [Inject] private GameParamFactory _params;
        [Inject] private GameProgressFactory _progresses;
        [Inject] private WindowsSystem _windows;
        [Inject] private ConnectionSystem _connection;
        [Inject] private TutorialSystem _tutorial;

        private GameParam _soft;
        private GameParam _hard;
        private GameParam _level;

        private const int DAILY_HOURS = 4;
        private DateTime _dailyRestartTime;

        public int Level => (int) (_projectSettings.DevBuild ? _levels.CurrentLevel.Id : _level.Value);
        public float ToNextDayTime => (float) (_dailyRestartTime - _connection.ServerTime).TotalSeconds;

        private enum GameState 
        {
            Loading,
            Play,
            Pause
        }

        private GameState _state;

        public bool GamePaused => _state is GameState.Pause or GameState.Loading;

        public GameSystem(LoadingSystem loader, LevelSystem levels)
        {
            _loader = loader;
            _loader.OnLoadedGame += OnLoadedGame;
            
            _levels = levels;
            _levels.OnLoadedLevel += OnLoadedLevel;

            _state = GameState.Loading;
        }

        public void Init()
        {
            _params.CreateParam(this, GameParamType.DailyQuestsRewardReceived, 0, true);
            // var dailyQuestProgress = _progresses.CreateProgress(this, GameParamType.DailyQuestsRewardCounter, 
            //     _balance.DefaultBalance.DailyQuestsComplete,
            //     false,
            //     true);
            
            _level = _params.CreateParam(this, GameParamType.Level, 1, true);
            _soft = _params.CreateParam(this, GameParamType.Soft, _balance.DefaultBalance.StartSoft, true);
            _hard = _params.CreateParam(this, GameParamType.Hard, 0, true);
        }

        private void OnLoadedGame()
        {
            _loader.OnLoadedGame -= OnLoadedGame;
            _state = GameState.Play;

            SetCurrentDay();
        }
        
        private void OnLoadedLevel()
        {
            // if (_flags.Has(GameFlag.TutorialFinished))
            // {
            //     _windows.OpenWindow<MainWindow>();  
            // }
        }

        public void IncLevel()
        {
            _level.Change(1);
        }

        public void AddCurrency(GameParamType type, float value)
        {
            switch (type)
            {
                case GameParamType.Soft:
                    _soft.Change(value);
                    break;
                
                case GameParamType.Hard:
                    _hard.Change(value);
                    break;
            }
        }
        
        public bool IsEnoughCurrency(GameParamType type, float needed)
        {
            switch (type)
            {
                case GameParamType.Soft:
                    return _soft.Value >= needed;

                case GameParamType.Hard:
                    return _hard.Value >= needed;
            }

            return false;
        }

        public void SpendCurrency(GameParamType type, float value, SpendCurrencyPlace place = SpendCurrencyPlace.None)
        {
            switch (type)
            {
                case GameParamType.Soft:
                    _soft.Change(-value);
                    break;
                
                case GameParamType.Hard:
                    _hard.Change(-value);
                    break;
            }

            if (place != SpendCurrencyPlace.None)
            {
                
            }
        }

        public void Tick(float deltaTime)
        {
            CheckLocalTimers();
        }

        private void CheckLocalTimers()
        {
            var time = _connection.ServerTime;
            if (time.DayOfYear == _lastVisitedDayOfYear || time.Hour != DAILY_HOURS) return;
            UpdateCurrentDay();
            ResetLocalTimers();
            _lastVisitedDayOfYear = time.DayOfYear;
        }

        private void SetCurrentDay()
        {
            if (_dailyRestartTime != DateTime.MinValue) return;
            var time = _connection.ServerTime;
            var newDay = _connection.ServerTime;
            if (time.Hour >= DAILY_HOURS && _lastVisitedDayOfYear != newDay.DayOfYear)
            {
                _lastVisitedDayOfYear = newDay.DayOfYear;
                ResetLocalTimers();
            }
            newDay = newDay.AddDays(1);
            _dailyRestartTime = new DateTime(newDay.Year, newDay.Month, newDay.Day, DAILY_HOURS, 0, 0);
        }
        
        private void UpdateCurrentDay()
        {
            var newDay = _connection.ServerTime.AddDays(1);
            _dailyRestartTime = new DateTime(newDay.Year, newDay.Month, newDay.Day, DAILY_HOURS, 0, 0);
        }
        
        private void ResetLocalTimers()
        {
            
        }
    }
}