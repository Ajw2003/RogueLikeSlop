using UnityEngine;
using Interfaces;

namespace StateMachine.States
{
    public class PlayerAttackState : PlayerState
    {
        public PlayerAttackState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            //Disable all other input than pause 
            
            //run the attack logic
            if (_stateMachine.AttackPoint == null)
            {
                Debug.LogWarning("AttackPoint not set on PlayerStateMachine!");
                return;
            }

            Collider[] hitEnemies = Physics.OverlapSphere(_stateMachine.AttackPoint.position, _stateMachine.AttackRange, _stateMachine.EnemyLayers);

            foreach (Collider enemy in hitEnemies)
            {
                // Avoid hitting ourselves if the layer mask is not set up correctly
                if (enemy.gameObject == _stateMachine.gameObject) continue;

                IDamageable damageable = enemy.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(_stateMachine.AttackDamage);
                    Debug.Log($"Hit {enemy.name} for {_stateMachine.AttackDamage} damage.");
                }
            }
        }

        public override void Exit()
        {
            //re enable input for dodge
            //re enable input for attack
            // re enable input for walk
            //re enable input for run
            
            //don't force state transfer just allow it to happen
        }
    }
}
