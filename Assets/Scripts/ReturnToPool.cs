using System;
using UnityEngine;
using UnityEngine.Pool;

public class ReturnToPool : MonoBehaviour
{
    public ObjectPool<GameObject> pool;
    public Transform objectToTrack;
    public float distanceFromObjectThreshold = 15f;

    private void Update()
    {
        if (Vector3.Distance(objectToTrack.position, transform.position) > distanceFromObjectThreshold)
        {
            pool.Release(gameObject);
        }
    }
}