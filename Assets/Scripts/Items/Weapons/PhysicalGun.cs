using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

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
    [SerializeField] private Camera playerCamera;

    [Header("Bolt Settings")]
    public float boltTravelDistance = 0.1f;
    public float boltUnlockRotation = -90f;
    public float flickThreshold = 2f; 
    public float pushPullSensitivity = 0.01f;

    [Header("State")]
    [SerializeField] private BoltState _boltState = BoltState.Locked;
    [SerializeField] private bool _hasRoundInChamber = false;
    [SerializeField] private bool _hasSpentShell = false;
    [SerializeField] private Transform shellSpawn;
    [SerializeField] private GameObject shellPrefab;
    private GameObject _shell;

    private Collider[] _playerColliders;

    public bool IsReloading => Input.GetKey(KeyCode.R);

    private float _currentBoltZ = 0f;
    private float _currentBoltRotation = 0f;
    private Vector3 _boltInitialLocalPos;
    private Quaternion _boltInitialLocalRot;
    
    private int currentAmmoInMag = 0;
    private bool roundInChamber = false;  
    private bool loading = false;
    
    [SerializeField] private int reservedAmmoInMag = 24;

    public int magCapacity;
    
    

    private void Start()
    {
        if (boltTransform != null)
        {
            _boltInitialLocalPos = boltTransform.localPosition;
            _boltInitialLocalRot = boltTransform.localRotation;
        }

        _playerColliders = transform.root.GetComponentsInChildren<Collider>();
        if (playerCamera == null) playerCamera = Camera.main;
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
        //bolt at -20y
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
                    //bolt at -90y
                    Debug.Log("Bolt Unlocked");
                }
                break;

            case BoltState.Unlocked:
                // flick left to lock bolt (only if bolt is forward)
                if (mouseX < -flickThreshold && _currentBoltZ <= 0.01f)
                {
                    _boltState = BoltState.Locked;
                    _currentBoltRotation = -20f;
                    //bolt at -20y
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
                    if (currentAmmoInMag < magCapacity && reservedAmmoInMag > 0 && !loading)
                    {
                        Debug.Log("Round loaded into breach.");
                        currentAmmoInMag++;
                        reservedAmmoInMag--;
                        loading = true;
                        // Use world position and rotation for instantiation, then parent it
                        _shell = Instantiate(shellPrefab, shellSpawn.position, shellSpawn.rotation, shellSpawn);
                        StartCoroutine(LoadAmmoInMag());
                    }
                }

                // push mouse forward to chamber round
                _currentBoltZ += -mouseY * pushPullSensitivity; 
                _currentBoltZ = Mathf.Clamp(_currentBoltZ, 0, boltTravelDistance);

                if (_currentBoltZ <= 0.01f)
                {
                    _boltState = BoltState.Unlocked;
                    _hasRoundInChamber = true;
                    Debug.Log("Bolt Pushed Forward");
                }
                break;
        }
    }

    private IEnumerator LoadAmmoInMag()
    {
        yield return new WaitForSeconds(0.1f);
        
        // Define targets in local space relative to shellSpawn
        Vector3 target1 = new Vector3(-1f, 0, 0); 
        Vector3 target2 = new Vector3(-1.05f, -0.2f, -0.25f);
        float speed = 2f;

        // Move to first local point
        while (Vector3.Distance(_shell.transform.localPosition, target1) > 0.001f)
        {
            _shell.transform.localPosition = Vector3.MoveTowards(_shell.transform.localPosition, target1, speed * Time.deltaTime);
            yield return null; // This prevents the infinite loop/freeze
        }

        // Move to second local point
        while (Vector3.Distance(_shell.transform.localPosition, target2) > 0.001f)
        {
            _shell.transform.localPosition = Vector3.MoveTowards(_shell.transform.localPosition, target2, speed * Time.deltaTime);
            yield return null; // This prevents the infinite loop/freeze
        }

        loading = false;
        // The bullet is now "loaded", we can hide it or destroy it
        Destroy(_shell); 
    }

    private void Fire()
    {
        // check if round loaded
        if (currentAmmoInMag <= 0 || !_hasRoundInChamber || _boltState != BoltState.Locked)
        {
            // if not loaded play click sound to illustrate not being loaded
            Debug.Log("Click! No round loaded.");
            return;
        }

        // if loaded consume 1 round of ammo
        _hasRoundInChamber = false;
        _hasSpentShell = true;
        currentAmmoInMag--;

        // instantiate bullet at gun barrel/fire point 
        if (stats != null && stats.projectilePrefab != null)
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            Vector3 targetPoint;
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.GetPoint(100f);
            }

            Vector3 shootDirection = (targetPoint - firePoint.position).normalized;
            float currentSpread = Random.Range(stats.minSpread, stats.maxSpread);
            Vector3 spreadOffset = Random.insideUnitSphere * currentSpread;
            shootDirection = (shootDirection + spreadOffset).normalized;

            var projectileInstance = Instantiate(stats.projectilePrefab, firePoint.position, Quaternion.LookRotation(shootDirection));
            
            // Set stats on projectile (mirroring SpellBook)
            projectileInstance.transform.localScale = new Vector3(stats.projectileSize, stats.projectileSize, stats.projectileSize);
            if (projectileInstance.TryGetComponent<NetworkedProjectile>(out var netProj))
            {
                netProj.lifeTime = stats.lifeTime;
                netProj.Damage = stats.damage;
            }

            // Ignore collisions with player
            var projCollider = projectileInstance.GetComponent<Collider>();
            if (projCollider != null)
            {
                foreach (var playerCollider in _playerColliders)
                {
                    if (playerCollider != null) Physics.IgnoreCollision(projCollider, playerCollider);
                }
            }

            // Apply force
            var rb = projectileInstance.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.AddForce(shootDirection * stats.projectileForce, ForceMode.Impulse);
                Debug.Log($"Fired! Force: {stats.projectileForce}");
            }
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
        boltTransform.localRotation = _boltInitialLocalRot * Quaternion.Euler(0, -_currentBoltRotation, 0);
        
        // Apply position for pulling back (Z-axis in local space)
        boltTransform.localPosition = _boltInitialLocalPos + new Vector3(0, -_currentBoltZ, 0);
    }
}

