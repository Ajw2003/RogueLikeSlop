using System.Collections;
using UnityEngine;

public class JumpPuzzleManager : MonoBehaviour
{
    public GameObject[] objects;
    public float[] delays;
    public float currentDelay;
    public bool isRunning = true;
    public float maxRange;
    public float minRange;
    public float overlapDuration = 0.5f;
    public int spacing = 4; // Add a new runner every 4 items

    private int[] activeRequests; // Reference counting for active objects
    
    // Spawns multiple runners to create an endless wave effect
    private IEnumerator MasterSequence()
    {
        // Calculate how many runners fit on the track
        int runnerCount = Mathf.Max(1, objects.Length / spacing);
        
        // Calculate the time delay required to space them out by 'spacing' indices
        // Since all runners follow the same path/delays, we just need to wait for the 
        // time it takes to traverse the first 'spacing' blocks.
        float startDelay = 0f;
        for (int i = 0; i < Mathf.Min(spacing, delays.Length); i++)
        {
            startDelay += delays[i];
        }

        for (int i = 0; i < runnerCount; i++)
        {
            StartCoroutine(SequenceRunner());
            yield return new WaitForSeconds(startDelay);
        }
    }

    private IEnumerator SequenceRunner()
    {
        while (isRunning)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                RequestEnable(i);
                currentDelay = delays[i];
                yield return new WaitForSeconds(currentDelay);
                
                // Pass the index to the disable routine
                StartCoroutine(DisableAfterDelay(i, overlapDuration));
            }
        }
    }

    private void RequestEnable(int index)
    {
        activeRequests[index]++;
        if (activeRequests[index] == 1)
        {
            objects[index].SetActive(true);
        }
    }

    private IEnumerator DisableAfterDelay(int index, float delay)
    {
        yield return new WaitForSeconds(delay);
        RequestDisable(index);
    }

    private void RequestDisable(int index)
    {
        activeRequests[index]--;
        if (activeRequests[index] <= 0)
        {
            activeRequests[index] = 0; // Safety clamp
            objects[index].SetActive(false);
        }
    }

    private void Start()
    {
        // --- Auto-Sort Objects by Proximity (Nearest Neighbor) ---
        if (objects.Length > 0)
        {
            System.Collections.Generic.List<GameObject> sortedList = new System.Collections.Generic.List<GameObject>();
            System.Collections.Generic.List<GameObject> remaining = new System.Collections.Generic.List<GameObject>(objects);

            // 1. Find the first object (closest to this Manager)
            GameObject current = GetClosest(transform.position, remaining);
            sortedList.Add(current);
            remaining.Remove(current);

            // 2. Find the rest of the chain
            while (remaining.Count > 0)
            {
                // Find the closest object to the LAST added object
                current = GetClosest(current.transform.position, remaining);
                sortedList.Add(current);
                remaining.Remove(current);
            }

            objects = sortedList.ToArray();
        }
        // ---------------------------------------------------------
        
        delays = new float[objects.Length];
        activeRequests = new int[objects.Length]; // Initialize reference counter

        for (int i = 0; i < objects.Length; i++)
        {
            float currentRandom = Random.Range(minRange, maxRange);
            delays[i] = currentRandom;
            activeRequests[i] = 0; 
        }

        foreach (GameObject obj in objects)
        {
            obj.SetActive(false);
        }

        StartCoroutine(MasterSequence());
    }

    private GameObject GetClosest(Vector3 referencePos, System.Collections.Generic.List<GameObject> candidates)
    {
        GameObject bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach (GameObject candidate in candidates)
        {
            Vector3 directionToTarget = candidate.transform.position - referencePos;
            float dSqrToTarget = directionToTarget.sqrMagnitude;

            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = candidate;
            }
        }

        return bestTarget;
    }
}
