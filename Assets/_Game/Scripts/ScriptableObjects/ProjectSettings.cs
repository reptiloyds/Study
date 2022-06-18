using UnityEngine;
using Zenject;

namespace _Game.Scripts.ScriptableObjects
{
    /// <summary>
    /// Используется хранения настроек проекта
    /// </summary>
    [CreateAssetMenu(fileName = "ProjectSettings", menuName = "_Game/ProjectSettings", order = 1)]
    public class ProjectSettings : ScriptableObjectInstaller
    {
        [Header("Game settings")] 
        public bool DevBuild;
        public bool ClearSaves;
        public bool SkipTutorial;
        public int ReturnWindowOfflineTime;
        
        [Header("Internet settings")]
        public string Host;
        public int InternetCheckConnectionDelay;
            
        [Header("Links")]
        public string TermsLink;
        public string PrivacyLink;
        public string AppStoreLink;
        public string GooglePlayLink;
    }
}