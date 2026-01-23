using UnityEngine;

namespace StateMachine.States
{
    public class MonsterIdleState : MonsterState
    {
        public MonsterIdleState(MonsterStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            _stateMachine.StopMoving();
        }

        public override void Update()
        {
            // Even when idling, check if the player can be seen
            if (_stateMachine.CanSeePlayer())
            {
                _stateMachine.ChangeState(_stateMachine.PursueState);
                return;
            }

            // If patrol points are added or becomes available, go back to patrol
            if (_stateMachine.PatrolPoints.Count > 0)
            {
                _stateMachine.ChangeState(_stateMachine.PatrolState);
            }
        }

        public override void Exit()
        {
        }
    }
}