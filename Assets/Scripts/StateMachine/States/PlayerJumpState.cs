using StateMachine;
using UnityEngine;

public class PlayerJumpState : PlayerState 
{
    public PlayerJumpState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        //apply force upwards and in direction facing 
        //set in air bool to true
    }

    public override void Update()
    {
        //if(in air)
        {
            //call ground detection logic here
            //once ground hit after jump //set in air to false
            //Call Exit
        }
        
    }

    public override void Exit()
    {
        //re-enable main inputs jump/walk/run/attack/dodge
        //don't force state change
    }
}
