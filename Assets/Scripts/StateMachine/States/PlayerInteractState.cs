using StateMachine;
using UnityEngine;

public class PlayerInteractState : PlayerState 
{
    public PlayerInteractState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        //enable exit interact input
        //disable all input other than pause, and exit interact
    }
    
    public override void Exit()
    {
        //re-enable main inputs jump/walk/run/attack/dodge
        //don't force state change
    }
}
