namespace StateMachine.States
{
    public class PlayerRespawnState : PlayerState
    {
        public PlayerRespawnState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            // Respawn Logic here
            // Disable All movement and make player invunerable 
        }

        public override void Exit()
        {
            //Move To Idle State When Respawning is finished 
        }
    }
}
