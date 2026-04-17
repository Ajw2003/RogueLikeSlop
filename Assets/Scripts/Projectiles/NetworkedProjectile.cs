using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkedProjectile : NetworkBehaviour
{
    public float lifeTime = 3f;
    private void OnCollisionEnter(Collision other)
    {
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