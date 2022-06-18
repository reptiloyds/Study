using System;

namespace _Game.Scripts.Components.Base
{
    public abstract class BaseComponent
    {
        public Action OnEnd;
        
        public virtual void Init()
        {
        }

        public virtual void Start()
        {
        }

        public virtual void Update(float deltaTime)
        {
        }

        public virtual void End()
        {
            OnEnd?.Invoke();
        }
    }
}