using System;
using _Game.Scripts.Enums;
using _Game.Scripts.Interfaces;
using Zenject;

namespace _Game.Scripts.Systems
{
    public class GameParam
    {
        public event Action UpdatedEvent;
        
        public IGameParam Owner { get; private set; }
        public GameParamType Type { get; private set; }
        public float Value { get; private set; }

        private void Init(IGameParam owner, GameParamType type, float value)
        {
            Owner = owner;
            Type = type;
            Value = value;
        }

        public void Change(float value)
        {
            Value += value;
            UpdatedEvent?.Invoke();
        }
        
        public virtual void SetValue(float value, bool updateEvent = true)
        {
            Value = value;
            if (updateEvent) UpdatedEvent?.Invoke();
        }
        
        public class Pool : MemoryPool<IGameParam, GameParamType, float, GameParam>
        {
            protected override void Reinitialize(IGameParam owner, GameParamType type, float target, GameParam param)
            {
                param.Init(owner, type, target);
            }
        }
    }
}