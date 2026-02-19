using UnityEngine;
using UnityEngine.AI;

namespace StateMachine.States
{
    public class MonsterIdleState : MonsterState
    {
        private float _visionTimer;
        private NavMeshAgent _agent;
        private Rigidbody _rb;

        public MonsterIdleState(MonsterStateMachine stateMachine) : base(stateMachine)
        {
            _agent = stateMachine.GetComponent<NavMeshAgent>();
            _rb = stateMachine.GetComponent<Rigidbody>();
        }

        public override void Enter()
        {
            _stateMachine.StopMoving();
            _visionTimer = 0f;
        }

        public override void Update()
        {
            // Re-activation logic for when coming from PickedUp or falling
            if (_agent != null )
            {
                // Wait until the monster is grounded and moving slowly
                if (_rb != null && _rb.linearVelocity.magnitude < 0.15f)
                {
                    Debug.Log($"{_stateMachine.gameObject.name} re-activating AI after landing.");
                    _agent.enabled = true;
                        
                    // We need to make sure the state machine's internal _isActive is true
                    // so it can continue with normal AI logic
                    _stateMachine.Activate(); 
                }
                
                // If the agent is still disabled (mid-air or mid-struggle), don't perform other logic
            }

            // Normal Idle Logic (Only runs if agent.enabled is true)
            _visionTimer += Time.deltaTime;
            if (_visionTimer < 0.2f) return;
            _visionTimer = 0f;

            // Check if the player can be seen
            if (_stateMachine.CanSeePlayer())
            {
                _stateMachine.ChangeState(_stateMachine.PursueState);
                return;
            }

            // If patrol points are available, go back to patrol
            if (_stateMachine.PatrolPoints.Count > 0)
            {
                _stateMachine.ChangeState(_stateMachine.PatrolState);
                return;
            }
        }

        public override void Exit()
        {
        }
    }
}