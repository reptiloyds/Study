using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Enums;
using _Game.Scripts.Factories;
using _Game.Scripts.Interfaces;
using _Game.Scripts.Systems;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.View.Fx
{
    public class UniversalFx : BaseFx, IGameProgress
    {
        [SerializeField] private List<FxConfig> _configs;

        private FxConfig _config;

        public class Pool : MonoMemoryPool<FXType, UniversalFx>
        {
            protected override void Reinitialize(FXType type, UniversalFx item)
            {
                item.Reset(type);
            }
        }

        public void Play()
        {
            _config.ParticleSystem.Play();
        }

        private new void Reset(FXType type)
        {
            ChooseFxConfig(type);
            base.Reset();
        }

        private void ChooseFxConfig(FXType fxType)
        {
            var config = _configs.FirstOrDefault(item => item.Type == fxType);
            if(config == null) return;
            _config = config;
        }
    }

    [Serializable]
    public class FxConfig
    {
        [SerializeField] private FXType _fxType;
        [SerializeField] private ParticleSystem _particleSystem;

        public FXType Type => _fxType;
        public ParticleSystem ParticleSystem => _particleSystem;
    }

    public enum FXType
    {
        None,
        Poof,
    }
}