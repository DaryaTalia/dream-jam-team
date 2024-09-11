using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    private int health;
    [SerializeField] private int healthMax;

    [SerializeField] private int damage;

    [SerializeField] private float moveSpeed;

    [SerializeField] private GameObject playerTarget;

    private void Start()
    {
        health = healthMax;
        playerTarget = GameObject.FindGameObjectWithTag("Player");
    }

    void Init() // This should be called when the enemy gets reused
    {
        health = healthMax;
        playerTarget = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, playerTarget.transform.position, moveSpeed * Time.deltaTime);
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;

        if(health <= 0)
        {
            Debug.Log("Enemy Dead");
        }
    }
}
