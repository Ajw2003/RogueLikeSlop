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
        if (_stateMachine.CameraTransform == null)
        {
            Debug.LogWarning("CameraTransform is not assigned in PlayerStateMachine. Cannot apply camera-relative movement.");
            return;
        }

        // Get camera's forward and right vectors, ignoring vertical component for movement
        Vector3 cameraForward = Vector3.ProjectOnPlane(_stateMachine.CameraTransform.forward, Vector3.up).normalized;
        Vector3 cameraRight = Vector3.ProjectOnPlane(_stateMachine.CameraTransform.right, Vector3.up).normalized;

        // Calculate movement direction relative to camera
        Vector3 moveDirection = (cameraForward * _stateMachine.MovementDirection.y) + (cameraRight * _stateMachine.MovementDirection.x);
        moveDirection.Normalize(); // Normalize to ensure consistent speed in all directions

        // Preserve the Y (vertical) velocity to not interfere with jumping/falling physics
        float currentVerticalVelocity = _stateMachine._rb.linearVelocity.y;

        // Apply horizontal movement while preserving vertical velocity
        _stateMachine._rb.linearVelocity = new Vector3(
            moveDirection.x * _stateMachine.walkSpeed,
            currentVerticalVelocity,
            moveDirection.z * _stateMachine.walkSpeed
        );
    }
    
    public override void FixedUpdate()
    {
        
    }

    public override void Exit()
    {
        //Move to idle State When movement Stops over certain threshold like 0.5s of not more than 0.001 velocity
    }
}
