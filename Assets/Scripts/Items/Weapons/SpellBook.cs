using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SpellBook : MonoBehaviour
{

    private float _fireRate;
    private float _minSpread;
    private float _maxSpread;
    private float _reloadTime;
    private float _projectileForce;
    
    private int _damage;
    private int _numberOfProjectiles;

    private bool _isHoming;
    private bool _isReloading;
    
    private Collider[] _playerColliders;
    
    private GameObject _projectilePrefab;
    
    [SerializeField] private SpellStats spellStats;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform shootPoint;
    
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AssignStats();
        _playerColliders = gameObject.GetComponentsInChildren<Collider>();
    }

    private void AssignStats()
    {
        _fireRate = spellStats.fireRate;
        _minSpread = spellStats.minSpread;
        _maxSpread = spellStats.maxSpread;
        _reloadTime = spellStats.reloadTime;
        _projectileForce = spellStats.projectileForce;
        _damage = spellStats.damage;
        _numberOfProjectiles = spellStats.numberOfProjectiles;
        _projectilePrefab = spellStats.projectilePrefab;
        _isHoming = spellStats.isHoming;
        _projectilePrefab.GetComponent<NetworkedProjectile>().lifeTime = spellStats.lifeTime;
        
    }
    
    

    public void CastSpell()
    {
        if(_isReloading) return;
        else
        {
            _isReloading = true;
        }
        StartCoroutine(ReloadRoutine());
        Ray ray =playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));// ray cast from screen center for aiming and shooting, this makes it so the crosshair is always where the arrow will go
        Vector3 targetPoint;// make temp value 
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))// raycast with max distance of 100m from player
        {
            targetPoint = hit.point; // Hit an object or wall 
        }
        else
        {
            targetPoint = ray.GetPoint(100f); // Hit nothing, aim at a point 100m away
        }
            
        //calculate the direction
        Vector3 shootDirection = (targetPoint - shootPoint.position).normalized;// make shoot direction use point from crosshair/ screen center

        if (_numberOfProjectiles > 1)
        {
            for (int i = 0; i < _numberOfProjectiles; i++)
            {
                var projectilePrefabInstance =  Instantiate(_projectilePrefab, shootPoint.position, Quaternion.LookRotation(shootDirection));
                var tempCollider =  projectilePrefabInstance.gameObject.GetComponent<Collider>();
                foreach (var playerCollider in _playerColliders)
                {
                    Physics.IgnoreCollision(tempCollider, playerCollider );
                }
        
                var tempRB = projectilePrefabInstance.gameObject.GetComponent<Rigidbody>();
                tempRB.AddForce(_projectileForce * shootDirection, ForceMode.Impulse);
            }
            
        }
        else
        {
            var projectilePrefabInstance =  Instantiate(_projectilePrefab, shootPoint.position, Quaternion.LookRotation(shootDirection));
            var tempCollider =  projectilePrefabInstance.gameObject.GetComponent<Collider>();
            foreach (var playerCollider in _playerColliders)
            {
                Physics.IgnoreCollision(tempCollider, playerCollider );
            }
        
            var tempRB = projectilePrefabInstance.gameObject.GetComponent<Rigidbody>();
            tempRB.AddForce(_projectileForce * shootDirection, ForceMode.Impulse);
        }

    }

    private IEnumerator ReloadRoutine()
    {
        yield return new WaitForSeconds(_reloadTime);
        _isReloading = false;   
        
    }
}
