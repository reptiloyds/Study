using _Game.Scripts.Factories;
using _Game.Scripts.Systems.Base;
using _Game.Scripts.Systems.Save;
using _Game.Scripts.Systems.Tutorial;
using Zenject;

namespace _Game.Scripts.Tools
{
    public class TestTools
    {
#if UNITY_EDITOR
        private static DiContainer _container;

        private static TutorialSystem _tutorialSystem;

        public TestTools(DiContainer container, TutorialSystem tutorialSystem)
        {
            _container = container;
            _tutorialSystem = tutorialSystem;
        }

        public static void RunMagicTest(string param = "")
        {
            _container.Resolve<TutorialSystem>().StartTutorial(1, 1);
        }
#endif
    }
}