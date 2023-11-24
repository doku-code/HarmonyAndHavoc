namespace Fineallday.StatePattern
{
    public abstract class PlayerState
    {
        protected PlayerController playerC;

        protected PlayerState(PlayerController player)
        {
            playerC = player;
        }

        public abstract void OnEnterState();
        public abstract void OnUpdateState();
        public abstract void OnExitState();
    }
}