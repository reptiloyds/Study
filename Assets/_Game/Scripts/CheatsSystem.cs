using System;
using _Game.Scripts.Enums;
using _Game.Scripts.Interfaces;
using _Game.Scripts.Systems.Base;
using UnityEngine;
using Zenject;

namespace _Game.Scripts
{
    public class CheatsSystem : ITickableSystem
    {
        [Inject] private GameSystem _game;
        
        public void Tick(float deltaTime)
        {
            if (_game.GamePaused) return;
            
            Pause();
            
            if (Input.GetKeyDown(KeyCode.F11)) CheckTimeScale(3f);
            if (Input.GetKeyDown(KeyCode.F12)) CheckTimeScale(10f);
            if (Input.GetKeyDown(KeyCode.S)) AddSoft();
        }

        private static void CheckTimeScale(float value) => Time.timeScale = Time.timeScale > 1f ? 1f : value;
        private static bool IsKeyDown(KeyCode keyCode) => Input.GetKeyDown(keyCode);
        
        private static void Pause()
        {
            if (!IsKeyDown(KeyCode.P)) return;
            Time.timeScale = Time.timeScale >= 1 ? 0 : 1;
        }
        
        private void AddSoft()
        {
            _game.AddCurrency(GameParamType.Soft, 10000000);
        }
    }
}