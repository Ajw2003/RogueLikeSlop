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
    [SerializeField] private float _damageCooldown = 0.5f;

    private float _lastDamageTime;
    
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
        // 1. Safety Checks
        if (_isDragging) return;
        if (Time.time < _lastDamageTime + _damageCooldown) return;

        // 2. Velocity Calculation
        float impactVelocity = collision.relativeVelocity.magnitude;
        float myVelocity = _rb.linearVelocity.magnitude;

        // 3. Damage Application
        int damage = Mathf.RoundToInt(impactVelocity * _damageMultiplier);
        bool dealtDamage = false;

        // Damage target
        if (collision.gameObject.TryGetComponent(out MonsterStateMachine targetMonster))
        {
            float currentHealthBefore = targetMonster.CurrentHealth;
            targetMonster.TakeDamage(damage, impactVelocity);
            if (targetMonster.CurrentHealth < currentHealthBefore) dealtDamage = true;
        }
        else if (collision.gameObject.TryGetComponent(out StateMachine.PlayerStateMachine player))
        {
            float currentHealthBefore = player.CurrentHealth;
            player.TakeDamage(damage, impactVelocity);
            if (player.CurrentHealth < currentHealthBefore) dealtDamage = true;
        }

        // Damage ourselves (if we are an enemy item being thrown)
        if (_monsterAI != null)
        {
            // Only take impact damage if we are NOT grounded/active
            if (!_agent.enabled || myVelocity > 1f) // Small threshold for "moving"
            {
                float currentHealthBefore = _monsterAI.CurrentHealth;
                _monsterAI.TakeDamage(damage, impactVelocity);
                if (_monsterAI.CurrentHealth < currentHealthBefore) dealtDamage = true;
            }
        }

        if (dealtDamage)
        {
            _lastDamageTime = Time.time;
            Debug.Log($"Impact Damage Dealt: {damage} (Impact: {impactVelocity:F1})");
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
