using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Enums;
using _Game.Scripts.Interfaces;
using _Game.Scripts.Systems;
using Sirenix.Serialization;

namespace _Game.Scripts.Factories
{
    public class GameProgressFactory : ITickableSystem
    {
        private readonly GameProgress.Pool _progressPool;
        private readonly List<GameProgress> _progresses = new();
        private readonly List<GameProgress> _savedProgresses = new();

        public GameProgressFactory(GameProgress.Pool progressPool)
        {
            _progressPool = progressPool;
        }

        public GameProgress CreateProgress(IGameProgress owner, GameParamType type, float target, bool looped = true, bool updatable = true, bool saved = false)
        {
            var progress = saved 
                ? _savedProgresses.Find(p => p.Owner == owner && p.Type == type)
                : _progresses.Find(p => p.Owner == owner && p.Type == type);
            
            if (progress != null) return progress;
            progress = _progressPool.Spawn(owner, type, target, looped, updatable);

            if (saved)
            {
                _savedProgresses.Add(progress);
            }
            else
            {
                _progresses.Add(progress);
            }
            
            return progress;
        }
        
        public void RemoveProgresses(IGameProgress owner)
        {
            var progresses = _progresses.FindAll(p => p.Owner == owner);
            foreach (var progress in progresses)
            {
                RemoveProgress(progress);
            }
        }

        private void RemoveProgress(GameProgress progress)
        {
            progress.Despawned();
            _progressPool.Despawn(progress);
            _progresses.Remove(progress);
        }

        public GameProgress GetProgress(IGameProgress owner, GameParamType type)
        {
            var savedParam = _progresses.FirstOrDefault(p => p.Owner == owner && p.Type == type);
            if (savedParam != null) return savedParam;
            var param = _savedProgresses.FirstOrDefault(p => p.Owner == owner && p.Type == type);
            return param;
        }
        
        public GameProgress GetProgress<T>(GameParamType type)
        {
            var savedProgress = _progresses.FirstOrDefault(p => p.Owner is T && p.Type == type);
            if (savedProgress != null) return savedProgress;
            var progress = _savedProgresses.FirstOrDefault(p => p.Owner is T && p.Type == type);
            return progress;
        }
        
        public void Tick(float deltaTime)
        {
            for (int i = 0; i < _progresses.Count; i++)
            {
                _progresses[i].Tick(deltaTime);
            }
        }
    }
}