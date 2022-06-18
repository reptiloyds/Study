using _Game.Scripts.Components.Base;
using _Game.Scripts.Systems;
using _Game.Scripts.Systems.Base;
using Zenject;

namespace _Game.Scripts.Components.Loader
{
    /// <summary>
    /// Класс для проверки интернета на этапе экрана загрузки
    /// </summary>
    public class ConnectionCheckComponent : BaseComponent
    {
        [Inject] private ConnectionSystem _connection;
        
        public override void Start()
        {
            _connection.Connected += OnConnected;
            _connection.RunCheckConnection();
            base.Start();
        }

        private void OnConnected(bool success)
        {
            if (!success) return;
            _connection.Connected -= OnConnected;
            End();
        }
    }
}