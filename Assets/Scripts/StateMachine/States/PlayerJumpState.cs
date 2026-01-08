using StateMachine;
using UnityEngine;

public class PlayerJumpState : PlayerState
{
    private float horizontalSpeed;

    public PlayerJumpState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        // Apply the jump force upward
        Vector3 jumpVector = Vector3.up * _stateMachine.JumpForce;
        _stateMachine._rb.AddForce(jumpVector, ForceMode.Impulse);

        // Store the current horizontal speed for movement during jump
        horizontalSpeed = _stateMachine.walkSpeed * 0.7f;
    }

    public override void FixedUpdate()
    {
        HandleHorizontalMovement();

        if (_stateMachine.IsGrounded)
        {
            Exit();
        }
    }
    
    private void HandleHorizontalMovement()
    {
        // Preserve the current Y (vertical) velocity
        float currentVerticalVelocity = _stateMachine._rb.linearVelocity.y;

        // Calculate new horizontal velocity based on input
        Vector3 targetVelocity = new Vector3(
            _stateMachine.MovementDirection.x * horizontalSpeed,
            currentVerticalVelocity,
            _stateMachine.MovementDirection.y * horizontalSpeed
        );

        // Apply the new velocity while preserving the Y component from jumping physics
        _stateMachine._rb.linearVelocity = targetVelocity;
    }

    public override void Exit()
    {
        // Check if there's movement input to transition to WalkState, otherwise IdleState
        if (_stateMachine.MovementDirection.sqrMagnitude > 0.1f)
        {
            _stateMachine.ChangeState(_stateMachine.WalkState);
        }
        else
        {
            _stateMachine.ChangeState(_stateMachine.IdleState);
        }
    }
}
