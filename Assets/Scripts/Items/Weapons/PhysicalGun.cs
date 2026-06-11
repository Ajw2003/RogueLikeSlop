using UnityEngine;

public class PhysicalGun : MonoBehaviour
{
    public enum BoltState { Locked, Unlocked, Open }

    [Header("References")]
    [Tooltip("The moving part of the gun.")]
    public Transform boltTransform;
    [Tooltip("Where the bullet is instantiated.")]
    public Transform firePoint;
    [Tooltip("The stats for this weapon.")]
    public SpellStats stats;

    [Header("Bolt Settings")]
    public float boltTravelDistance = 0.1f;
    public float boltUnlockRotation = 45f;
    public float flickThreshold = 2f; 
    public float pushPullSensitivity = 0.01f;

    [Header("State")]
    [SerializeField] private BoltState _boltState = BoltState.Locked;
    [SerializeField] private bool _hasRoundInChamber = false;
    [SerializeField] private bool _hasSpentShell = false;

    public bool IsReloading => Input.GetKey(KeyCode.R);

    private float _currentBoltZ = 0f;
    private float _currentBoltRotation = 0f;
    private Vector3 _boltInitialLocalPos;
    private Quaternion _boltInitialLocalRot;

    private void Start()
    {
        if (boltTransform != null)
        {
            _boltInitialLocalPos = boltTransform.localPosition;
            _boltInitialLocalRot = boltTransform.localRotation;
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            Reload();
        }
        else if (Input.GetMouseButtonDown(0)) // Fire1
        {
            Fire();
        }
        
        UpdateBoltVisuals();
    }

    private void Reload()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        switch (_boltState)
        {
            case BoltState.Locked:
                // flick mouse right to unlock bolt
                if (mouseX > flickThreshold)
                {
                    _boltState = BoltState.Unlocked;
                    _currentBoltRotation = boltUnlockRotation;
                    Debug.Log("Bolt Unlocked");
                }
                break;

            case BoltState.Unlocked:
                // flick left to lock bolt (only if bolt is forward)
                if (mouseX < -flickThreshold && _currentBoltZ <= 0.01f)
                {
                    _boltState = BoltState.Locked;
                    _currentBoltRotation = 0f;
                    Debug.Log("Bolt Locked");
                }
                // pull mouse back to move bolt to open position
                else
                {
                    _currentBoltZ += -mouseY * pushPullSensitivity; // Pulling back (negative Y) increases Z offset
                    _currentBoltZ = Mathf.Clamp(_currentBoltZ, 0, boltTravelDistance);

                    if (_currentBoltZ >= boltTravelDistance && _boltState != BoltState.Open)
                    {
                        _boltState = BoltState.Open;
                        Debug.Log("Bolt Open");
                        if (_hasSpentShell)
                        {
                            _hasSpentShell = false;
                            Debug.Log("Spent shell ejected!");
                        }
                    }
                    else if (_currentBoltZ < boltTravelDistance && _boltState == BoltState.Open)
                    {
                        // Transitioning back from Open to Unlocked as we push forward
                        _boltState = BoltState.Unlocked;
                    }
                }
                break;

            case BoltState.Open:
                // click left mouse to load bullet into breach
                if (Input.GetMouseButtonDown(0))
                {
                    if (!_hasRoundInChamber)
                    {
                        _hasRoundInChamber = true;
                        Debug.Log("Round loaded into breach.");
                    }
                }

                // push mouse forward to chamber round
                _currentBoltZ += -mouseY * pushPullSensitivity; 
                _currentBoltZ = Mathf.Clamp(_currentBoltZ, 0, boltTravelDistance);

                if (_currentBoltZ <= 0.01f)
                {
                    _boltState = BoltState.Unlocked;
                    Debug.Log("Bolt Pushed Forward");
                }
                break;
        }
    }

    private void Fire()
    {
        // check if bolt is locked
        if (_boltState != BoltState.Locked)
        {
            Debug.Log("Cannot fire: Bolt not locked.");
            return;
        }

        // check if round loaded
        if (!_hasRoundInChamber)
        {
            // if not loaded play click sound to illustrate not being loaded
            Debug.Log("Click! No round loaded.");
            return;
        }

        // if loaded consume 1 round of ammo
        _hasRoundInChamber = false;
        _hasSpentShell = true;

        // instantiate bullet at gun barrel/fire point 
        if (stats != null && stats.projectilePrefab != null)
        {
            GameObject projectile = Instantiate(stats.projectilePrefab, firePoint.position, firePoint.rotation);
            
            // shoot towards where aiming with mouse 
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(firePoint.forward * stats.projectileForce, ForceMode.Impulse);
            }
            
            Debug.Log("Fired!");
        }
        else
        {
            Debug.LogWarning("No projectile prefab or stats assigned to PhysicalGun.");
        }
    }

    private void UpdateBoltVisuals()
    {
        if (boltTransform == null) return;

        // Apply rotation for unlocking (around local Z or Y depending on model, let's assume Z for rotation)
        boltTransform.localRotation = _boltInitialLocalRot * Quaternion.Euler(0, 0, _currentBoltRotation);
        
        // Apply position for pulling back (Z-axis in local space)
        boltTransform.localPosition = _boltInitialLocalPos + new Vector3(0, 0, -_currentBoltZ);
    }
}

