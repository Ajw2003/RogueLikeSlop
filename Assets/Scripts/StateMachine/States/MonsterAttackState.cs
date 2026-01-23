using UnityEngine;
using Interfaces;

namespace StateMachine.States
{
    public class MonsterAttackState : MonsterState
    {
        private float _lastAttackTime;

        public MonsterAttackState(MonsterStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            _stateMachine.StopMoving();
            _lastAttackTime = -_stateMachine.AttackRate; // Allow immediate first attack
        }

        public override void Update()
        {
            if (_stateMachine.PlayerTarget == null)
            {
                _stateMachine.ChangeState(_stateMachine.PatrolState);
                return;
            }

            // Look at player
            Vector3 lookPos = _stateMachine.PlayerTarget.position;
            lookPos.y = _stateMachine.transform.position.y;
            _stateMachine.transform.LookAt(lookPos);

            float distance = Vector3.Distance(_stateMachine.transform.position, _stateMachine.PlayerTarget.position);

            if (distance > _stateMachine.AttackRange)
            {
                _stateMachine.ChangeState(_stateMachine.PursueState);
                return;
            }

            if (Time.time >= _lastAttackTime + _stateMachine.AttackRate)
            {
                Attack();
                _lastAttackTime = Time.time;
            }
        }

        private void Attack()
        {
            PlayerStateMachine stateMachine = _stateMachine.PlayerTarget.GetComponent<PlayerStateMachine>();
            stateMachine.TakeDamage(_stateMachine.AttackDamage);
        }
    }
}
