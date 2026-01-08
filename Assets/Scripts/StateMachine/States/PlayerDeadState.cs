namespace StateMachine.States
{
    public class PlayerDeadState : PlayerState
    {
        public PlayerDeadState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            //Disable all Input But Pause
        }

        public override void Exit()
        {
            //force player into respawn state after delay
        }
    }
}
