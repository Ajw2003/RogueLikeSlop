using StateMachine;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour
{
    private Rigidbody _rb;
    private bool _isDragging = false;
    private Vector3 _targetPosition;
    private Quaternion _targetRotation = Quaternion.identity;
    
    // References for enemy handling
    private MonsterStateMachine _monsterAI;
    private NavMeshAgent _agent;

    [Header("Physics Settings")]
    [SerializeField] private float _followSpeed = 20f;
    [SerializeField] private float _rotationSpeed = 10f;

    [Header("Damage Settings")]
    [SerializeField] private float _damageMultiplier = 2f;
    [SerializeField] private float _minVelocityForDamage = 2f;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _monsterAI = GetComponent<MonsterStateMachine>();
        _agent = GetComponent<NavMeshAgent>();

        // Ensure the rigidbody is set up for physical dragging
        _rb.useGravity = true;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _targetRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        if (_isDragging)
        {
            // Move towards target position using physics
            Vector3 direction = _targetPosition - _rb.position;
            _rb.linearVelocity = direction * _followSpeed;
            
            // Slerp towards target rotation
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, _targetRotation, Time.fixedDeltaTime * _rotationSpeed));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        // Don't deal damage if we're currently being held/dragged
        if (_isDragging) return;

        float impactVelocity = collision.relativeVelocity.magnitude;

        if (impactVelocity >= _minVelocityForDamage)
        {
            // Calculate damage and convert to integer
            int damage = Mathf.RoundToInt(impactVelocity * _damageMultiplier);
            
            // 1. Damage the thing we hit (if it's a monster or player)
            if (collision.gameObject.TryGetComponent(out MonsterStateMachine targetMonster))
            {
                targetMonster.TakeDamage(damage);
                Debug.Log($"Item hit {collision.gameObject.name} for {damage} damage");
            }
            else if (collision.gameObject.TryGetComponent(out StateMachine.PlayerStateMachine player))
            {
                player.TakeDamage(damage);
                Debug.Log($"Item hit Player for {damage} damage");
            }

            // 2. Damage OURSELVES if we are an enemy being thrown
            if (_monsterAI != null)
            {
                _monsterAI.TakeDamage(damage);
                Debug.Log($"Enemy Item {gameObject.name} took {damage} impact damage");
                
                // If we are still alive and have settled, we could potentially re-activate
                // But for now, let's keep it simple: they stay deactivated until we decide otherwise
            }
        }
    }

    public void StartDragging()
    {
        _isDragging = true;
        _rb.useGravity = false;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _targetRotation = transform.rotation;

        // Transition to PickedUp state if we are an enemy
        if (_monsterAI != null)
        {
            _monsterAI.PickUp();
        }
    }

    public void StopDragging()
    {
        _isDragging = false;
        _rb.useGravity = true;

        // Release the AI from the PickedUp state
        if (_monsterAI != null)
        {
            _monsterAI.Release();
        }
    }

    public void Throw(Vector3 direction, float force)
    {
        _isDragging = false;
        _rb.useGravity = true;
        _rb.AddForce(direction * force, ForceMode.Impulse);

        // Release the AI from the PickedUp state
    }

    public void UpdateTargetPosition(Vector3 position)
    {
        _targetPosition = position;
    }

    public void UpdateRotation(Quaternion rotation)
    {
        _targetRotation = rotation;
    }

    public Quaternion TargetRotation => _targetRotation;
    public bool IsDragging => _isDragging;
}