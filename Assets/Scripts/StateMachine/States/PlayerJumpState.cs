using StateMachine;
using UnityEngine;

public class PlayerJumpState : PlayerState
{
    private float _jumpTime;

    public PlayerJumpState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        // Apply the jump force upward
        Vector3 jumpVector = Vector3.up * _stateMachine.JumpForce;
        _stateMachine._rb.AddForce(jumpVector, ForceMode.Impulse);

        _jumpTime = Time.time;
    }

    public override void FixedUpdate()
    {
        HandleAirSteering();

        // Add a small grace period before checking grounded to ensure we've actually left the ground
        if (Time.time > _jumpTime + 0.2f && _stateMachine.IsGrounded)
        {
            Exit();
        }
    }

    private void HandleAirSteering()
    {
        // Use camera-relative movement
        Vector3 cameraForward = Vector3.ProjectOnPlane(_stateMachine.CameraTransform.forward, Vector3.up).normalized;
        Vector3 cameraRight = Vector3.ProjectOnPlane(_stateMachine.CameraTransform.right, Vector3.up).normalized;
        Vector3 moveInput = (cameraForward * _stateMachine.MovementDirection.y) + (cameraRight * _stateMachine.MovementDirection.x);

        if (moveInput.sqrMagnitude > 0.01f)
        {
            // Apply a force for steering
            float steeringForce = _stateMachine.walkSpeed * _stateMachine.AirControl * 5f;
            _stateMachine._rb.AddForce(moveInput * steeringForce, ForceMode.Acceleration);

            // Clamp horizontal velocity to max walkSpeed
            Vector3 horizontalVel = new Vector3(_stateMachine._rb.linearVelocity.x, 0, _stateMachine._rb.linearVelocity.z);
            if (horizontalVel.magnitude > _stateMachine.walkSpeed)
            {
                horizontalVel = horizontalVel.normalized * _stateMachine.walkSpeed;
                _stateMachine._rb.linearVelocity = new Vector3(horizontalVel.x, _stateMachine._rb.linearVelocity.y, horizontalVel.z);
            }
        }
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
