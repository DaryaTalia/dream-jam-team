using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    #region Variable
    [SerializeField] GameObject[] enemyArray;
    public float spawnATimer;


    #endregion

    // Start is called before the first frame update
    void Start()
    {
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
            SpawnEnemies();
            spawnATimer = 5f;
        }

    }

    void SpawnEnemies()
    {

        Vector3 loc = getRange();
        Instantiate(enemyArray[0], loc, Quaternion.identity /*Quaternion.Euler(0, 180, 0)*/);

    }


    private Vector3 getRange()
    {
        return new Vector3(UnityEngine.Random.Range(transform.position.x - (transform.localScale.x / 2), transform.position.x + (transform.localScale.x / 2)), 0, transform.position.z);
        // Local X scale = 100
    }
}
