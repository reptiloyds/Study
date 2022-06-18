using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Interfaces;

namespace _Game.Scripts.Systems
{
    public class InvokeSystem
    {
        private static List<InvokeClass> _invokeClasses = new ();

        private static List<InvokeClass> _deleteInvokes = new();
        public delegate void InvokeCallback();

        public static void Tick(float deltaTime)
        {
            if (_invokeClasses == null) return;
			
            foreach (var method in _invokeClasses)
            {
            }

            // var count = _invokeClasses.Count;
            //
            // for (var i = 0; i < count; i++)
            // {
            //     _invokeClasses[i].Time -= deltaTime;
            //     if (_invokeClasses[i].Time <= 0f)
            //     {
            //         _invokeClasses[i].Callback?.Invoke();
            //         _invokeClasses.Remove(_invokeClasses[i]);
            //         break;
            //     }
            // }

            _deleteInvokes = new List<InvokeClass>(_invokeClasses);

            foreach (var deleteInvoke in _deleteInvokes)
            {
                deleteInvoke.Time -= deltaTime;
                if (deleteInvoke.Time <= 0f)
                {
                    deleteInvoke.Callback?.Invoke();
                    _invokeClasses.Remove(deleteInvoke);
                    break;
                }
            }
            
            _deleteInvokes.Clear();
        }
        
        public static void StartInvoke(InvokeCallback method, float time, bool checkExisting = false, IInvoke owner = null)
        {
            if (checkExisting)
            {
                if (_invokeClasses.FirstOrDefault(c => c.Callback.Target == method.Target) != null)
                {
                    return;
                }
            }

            var newInvoke = new InvokeClass
            {
                Callback = method,
                Time = + time,
                Owner = owner
            };
            _invokeClasses.Add(newInvoke);
        }

        public static void CancelInvoke(InvokeCallback method, bool all = false)
        {
            if (_invokeClasses == null) return;
            if (_invokeClasses.Count == 0) return;
            var targetInvokes = _invokeClasses.Where(invoke =>
                invoke.Callback.Target == method.Target && invoke.Callback.Equals(method)).ToList();
            if(targetInvokes.Count == 0) return;
            foreach (var invoke in targetInvokes)
            {
                _invokeClasses.Remove(invoke);
                if(!all) break;
            }
        }
        
        public static void CancelInvoke(IInvoke owner, bool all = false)
        {
            if (_invokeClasses == null) return;
            if (_invokeClasses.Count == 0) return;
            var targetInvokes = _invokeClasses.Where(invoke =>
                invoke.Owner == owner).ToList();
            if(targetInvokes.Count == 0) return;
            foreach (var invoke in targetInvokes)
            {
                _invokeClasses.Remove(invoke);
                if(!all) break;
            }
        }

        public static void Clear()
        {
            _invokeClasses.Clear();
        }
    }
    
    public class InvokeClass
    {
        public InvokeSystem.InvokeCallback Callback;
        public float Time;
        public IInvoke Owner;
    }

    public interface IInvoke
    {
        
    }
}
