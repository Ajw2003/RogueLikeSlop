using UnityEngine;

namespace StateMachine.States
{
    public class MonsterPatrolState : MonsterState
    {
        public MonsterPatrolState(MonsterStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            // Optionally set initial patrol point or behavior
        }

        public override void Update()
        {
            // Check for player vision
            if (_stateMachine.CanSeePlayer())
            {
                _stateMachine.ChangeState(_stateMachine.PursueState);
                return;
            }

            // Patrol logic
            if (_stateMachine.PatrolPoints.Count == 0)
            {
                // If no patrol points, maybe just idle or stand still
                _stateMachine.ChangeState(_stateMachine.IdleState);
                return;
            }

            Vector3 targetPoint = _stateMachine.PatrolPoints[_stateMachine.CurrentPatrolPointIndex];
            _stateMachine.MoveTo(targetPoint);

            // Check if reached current patrol point
            if (Vector3.Distance(_stateMachine.transform.position, targetPoint) < _stateMachine.PatrolPointReachedThreshold)
            {
                _stateMachine.CurrentPatrolPointIndex = (_stateMachine.CurrentPatrolPointIndex + 1) % _stateMachine.PatrolPoints.Count;
            }
        }

        public override void Exit()
        {
            // Clean up if necessary
        }
    }
}