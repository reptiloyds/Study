using _Game.Scripts.Balance;
using _Game.Scripts.Enums;
using _Game.Scripts.Factories;
using _Game.Scripts.Interfaces;
using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.Systems;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.View.Units
{
    public class BaseUnitView : BaseView, IGameParam, IGameProgress
    {
        [Inject] protected GameBalanceConfigs Balance;
        [Inject] protected GameParamFactory Params; 
        [Inject] protected GameProgressFactory Progresses;
        [Inject] protected UIFactory UIFactory;

        private static readonly int ANIMATION_SPEED = Animator.StringToHash("Speed");
        
        protected Animator Animator;
        protected Quaternion CameraRotation;
        
        protected GameParam Speed;

        [HideInInspector] public UnitConfig UnitConfig;
        [HideInInspector] public UnitState State;

        public override void Init()
        {
            CreateParameters();
            base.Init();
        }
        
        private void CreateParameters()
        {
            if (UnitConfig == null) return;
            
            foreach (var paramConfig in UnitConfig.ParamConfigs)
            {
                var param = Params.GetParam(this, paramConfig.ParamType);
                if (param == null)
                {
                    Params.CreateParam(this, paramConfig.ParamType, paramConfig.BaseValue);
                }
            }
            Speed = Params.GetParam(this, GameParamType.Speed);
        }

        public virtual void Tick(float deltaTime)
        {
        }

        protected void SetState(UnitState state)
        {
            if (state == State) return;
            
            State = state;
            
            switch (state)
            {
                case UnitState.Idle:
                    PlayAnimation(UnitAnimationType.Idle);
                    break;
                
                case UnitState.Moving:
                    break;
            }
        }

        private void SetLayerWeight(int id, float weight)
        {
            Animator.SetLayerWeight(id, weight);
        }
        
        public void SetLayerWeight(int id, float weight, float time)
        {
            var t = GetLayerWeight(id);
            DOTween.To(() => t, x => SetLayerWeight(id, x), weight, time);
        }

        private float GetLayerWeight(int id)
        {
            return Animator.GetLayerWeight(id);
        }

        protected void SetAnimatorFloat(AnimationFloat animationFloat, float value)
        {
            switch (animationFloat)
            {
                case AnimationFloat.MoveSpeed:
                    Animator.SetFloat(ANIMATION_SPEED, value);
                    break;
                case AnimationFloat.HasStuff:
                    Animator.SetLayerWeight(1, value);
                    break;
            }
        }

        private void PlayAnimation(UnitAnimationType animationType)
        {
            switch (animationType)
            {
                case UnitAnimationType.Idle:
                    break;
                case UnitAnimationType.Move:
                    break;
            }
        }

        public virtual void SetDmg(float dmg)
        {
        }
    }
    
    public enum AnimationFloat
    {
        MoveSpeed,
        HasStuff,
    }
    
    public enum UnitState
    {
        Idle,
        Moving,
    }
}