using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class SpawnManager : MonoBehaviour
{
    #region Variable
    [SerializeField] GameObject[] enemyArray;
    public float spawnATimer;
    private ObjectPool<EnemyManager> enemyPool;
    [SerializeField] int defaultEnemyPoolSize = 10;
    [SerializeField] int maxEnemyPoolSize = 30;
    


    #endregion

    // Start is called before the first frame update
    void Start()
    {
        enemyPool = new ObjectPool<EnemyManager>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
            OnDestroyPoolObject, true, defaultEnemyPoolSize, maxEnemyPoolSize);
        spawnATimer = 5f;
    }

    // Update is called once per frame
    void Update()
    {

        //Debug.Log(getRange());


        if(spawnATimer >= 0)
        {
            spawnATimer -= Time.deltaTime;
        }

        if(spawnATimer < 0)
        {
            EnemyManager enemy = enemyPool.Get();
            spawnATimer = 5f;
        }

    }

    public void Reset()
    {
        foreach(EnemyManager em in this.GetComponentsInChildren<EnemyManager>())
        {
            Destroy(em.gameObject);
        }

        enemyPool = new ObjectPool<EnemyManager>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
            OnDestroyPoolObject, true, defaultEnemyPoolSize, maxEnemyPoolSize);
        spawnATimer = 5f;
    }

    private int enemySpawnID()
    {
        int enemyID = Random.Range(0, 100);

        if (enemyID < 50)
        {
            // Set to spawn Speed -- 50% chance
            enemyID = 0;
        }
        else if (enemyID < 80)
        {
            // Set to spawn Drone -- 30% chance
            enemyID = 1;
        }
        else
        {
            // Set to spawn Elite -- 20% chance
            enemyID = 2;
        }
        
        
        //int enemyID = Random.Range(0, enemyArray.Length);

        return enemyID;
    }

    private Vector3 getRange()
    {
        return new Vector3(UnityEngine.Random.Range(transform.position.x - (transform.localScale.x / 2), transform.position.x + (transform.localScale.x / 2)), 0, transform.position.z);
        // Local X scale = 100
    }
    
    EnemyManager CreatePooledItem()
    {
        Vector3 loc = getRange();
        var go = Instantiate(enemyArray[enemySpawnID()], loc, Quaternion.identity /*Quaternion.Euler(0, 180, 0)*/);
        go.transform.SetParent(this.transform);
        EnemyManager enemy = go.GetComponent<EnemyManager>();
        enemy.pool = enemyPool;
        return enemy;
    }
    
    // Called when an item is returned to the pool using Release
    void OnReturnedToPool(EnemyManager enemy)
    {
        enemy.gameObject.SetActive(false);
    }

    // Called when an item is taken from the pool using Get
    void OnTakeFromPool(EnemyManager enemy)
    {
        enemy.Init();
        enemy.transform.position = getRange();
        enemy.gameObject.SetActive(true);
    }

    // If the pool capacity is reached then any items returned will be destroyed.
    // We can control what the destroy behavior does, here we destroy the GameObject.
    void OnDestroyPoolObject(EnemyManager enemy)
    {
        Destroy(enemy);
    }
}
