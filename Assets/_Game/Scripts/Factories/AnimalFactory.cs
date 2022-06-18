using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Interfaces;
using _Game.Scripts.Systems.Base;
using _Game.Scripts.View.Animal;

namespace _Game.Scripts.Factories
{
    public class AnimalFactory : ITickableSystem
    {
        private LevelSystem _levelSystem;
        
        private List<BaseAnimal> _baseAnimals;

        public AnimalFactory(LevelSystem levelSystem)
        {
            _levelSystem = levelSystem;
            _levelSystem.OnLoadedLevel += OnLoadedLevel;
            _levelSystem.OnDestroyLevel += OnDestroyLevel;
        }

        private void OnLoadedLevel()
        {
            _baseAnimals = _levelSystem.CurrentLevel.GetComponentsInChildren<BaseAnimal>().ToList();
            foreach (var baseAnimal in _baseAnimals)
            {
                baseAnimal.Init();
            }
        }

        private void OnDestroyLevel()
        {
            _baseAnimals.Clear();
        }

        public void Tick(float deltaTime)
        {
            var updateAnimals = new List<BaseAnimal>(_baseAnimals);
            foreach (var updateAnimal in updateAnimals)
            {
                updateAnimal.Tick(deltaTime);
            }
        }
    }
}