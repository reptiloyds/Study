using System.Collections.Generic;
using _Game.Scripts.Interfaces;
using _Game.Scripts.Systems.Base;
using _Game.Scripts.View.Units;
using Zenject;

namespace _Game.Scripts.Factories
{
    public class AIFactory : ITickableSystem
    {
        [Inject] private AIWorkerView.Pool _aiWorkerPool;

        private List<AIWorkerView> _aiWorker = new();

        [Inject]
        private void Construct(LevelSystem levelSystem)
        {
            levelSystem.OnDestroyLevel += OnDestroyLevel;
        }

        private void OnDestroyLevel()
        {
            foreach (var worker in _aiWorker)
            {
                worker.OnDestroy();
                _aiWorkerPool.Despawn(worker);
            }
            
            _aiWorker.Clear();
        }
        
        public void Tick(float deltaTime)
        {
            for (var i = 0; i < _aiWorker.Count; i++)
            {
                _aiWorker[i].Tick(deltaTime);
            }
        }

        public AIWorkerView SpawnWorker()
        {
            var worker = _aiWorkerPool.Spawn();
            _aiWorker.Add(worker);

            return worker;
        }

        public void RemoveWorker(AIWorkerView aiWorkerView)
        {
            _aiWorkerPool.Despawn(aiWorkerView);
            _aiWorker.Remove(aiWorkerView);
        }
    }
}