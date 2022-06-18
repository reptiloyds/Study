using UnityEngine;

namespace _Game.Scripts.Tools
{
    public class RunMagicTestAtRuntime : MonoBehaviour
    {
        public void RunMagicTest()
        {
#if UNITY_EDITOR
            TestTools.RunMagicTest();      
#endif
        }
    }
}
