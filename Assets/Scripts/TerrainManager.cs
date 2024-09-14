using System;
using UnityEngine;
using UnityEngine.Pool;

public class TerrainManager : MonoBehaviour
{
    [SerializeField] Transform objectToTrack;
    [SerializeField] GameObject terrainPiecePrefab;
    private ObjectPool<GameObject> terrainPool;
    private float lastSpawn;
    public int terrainPoolCount;
    public float spawnSpacing = 10f;
    public float spawnThresholdFromCenter = 5f;
    public float returnToPoolThreshold = 15f;

    private void Start()
    {
        terrainPool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
            OnDestroyPoolObject, true, 3, terrainPoolCount);
        PlaceNewPiece(transform.position);
    }

    private void Update()
    {
        var signedTrackedDiff = objectToTrack.position.x - lastSpawn;
        if (signedTrackedDiff > spawnThresholdFromCenter)
        {
            var newSpawn = lastSpawn + (spawnSpacing * Mathf.Sign(signedTrackedDiff));
            PlaceNewPiece(new Vector3(newSpawn, transform.position.y, transform.position.z));
        }
    }

    void PlaceNewPiece(Vector3 position)
    {
        var piece = terrainPool.Get();
        piece.transform.position = position;
        lastSpawn = position.x;
    }

    GameObject CreatePooledItem()
    {
        var go = Instantiate(terrainPiecePrefab, transform.position, Quaternion.identity);
        ReturnToPool rtp = go.AddComponent<ReturnToPool>();
        rtp.objectToTrack = objectToTrack;
        rtp.pool = terrainPool;
        rtp.distanceFromObjectThreshold = returnToPoolThreshold;
        return go;
    }
    
    // Called when an item is returned to the pool using Release
    void OnReturnedToPool(GameObject piece)
    {
        piece.SetActive(false);
    }

    // Called when an item is taken from the pool using Get
    void OnTakeFromPool(GameObject piece)
    {
        piece.SetActive(true);
    }

    // If the pool capacity is reached then any items returned will be destroyed.
    // We can control what the destroy behavior does, here we destroy the GameObject.
    void OnDestroyPoolObject(GameObject piece)
    {
        Destroy(piece);
    }
}