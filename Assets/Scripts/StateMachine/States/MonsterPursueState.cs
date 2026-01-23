using UnityEngine;

namespace StateMachine.States
{
    public class MonsterPursueState : MonsterState
    {
        public MonsterPursueState(MonsterStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            // Optionally set initial pursuit behavior
        }

        public override void Update()
        {
            // If player leaves vision, switch back to patrol
            if (!_stateMachine.CanSeePlayer())
            {
                _stateMachine.ChangeState(_stateMachine.PatrolState);
                return;
            }

            // Pursue logic: move towards the player
            if (_stateMachine.PlayerTarget != null)
            {
                float distance = Vector3.Distance(_stateMachine.transform.position, _stateMachine.PlayerTarget.position);
                if (distance <= _stateMachine.AttackRange)
                {
                    _stateMachine.ChangeState(_stateMachine.AttackState);
                    return;
                }

                _stateMachine.MoveTo(_stateMachine.PlayerTarget.position);
            }
            else
            {
                // If for some reason PlayerTarget is null while pursuing, go back to patrol
                _stateMachine.ChangeState(_stateMachine.PatrolState);
            }
        }

        public override void Exit()
        {
            // Clean up if necessary
        }
    }
}