using UnityEngine;
using Zenject;

namespace _Game.Scripts.View.Balls
{
    public class BaseBall : BaseView
    {
        public class Pool : MonoMemoryPool<BaseBall>
        {
            protected override void Reinitialize(BaseBall item)
            {
                item.Reset();
            }
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }
    }
}