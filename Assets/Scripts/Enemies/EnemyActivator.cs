using StateMachine;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
public class EnemyActivator : MonoBehaviour
{
    [Tooltip("The State Machine to activate.")]
    public MonsterStateMachine monsterStateMachine;

    [Tooltip("Distance at which the enemy activates.")]
    public float activationRadius = 20f;

    [Tooltip("Distance at which the enemy deactivates.")]
    public float deactivationRadius = 30f;

    private SphereCollider _activationTrigger;
    private Transform _playerTransform;
    private bool _isActive = false;

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

    private IEnumerator WatchPlayerDistance()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        while (_isActive && _playerTransform != null)
        {
            yield return wait;

            float distance = Vector3.Distance(transform.position, _playerTransform.position);
            
            if (distance > deactivationRadius)
            {
                DeactivateEnemy();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isActive || monsterStateMachine == null) return;

        if (other.CompareTag("Player") || other.GetComponent<PlayerStateMachine>() != null || other.GetComponentInParent<PlayerStateMachine>() != null)
        {
            _playerTransform = other.transform;
            ActivateEnemy();
        }
    }

    private void ActivateEnemy()
    {
        _isActive = true;
        monsterStateMachine.Activate();
        _activationTrigger.enabled = false; // Disable trigger to stop checks
        StartCoroutine(WatchPlayerDistance());
    }

    private void DeactivateEnemy()
    {
        _isActive = false;
        monsterStateMachine.Deactivate();
        _activationTrigger.enabled = true; // Re-enable trigger
        _playerTransform = null;
    }

    // Helper to visualize the radius in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, activationRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, deactivationRadius);
    }
}
