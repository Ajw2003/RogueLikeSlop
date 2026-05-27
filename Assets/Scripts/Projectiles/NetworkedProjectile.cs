using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkedProjectile : NetworkBehaviour
{
    public float lifeTime = 3f;

    public int Damage;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<MonsterStateMachine>())
        {
            other.gameObject.GetComponent<MonsterStateMachine>().TakeDamage(Damage);
        }
        DestroySelf();
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(DelayedDestroy());
    }

    IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}