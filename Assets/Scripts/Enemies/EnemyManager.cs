using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    #region
    [SerializeField] private int enemyType; // 0 speed / 1 drone / 2 Imob / 3 DmgDlr

    public int health; // made public to see for testing
    [SerializeField] private int healthSpeed;
    [SerializeField] private int healthDrone;
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

    [SerializeField] private GameObject currentTarget;
    [SerializeField] private GameObject cargoTarget;
    [SerializeField] private GameObject playerTarget;
    [SerializeField] private LayerMask playerMask;
    #endregion

    private void Start()
    {
        Init();

        //playerTarget = GameObject.FindGameObjectWithTag("Player");
    }

    void Init() // This should be called when the enemy gets reused
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player");
        //cargoTarget = GameObject.FindGameObjectWithTag("Cargo");

        currentTarget = playerTarget;

        isDead = false;
        pauseMovement = false;
        canAttack = true;

        switch (enemyType)
        {
            case 0: // Speed Enemy
                health = healthSpeed;
                break;
            case 1: // Enemy AOE
                health = healthDrone;
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
            transform.position = Vector3.MoveTowards(transform.position, currentTarget.transform.position, moveSpeed * Time.deltaTime);

            if(enemyType == 1)
            {
                transform.position = new Vector3(transform.position.x, 2f, transform.position.z); // This just sets the height right but should be fixed later
            }
        }

        if (!isDead)
        {
            AttackPlayer();
        }
        
    }

    public void TakeDamage(int dmg, float knockback, Vector3 dir)
    {
        health -= dmg;

        if(knockback > 0)
        {
            //transform.position += dir * 3;
            transform.position = Vector3.Lerp(transform.position, transform.position + dir * knockback, 5 * Time.deltaTime);
        }

        if(health <= 0)
        {
            //Debug.Log("Enemy Dead");
            isDead = true;
            pauseMovement = true;

            //StopAllCoroutines(); // doesn't stop coroutines started from another script
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

    public IEnumerator AggroCoroutine(float length)
    {
        GameObject origTarget = currentTarget;

        if (immuneAggro)
        {
            yield break;
        }
        else
        {
            currentTarget = playerTarget; // This technically does nothing now since the enemies only target the player, needs to be switched after the Cargo is added
        }

        yield return new WaitForSeconds(length);

        if (!isDead)
        {
            //Debug.Log("Undo Aggro");
            currentTarget = origTarget;
        }
    }

    /*public void SwitchAggro(float length)
    {
        GameObject origTarget = currentTarget;

        if (immuneAggro)
        {
            return;
        }
        else
        {
            currentTarget = playerTarget; // This technically does nothing now since the enemies only target the player, needs to be switched after the Cargo is added
        }

        //new WaitForSeconds(length);

        //Debug.Log("After " + length + " seconds, this switched aggro");

        if(length > 0)
        {
            length -= Time.deltaTime;
        }
        else if (length <= 0)
        {
            currentTarget = origTarget;
        }
    }*/
}
