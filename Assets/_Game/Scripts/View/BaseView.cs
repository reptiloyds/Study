using UnityEngine;

namespace _Game.Scripts.View
{
    public class BaseView : MonoBehaviour
    {
        public virtual void Init()
        {
        }
        
        protected virtual void Reset()
        {
        }

        public virtual void OnDestroy()
        {
        }
    }
}