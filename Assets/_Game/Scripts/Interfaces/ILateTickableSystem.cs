namespace _Game.Scripts.Interfaces
{
    public interface ILateTickableSystem
    {
        void LateTick(float deltaTime);
    }
}