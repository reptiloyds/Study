using _Game.Scripts.Balance;
using _Game.Scripts.Components.Base;
using Zenject;

namespace _Game.Scripts.Components.Loader
{
    /// <summary>
    /// Класс для инициализации баланса
    /// </summary>
    public class GameBalanceInitializeComponent : BaseComponent
    {
        [Inject] private GameBalanceConfigs _balance;

        public override void Start()
        {
            _balance.SetDefaultBalance();
            base.Start();
            End();
        }
    }
}