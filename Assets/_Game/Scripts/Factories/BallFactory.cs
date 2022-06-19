using System.Collections.Generic;
using _Game.Scripts.Interfaces;
using _Game.Scripts.View.Balls;
using Zenject;

namespace _Game.Scripts.Factories
{
    public class BallFactory
    {
        [Inject] private BaseBall.Pool _ballPool;
        private List<BaseBall> _balls = new ();

        public BaseBall SpawnBall()
        {
            var ball = _ballPool.Spawn();
            _balls.Add(ball);
            return ball;
        }

        public void RemoveBall(BaseBall ball)
        {
            _balls.Remove(ball);
            _ballPool.Despawn(ball);
        }
    }
}