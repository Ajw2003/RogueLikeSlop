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
    
    public override void FixedUpdate()
    {
        _stateMachine._rb.linearVelocity = new Vector3(_stateMachine.MovementDirection.x, _stateMachine._rb.linearVelocity.y, _stateMachine.MovementDirection.y) * _stateMachine.walkSpeed;
    }

    public override void Exit()
    {
        //Move to idle State When movement Stops over certain threshold like 0.5s of not more than 0.001 velocity
    }
}
