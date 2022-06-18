using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Enums;
using _Game.Scripts.Interfaces;
using _Game.Scripts.Systems;

namespace _Game.Scripts.Factories
{
    /// <summary>
    /// Класс хранения параметров объектов
    /// </summary>
    public class GameParamFactory
    {
        private readonly GameParam.Pool _paramsPool;
        private readonly List<GameParam> _params = new();
        private readonly List<GameParam> _savedParams = new();

        public List<GameParam> SavedParams => _savedParams;
        
        public GameParamFactory(GameParam.Pool paramsPool)
        {
            _paramsPool = paramsPool;
        }

        public GameParam CreateParam(IGameParam owner, GameParamType type, float value, bool saved = false)
        {
            var param = saved 
                ? _savedParams.Find(p => p.Owner == owner && p.Type == type)
                : _params.Find(p => p.Owner == owner && p.Type == type);
            
            if (param != null) return param;
            param = _paramsPool.Spawn(owner, type, value);

            if (saved)
            {
                _savedParams.Add(param);
            }
            else
            {
                _params.Add(param);
            }
            
            return param;
        }

        public void RemoveParams(IGameParam owner)
        {
            var ownerParams = _params.FindAll(p => p.Owner == owner);
            foreach (var ownerParam in ownerParams)
            {
                RemoveParam(ownerParam);
            }
        }
        
        public void RemoveParam(GameParam param)
        {
            _paramsPool.Despawn(param);
            _params.Remove(param);
            _params.Remove(param);
        }

        public GameParam GetParam(IGameParam owner, GameParamType type)
        {
            var savedParam = _savedParams.FirstOrDefault(p => p.Owner == owner && p.Type == type);
            if (savedParam != null) return savedParam;
            var param = _params.FirstOrDefault(p => p.Owner == owner && p.Type == type);
            return param;
        }
        
        public GameParam GetParam<T>(GameParamType type)
        {
            var savedParam = _savedParams.FirstOrDefault(p => p.Owner is T && p.Type == type);
            if (savedParam != null) return savedParam;
            var param = _params.FirstOrDefault(p => p.Owner is T && p.Type == type);
            return param;
        }

        public List<GameParam> GetSavedParams<T>()
        {
            return _savedParams.FindAll(p => p.Owner is T);
        }

        public float GetParamValue(IGameParam owner, GameParamType type)
        {
            var savedParam = _savedParams.FirstOrDefault(p => p.Owner == owner && p.Type == type)?.Value;
            if (savedParam != null) return savedParam.Value;
            var param = _params.FirstOrDefault(p => p.Owner == owner && p.Type == type)?.Value ?? 0;
            return param;
        }
        
        public float GetParamValue<T>(GameParamType type)
        {
            var savedParam = _savedParams.FirstOrDefault(p => p.Owner is T && p.Type == type)?.Value;
            if (savedParam != null) return savedParam.Value;
            var param = _params.FirstOrDefault(p => p.Owner is T && p.Type == type)?.Value ?? 0;
            return param;
        }
    }
}