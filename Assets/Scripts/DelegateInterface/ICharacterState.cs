namespace Fineallday.DelegateInterface
{
    public interface ICharacterState
    {
        void UpdateStateHandler(PlayerController pc);
        bool PingStateHandler(PlayerController pc);
    }
}
