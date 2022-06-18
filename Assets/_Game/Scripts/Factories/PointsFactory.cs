using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Game.Scripts.Balance;
using _Game.Scripts.Enums;
using _Game.Scripts.Interfaces;
using _Game.Scripts.Systems.Base;
using _Game.Scripts.Systems.Save;
using _Game.Scripts.View.CollectableItems;
using _Game.Scripts.View.Points;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.Factories
{
    public class PointsFactory : ITickableSystem
    {
        private readonly DiContainer _container;
        private readonly GameSystem _gameSystem;
        private readonly GameBalanceConfigs _balance;
        private readonly LevelSystem _levels;
        private readonly SaveSystem _saveSystem;
        
        private readonly List<BasePointView> _points = new();

        private BasePointView _currentPoint;

        public PointsFactory(DiContainer container, GameBalanceConfigs balance, LevelSystem levels, GameSystem gameSystem, SaveSystem saveSystem)
        {
            _container = container;
            _gameSystem = gameSystem;
            _balance = balance;
            _levels = levels;
            _saveSystem = saveSystem;
            _levels.OnLoadedLevel += OnLoadedLevel;
            _levels.OnPlayLevel += OnPlayLevel;
            _levels.OnDestroyLevel += OnDestroyLevel;
        }

        private void OnLoadedLevel()
        {
            UpdatePoints();
        }
        
        private void OnPlayLevel()
        {
            foreach (var point in _points)
            {
                point.Play();
            }
        }
        
        private void OnDestroyLevel()
        {
            foreach (var point in _points)
            {
                var type = point.GetType();
                _container.Unbind(type);
                point.OnDestroy();
            }
            
            _points.Clear();
        }

        private void UpdatePoints()
        {
            var points = _levels.CurrentLevel.GetComponentsInChildren<BasePointView>();
            int id = 0;
            foreach (var point in points)
            {
                var config = _balance.DefaultBalance.Points.FirstOrDefault(p => p.Type == point.Type);
                _container.BindInstance(point);
                point.SetConfig(config);
                point.Init();
                point.SetId(id);
                point.SetRegion(_gameSystem.Level);
                
                _points.Add(point);

                id++;
            }
            
            _saveSystem.LateLoadData();
        }

        public BasePointView GetPointViewTypeOf(PointType type)
        {
            return _points.FirstOrDefault(item => item.Type == type);
        }

        public List<BasePointView> GetPoints()
        {
            return _points;
        }

        public BasePointView GetPoint(int id, PointType pointType, int region)
        {
            return _points.FirstOrDefault(item => item.ID == id && item.Type == pointType && item.Region == region);
        }

        public void Tick(float deltaTime)
        {
            for (var i = 0; i < _points.Count; i++)
            {
                _points[i].Tick(deltaTime);
            }
        }
    }
}