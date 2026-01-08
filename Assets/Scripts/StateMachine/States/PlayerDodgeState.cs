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
            Vector3 dodgeVector = new Vector3(_stateMachine.MovementDirection.x, 0, _stateMachine.MovementDirection.y ) * _stateMachine.DodgeForce;
            _stateMachine._rb.AddForce(dodgeVector, ForceMode.Impulse); 
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
