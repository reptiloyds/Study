using _Game.Scripts.Interfaces;
using _Game.Scripts.Systems;
using _Game.Scripts.View.Fx;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.Factories
{
    public class FxFactory : ITickableSystem
    {
        [Inject] private UniversalFx.Pool _fxPool;

        public void PlayFx(FXType fxType, float time, Vector3 position)
        {
            var fx = _fxPool.Spawn(fxType);
            fx.transform.position = position;
            fx.Play();
            InvokeSystem.StartInvoke(() => RemoveFx(fx), time);
        }
        
        public void RemoveFx(UniversalFx fx)
        {
            _fxPool.Despawn(fx);
        }

        /*public TFx ShowFx<TFx>(Vector3 position) where TFx : BaseFx
        {
            var fx = Pool.Pop<TFx>(_map.View.transform);
            fx.transform.position = position;
            _activeFx.Add(fx);
            _map.StartInvoke(this, RemoveFx, 5f);
            return fx;
        }

        public TFx ShowFxWithText<TFx>(string value, Vector3 position) where TFx : BaseFx
        {
            var fx = ShowFx<TFx>(position);
            ShowFx<TextFx>(position).Init(value);
            return fx;
        }
		
        public TFx ShowUIFx<TFx>(Vector3 position) where TFx : BaseFx
        {
            var fx = Pool.Pop<TFx>(_canvas.transform);
            fx.transform.position = position;
            _activeFx.Add(fx);
            _map.StartInvoke(this, RemoveFx, 5f);
            return fx;
        }

        private void RemoveFx()
        {
            _activeFx[0]?.PushToPool();
            _activeFx.RemoveAt(0);
        }*/
        public void Tick(float deltaTime)
        {
            
        }
    }
}
