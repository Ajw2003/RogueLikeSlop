using StateMachine;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class EnemyActivator : MonoBehaviour
{
    [Tooltip("The State Machine to activate.")]
    public MonsterStateMachine monsterStateMachine;

    [Tooltip("Distance at which the enemy activates.")]
    public float activationRadius = 20f;

    private SphereCollider _activationTrigger;

    private void Awake()
    {
        _activationTrigger = GetComponent<SphereCollider>();
        _activationTrigger.isTrigger = true;
        _activationTrigger.radius = activationRadius;

        // Try to find the state machine on the parent if not assigned
        if (monsterStateMachine == null)
        {
            monsterStateMachine = GetComponentInParent<MonsterStateMachine>();
        }

        if (monsterStateMachine == null)
        {
            Debug.LogError($"EnemyActivator on {gameObject.name} could not find a MonsterStateMachine!");
            enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (monsterStateMachine == null) return;

        // Check if the object entering is the player
        // Assuming the player has a tag "Player" or a specific component.
        // Based on previous file reads, Player has 'PlayerStateMachine'.
        if (other.CompareTag("Player") || other.GetComponent<PlayerStateMachine>() != null || other.GetComponentInParent<PlayerStateMachine>() != null)
        {
            monsterStateMachine.Activate();
            
            // Disable this object/collider so it doesn't trigger again
            gameObject.SetActive(false);
        }
    }

    // Helper to visualize the radius in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, activationRadius);
    }
}
