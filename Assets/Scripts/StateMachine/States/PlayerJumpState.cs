using StateMachine;
using UnityEngine;

public class PlayerJumpState : PlayerState
{
    private float horizontalSpeed;
    private float _jumpTime;

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
        _jumpTime = Time.time;
    }

    public override void FixedUpdate()
    {
        HandleHorizontalMovement();

        // Add a small grace period before checking grounded to ensure we've actually left the ground
        if (Time.time > _jumpTime + 0.2f && _stateMachine.IsGrounded)
        {
            Exit();
        }
    }
    
    private void HandleHorizontalMovement()
    {
        // Preserve the current Y (vertical) velocity
        float currentVerticalVelocity = _stateMachine._rb.linearVelocity.y;

        // Calculate direction relative to player orientation
        Vector3 inputDir = new Vector3(_stateMachine.MovementDirection.x, 0, _stateMachine.MovementDirection.y);
        
        // Normalize input if magnitude > 1 to prevent faster diagonal movement
        if (inputDir.sqrMagnitude > 1f) inputDir.Normalize();

        Vector3 moveDir = _stateMachine.transform.TransformDirection(inputDir);

        Vector3 targetVelocity = moveDir * horizontalSpeed;
        targetVelocity.y = currentVerticalVelocity;

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
