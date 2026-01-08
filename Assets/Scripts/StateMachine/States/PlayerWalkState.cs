using StateMachine;
using UnityEngine;

public class PlayerWalkState : PlayerState 
{
    public PlayerWalkState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        
    }

    public override void Update()
    {
        //Call Walking movement logic here
        //Update direction based on keypress
    }

    public override void Exit()
    {
        //Move to idle State When movement Stops over certain threshold like 0.5s of not more than 0.001 velocity
    }
}
