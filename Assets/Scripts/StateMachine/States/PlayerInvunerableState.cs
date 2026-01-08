namespace StateMachine.States
{
    public class PlayerInvunerableState : PlayerState
    {
        public PlayerInvunerableState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            // Disable All movement and make player invunerable 
        }
    }
}
