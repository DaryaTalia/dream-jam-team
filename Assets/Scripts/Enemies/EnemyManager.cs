using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private int enemyType;

    public int health; // made public to see for testing
    [SerializeField] private int healthSpeed;
    [SerializeField] private int healthAOE;
    [SerializeField] private int healthImob;
    [SerializeField] private int healthDmgDlr;
    public bool isDead;
    public bool pauseMovement;

    private bool immuneAggro; // Set Immobilizer Immune to Aggro

    [SerializeField] private float enemyRange;
    public float atkCooldownMax;
    public float atkCooldown;
    public bool canAttack;

    [SerializeField] private int damage;

    [SerializeField] private float moveSpeed;

    [SerializeField] private GameObject playerTarget;
    [SerializeField] private LayerMask playerMask;

    private void Start()
    {
        Init();

        //playerTarget = GameObject.FindGameObjectWithTag("Player");
    }

    void Init() // This should be called when the enemy gets reused
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player");
        isDead = false;
        pauseMovement = false;
        canAttack = true;

        switch (enemyType)
        {
            case 0: // Speed Enemy
                health = healthSpeed;
                break;
            case 1: // Enemy AOE
                health = healthAOE;
                break;
            case 2: // Enemy Imot
                health = healthImob;
                immuneAggro = true;
                break;
            case 3: // Enemy DmgDlr
                health = healthDmgDlr;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!pauseMovement)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerTarget.transform.position, moveSpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, 1.9f, transform.position.z); // This just sets the height right but should be fixed later
        }

        if (!isDead)
        {
            AttackPlayer();
        }
        
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;

        if(health <= 0)
        {
            //Debug.Log("Enemy Dead");
            isDead = true;
            pauseMovement = true;

            transform.position = new Vector3(0, -10f, 0); // Hides enemy under the map so it can be reused later
        }
    }

    void AttackPlayer()
    {
        if (atkCooldown <= 0)
        {
            if (Vector3.Distance(transform.position, playerTarget.transform.position) <= enemyRange * .75f)
            {
                // This is the start of the windup for attack

                pauseMovement = true;
                atkCooldown = atkCooldownMax; // Sets timer Cooldown to provide time for Atk Anim and pauses between attacks
                canAttack = true;
            }
        }

        if (atkCooldown <= atkCooldownMax/2 && atkCooldown > 0 && canAttack) // This pauses movement the enemy for half the cooldown timer for Atk Anim
        {
            pauseMovement = false;
            // This is where enemy will swing and try to dmg player

            Vector3 pos = playerTarget.transform.position;
            Vector3 dir = (pos - this.transform.position).normalized; // Directional Vector towards player

            Collider[] PlayerInRange = Physics.OverlapSphere(this.transform.position + (dir * enemyRange / 2), enemyRange / 2, playerMask);
            if(PlayerInRange.Length > 0)
            {
                //Debug.Log(PlayerInRange[0] + " Hit by attack");
                PlayerInRange[0].GetComponent<PlayerStats>().TakeDamage(1);
            }

            canAttack = false;
        }

        if (atkCooldown > 0)
        {
            atkCooldown -= Time.deltaTime;
        }
    }
}
