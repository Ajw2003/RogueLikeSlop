using UnityEngine;
using UnityEngine.AI;

namespace StateMachine.States
{
    public class MonsterPickedUpState : MonsterState
    {
        private NavMeshAgent _agent;
        private Rigidbody _rb;

        public MonsterPickedUpState(MonsterStateMachine stateMachine) : base(stateMachine)
        {
            _agent = stateMachine.GetComponent<NavMeshAgent>();
            _rb = stateMachine.GetComponent<Rigidbody>();
        }

        public override void Enter()
        {
            base.Enter();
            
            // Deactivate the Monster logic
            _stateMachine.Deactivate();
            
            // Disable navigation
            if (_agent != null)
            {
                _agent.enabled = false;
            }

            // Ensure physics settings are correct for being held
            if (_rb != null)
            {
                _rb.useGravity = false;
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }

            // Start the struggle timer coroutine on the state machine
            _stateMachine.StartStruggling();
        }

        public override void Exit()
        {
            base.Exit();
            // Stop the struggle timer if we exit for any other reason (like being thrown)
            _stateMachine.StopStruggling();
        }

        public override void Update()
        {
            // Do nothing while picked up, Item.cs handles the movement
        }
    }
}