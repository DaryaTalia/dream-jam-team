using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyManager : MonoBehaviour
{
    #region Variables
    //[SerializeField] private int enemyType; // 0 speed / 1 drone / 2 Imob / 3 DmgDlr
    [SerializeField] private EnemyClass enemyClass;
    [SerializeField] private GameObject AOEField;

    [SerializeField] private Animator animatorSpeed;
    [SerializeField] private Animator animatorElite;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private float tempXScale;

    public int health; // made public to see for testing
    [SerializeField] private int healthSpeed;
    [SerializeField] private int healthDrone;
    [SerializeField] private int healthGravy;
    [SerializeField] private int healthElite;

    [SerializeField] private float knockbackRes;

    public bool isDead;
    public bool pauseMovement;

    private bool immuneAggro; // Set Immobilizer Immune to Aggro

    [SerializeField] private Collider[] PlayerInRange;
    [SerializeField] private Collider[] CargoInRange;
    [SerializeField] private float enemyRange;
    public float atkCooldownMax;
    public float atkCooldown;
    public float cooldownMovePause;
    public bool canAttack;

    [SerializeField] private int damage;

    [SerializeField] private float moveSpeed;

    [SerializeField] private GameObject currentTarget;
    [SerializeField] private GameObject cargoTarget;
    [SerializeField] private GameObject playerTarget;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask cargoMask;
    public ObjectPool<EnemyManager> pool;
    #endregion

    private void Start()
    {
        Init();

        //playerTarget = GameObject.FindGameObjectWithTag("Player");
    }

    public void Init() // This should be called when the enemy gets reused
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player");
        cargoTarget = GameObject.FindGameObjectWithTag("Cargo");

        currentTarget = cargoTarget;

        isDead = false;
        pauseMovement = false;
        canAttack = true;
        tempXScale = transform.localScale.x;

        atkCooldown = -1;

        switch (enemyClass)
        {
            case EnemyClass.Speed: // Speed Enemy
                health = healthSpeed;
                break;
            case EnemyClass.Drone: // Enemy Drone
                health = healthDrone;
                break;
            case EnemyClass.Imob: // Enemy Gravy
                health = healthGravy;
                immuneAggro = true;
                break;
            case EnemyClass.Elite: // Enemy Elite
                health = healthElite;
                immuneAggro = true;
                break;
        }
    }

    public enum EnemyClass
    {
        Speed, Drone, Imob, Elite
    }

    // Update is called once per frame
    void Update()
    {
        if (!pauseMovement)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentTarget.transform.position, moveSpeed * Time.deltaTime);

            if(enemyClass == EnemyClass.Drone)
            {
                transform.position = new Vector3(transform.position.x, 2f, transform.position.z); // This just sets the height right but should be fixed later
            }
        }

        if (!isDead)
        {
            AttackPlayer();

            turnLeftRight();
        }
        
    }

    void turnLeftRight()
    {
        Vector3 pos = playerTarget.transform.position;
        Vector3 dir = (pos - this.transform.position).normalized;

        //float dirflipper = -1;

        //Debug.Log(dir.x);

        if (dir.x > 0) // should be right
        {
            spriteRenderer.flipX = true;
            //transform.localScale = new Vector3(-tempXScale, transform.localScale.y, 1);
        }
        if (dir.x < 0)
        {
            spriteRenderer.flipX = false;
            //transform.localScale = new Vector3(tempXScale, transform.localScale.y, 1);
        }
    }

    public void TakeDamage(int dmg, float knockback, Vector3 dir)
    {
        health -= dmg;

        knockback = knockback * knockbackRes;

        if(knockback > 0)
        {
            //transform.position += dir * 3;
            transform.position = Vector3.Lerp(transform.position, transform.position + dir * knockback, 5 * Time.deltaTime);
        }

        if(health <= 0)
        {
            AudioManager.instance.Play("Enemy Die");

            //Debug.Log("Enemy Dead");
            isDead = true;
            pauseMovement = true;

            //StopAllCoroutines(); // doesn't stop coroutines started from another script
            transform.position = new Vector3(0, -10f, 0); // Hides enemy under the map so it can be reused later
            pool.Release(this);
        }

    }

    void AttackPlayer()
    {
        AudioManager.instance.Play("Enemy Melee");

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
        
        if (atkCooldown <= atkCooldownMax/cooldownMovePause && atkCooldown > 0 && canAttack) // This pauses movement the enemy for half the cooldown timer for Atk Anim
        {
            pauseMovement = false;
            // This is where enemy will swing and try to dmg player

            Vector3 pos = playerTarget.transform.position;
            Vector3 dir = (pos - this.transform.position).normalized; // Directional Vector towards player

            switch (enemyClass)
            {
                case EnemyClass.Speed: // Speed Enemy
                    animatorSpeed.SetBool("isAttacking", true);

                    EliteDealDmg();

                    canAttack = false;

                    break;

                case EnemyClass.Drone: // Enemy Drone
                    
                    Vector3 tempPos = this.transform.position;

                    GameObject tempFieldRef = Instantiate(AOEField, tempPos, Quaternion.identity);

                    tempPos.y = 0.1f;
                    pos.y = 0.1f;
                    tempFieldRef.GetComponent<PoisonAOE>().lerpToDestination(tempPos, pos);

                    canAttack = false;

                    break;
                case EnemyClass.Imob: // Enemy Gravy
                    break;

                case EnemyClass.Elite: // Enemy Elite

                    int atkswitch = UnityEngine.Random.Range(0, 2);

                    if(atkswitch == 0)
                    {
                        animatorElite.SetBool("atk1", true);  // Will need to change this to Elite anim////////////////////////////////////////

                        /*PlayerInRange = Physics.OverlapSphere(this.transform.position + (dir * enemyRange / 2), enemyRange / 2, playerMask);
                        if (PlayerInRange.Length > 0)
                        {
                            //Debug.Log(PlayerInRange[0] + " Hit by attack");
                            PlayerInRange[0].GetComponent<PlayerStats>().TakeDamage(damage);
                        }*/

                        canAttack = false;
                    }
                    else if(atkswitch == 1)
                    {
                        animatorElite.SetBool("atk2", true);  // Will need to change this to Elite anim////////////////////////////////////////

                        /*PlayerInRange = Physics.OverlapSphere(this.transform.position + (dir * enemyRange / 2), enemyRange / 2, playerMask);
                        if (PlayerInRange.Length > 0)
                        {
                            //Debug.Log(PlayerInRange[0] + " Hit by attack");
                            PlayerInRange[0].GetComponent<PlayerStats>().TakeDamage(damage);
                        }*/

                        canAttack = false;
                    }
                    break;
            }
        }

        if (atkCooldown > 0)
        {
            atkCooldown -= Time.deltaTime;
        }
    }

    void EliteDealDmg()
    {
        Vector3 pos = playerTarget.transform.position;
        Vector3 dir = (pos - this.transform.position).normalized; // Directional Vector towards player

        PlayerInRange = Physics.OverlapSphere(this.transform.position + (dir * enemyRange / 2), enemyRange / 2, playerMask);
        if (PlayerInRange.Length > 0)
        {
            try
            {
                foreach(Collider ps in PlayerInRange)
                {
                    if (ps.GetComponent<PlayerStats>())
                    {
                        ps.GetComponent<PlayerStats>().TakeDamage(damage);
                    }
                }
            }
            catch(NullReferenceException e)
            {
                Debug.LogError(e.Message);
            }
        }

        CargoInRange = Physics.OverlapSphere(this.transform.position + (dir * enemyRange / 2), enemyRange / 2, cargoMask);
        if (CargoInRange.Length > 0)
        {
            try
            {
                foreach (Collider ps in CargoInRange)
                {
                    GameManager.Instance.cargoController.DamageCargo(damage);
                }
            }
            catch (NullReferenceException e)
            {
                Debug.LogError(e.Message);
            }
        }
    }

    void SpeedAtkFalse()
    {
        animatorSpeed.SetBool("isAttacking", false);
    }

    void EliteatksFalse()
    {
        if(animatorElite.GetBool("atk1"))
        {
            animatorElite.SetBool("atk1", false);
        }

        if (animatorElite.GetBool("atk2"))
        {
            animatorElite.SetBool("atk2", false);
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

    
}
