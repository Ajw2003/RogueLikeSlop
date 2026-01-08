namespace StateMachine.States
{
    public class PlayerAttackState : PlayerState
    {
        public PlayerAttackState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            //Disable all other input than pause 
            
            //run the attack logic
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
