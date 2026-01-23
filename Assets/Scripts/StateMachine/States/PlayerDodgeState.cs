using UnityEngine;

namespace StateMachine.States
{
    public class PlayerDodgeState : PlayerState
    {
        public PlayerDodgeState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }
        
        public override void Enter()
        {
            Vector3 inputDirection = new Vector3(_stateMachine.MovementDirection.x, 0, _stateMachine.MovementDirection.y);
            
            // Normalize to ensure consistent force magnitude (avoid faster diagonal dodges)
            if (inputDirection.sqrMagnitude > 1f)
            {
                inputDirection.Normalize();
            }

            // Transform direction from local (player-relative) to world space
            Vector3 dodgeDirection = _stateMachine.transform.TransformDirection(inputDirection);

            // If no input, default to dodging backward or just don't dodge? 
            // Usually games dodge backwards or current facing direction if no input. 
            // Let's assume if input is zero, we might want to dodge backwards or just forward?
            // Existing code implied (0,0,0) force if no input. Let's keep it but maybe handle zero case if needed.
            // If the user wants a specific 'no input' behavior (like backstep), I'd add it here.
            // For now, let's stick to the transform fix.
            
            _stateMachine._rb.AddForce(dodgeDirection * _stateMachine.DodgeForce, ForceMode.Impulse); 
        }

        public override void FixedUpdate()
        {
            if (_stateMachine._rb.linearVelocity.magnitude < 1.0f)
            {
                Exit();
            }
        }

        public override void Exit()
        {
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
}
