using System.Threading.Tasks;
using NUnit.Framework.Internal.Commands;
using UnityEngine;

namespace StateMachine.States
{
    public class PlayerAttackState : PlayerState
    {
        public PlayerAttackState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            Collider[] hitEnemies = Physics.OverlapSphere(_stateMachine.AttackPoint.position, _stateMachine.AttackRange, _stateMachine.EnemyLayers);

            foreach (Collider enemy in hitEnemies)
            {
                // Avoid hitting ourselves if the layer mask is not set up correctly
                if (enemy.gameObject == _stateMachine.gameObject) continue;
                
                enemy.GetComponent<MonsterStateMachine>().TakeDamage(_stateMachine.AttackDamage);
            }

            Task.Run(async () =>
            {
                await Task.Delay(100);
                Exit();
            });
        }

        public override void Exit()
        {
            //re enable input for dodge
            //re enable input for attack
            // re enable input for walk
            //re enable input for run
            
            //don't force state transfer just allow it to happen
            
            _stateMachine.ChangeState(_stateMachine.IdleState);
        }
    }
}
