using UnityEngine;
using UnityEngine.AI;

namespace StateMachine.States
{
    public class MonsterPickedUpState : MonsterState
    {
        private NavMeshAgent _agent;
        private Rigidbody _rb;
        private Item _item;
        private float _escapeTimer;
        private PlayerStateMachine _player;
        private float _damageTickTimer;

        public MonsterPickedUpState(MonsterStateMachine stateMachine) : base(stateMachine)
        {
            _agent = stateMachine.GetComponent<NavMeshAgent>();
            _rb = stateMachine.GetComponent<Rigidbody>();
            _item = stateMachine.GetComponent<Item>();
        }

        public override void Enter()
        {
            base.Enter();
            
            // Find player for damage stats
            if (_player == null)
            {
                _player = Object.FindFirstObjectByType<PlayerStateMachine>();
            }

            // Ensure we have the item reference
            if (_item == null) _item = _stateMachine.GetComponent<Item>();

            // Stop movement instead of calling Deactivate() (which would change state to Idle)
            _stateMachine.StopMoving();
            
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
            
            // Set a random escape timer (kept for internal state logic if needed, 
            // though the coroutine in StateMachine now handles the actual release)
            _escapeTimer = Random.Range(_stateMachine.MinEscapeTime, _stateMachine.MaxEscapeTime);
        }

        public override void Exit()
        {
            base.Exit();
            // Stop the struggle timer if we exit for any other reason (like being thrown)
            _stateMachine.StopStruggling();
        }

        public override void Update()
        {
            HandleChokeDamage();
        }

        private void HandleChokeDamage()
        {
            // Only apply damage while actually being dragged/held by the player
            if (_item == null || !_item.IsDragging) return;
            if (_player == null) return;

            _damageTickTimer += Time.deltaTime;
            if (_damageTickTimer >= 1.0f) // Apply damage once per second
            {
                _stateMachine.TakeDamage(_player.ChokeDamage);
                _damageTickTimer = 0f;
                Debug.Log($"{_stateMachine.gameObject.name} taking choke damage: {_player.ChokeDamage}");
            }
        }
    }
}
