namespace StateMachine.States
{
    public class PlayerDodgeState : PlayerState
    {
        public PlayerDodgeState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }
        
        public override void Enter()
        {
            //disable any other input but pause
        }
        public override void Exit()
        {
            //re enable input for dodge
            //re enable input for attack
            // re enable input for walk
            //re enable input for run
            
            //don't force state transfer just allow it to happen
        }
    }
}
