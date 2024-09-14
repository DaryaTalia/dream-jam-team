using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    #region Variables
    [SerializeField] Transform target; // Aim to Mouse Cursor, should this be the GameObject cursor or point to script for moving the cursor

    public bool DBNO_atk = false;

    public PlayerClass playerClass;
    public GameObject magicianAbilityPrefab;
    public float magicianAbilityCooldown = 0.5f;
    private float abilityCooldown = 0f;
    //private float cooldownTimer = 0f;
    
    public Animator animatorMouse;

    [SerializeField] Animator animatorChad;

    private float chargeTime;
    [SerializeField] private float chargeTimeMax = 1.5f;
    public bool chargingAttack = false;

    private float attackTime;
    [SerializeField] private float attackTimeMax = 0.2f;

    private float abilityTime;
    [SerializeField] private float abilityTimeMax = 3f;

    public float playerRange;

    [SerializeField] LayerMask enemyMask;

    #endregion

    // Update is called once per frame
    void Update()
    {
        /*// Tick the cooldown timer
        cooldownTimer += Time.deltaTime;*/  // Not needed, already have abilityTime to track ability cooldown
        
        // We do this every frame to allow live tweaking the value, but still have this switch in one place
        switch (playerClass)
        {
            case PlayerClass.Magician:
                abilityCooldown = magicianAbilityCooldown;
                break;
            case PlayerClass.Brawler:
                break;
        }


        if (!DBNO_atk)
        {
            turnLeftRight();

            #region Basic and Charge Attack
            if (attackTime <= 0)
            {
                if (Input.GetMouseButtonDown(0)) // This should count as being held down unless you have MouseButtonUp
                {
                    //Debug.Log("Basic Attack");
                    chargingAttack = true;
                    animatorChad.SetBool("AtkCharging", true);
                    animatorMouse.SetBool("isAttacking", true);
                }

                if (Input.GetMouseButtonUp(0))
                {
                    chargingAttack = false;
                    animatorChad.SetBool("AtkCharging", false);
                    animatorMouse.SetBool("isAttacking", false);

                    if (chargeTime >= chargeTimeMax)
                    {
                        // ChargeAttack(); // controlled in anim event now
                        animatorChad.SetBool("AtkCharge", true);
                        attackTime = attackTimeMax;
                    }
                    else
                    {
                        //BasicAttack(); // To see if this will let anim event do it
                        animatorChad.SetBool("AtkCharging", false);
                        animatorChad.SetBool("AtkBasic", true);
                        attackTime = attackTimeMax;
                    }
                    chargeTime = 0;
                }

                if (chargingAttack)
                {
                    chargeTime += Time.deltaTime;
                }
            }

            if (attackTime > 0)
            {
                attackTime -= Time.deltaTime;
            }

            #endregion

            #region Class Ability
            if (abilityTime <= 0)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    ClassAbility();
                    abilityTime = abilityTimeMax;
                }
            }
            if (abilityTime > 0)
            {
                abilityTime -= Time.deltaTime;
            }


            #endregion
        }
    }

    void turnLeftRight()
    {
        Vector3 pos = target.position;
        Vector3 dir = (pos - this.transform.position).normalized;

        if(dir.x > 0) // should be right
        {
            transform.localScale = new Vector3(18, 18, 1);
        }
        if(dir.x < 0)
        {
            transform.localScale = new Vector3(-18, 18, 1);
        }
    }

    void BasicAttack()
    {
        Vector3 pos = target.position;
        Vector3 dir = (pos - this.transform.position).normalized;
        //Debug.DrawLine(this.transform.position, this.transform.position + dir * playerRange, Color.red, Mathf.Infinity); // Draws line from Cursor to Player at range 10

        Vector3 playerBox = new Vector3(2, 4, 3);

        if (Physics.CheckBox(this.transform.position + (dir * playerRange/2), playerBox, Quaternion.identity, enemyMask))
        {
            //Debug.Log("Enemy in Range");

            Collider[] EnemiesInRange = Physics.OverlapBox(this.transform.position + (dir * playerRange / 2), playerBox, Quaternion.identity, enemyMask);

            for (int i = 0; i < EnemiesInRange.Length; i++)
            {
                // Below is my old code to get the Enemy script for taking damage
                //EnemiesInRange[i].transform.parent.gameObject.GetComponent<Enemy_Controller>().TakeDamageMethod(playerDamage);

                //Debug.Log(EnemiesInRange[i]);
                EnemiesInRange[i].GetComponent<EnemyManager>().TakeDamage(1, 10, dir);
            }
        }

        
    }

    void atkBasicFalse()
    {
        animatorChad.SetBool("AtkBasic", false);
    }

    void ChargeAttack()
    {
        //Debug.Log("Charge Attack");

        // Create Checksphere located in Direction of the MouseCursor (target)
        // can be a bigger or something or a completely different attack

        Vector3 pos = target.position;
        Vector3 dir = (pos - this.transform.position).normalized;

        Vector3 playerBox = new Vector3(4, 4, 2);

        if (Physics.CheckBox(this.transform.position + (dir * playerRange / 2), playerBox, Quaternion.identity, enemyMask))
        {
            //Debug.Log("Enemy in Range");

            Collider[] EnemiesInRange = Physics.OverlapBox(this.transform.position + (dir * playerRange / 2), playerBox, Quaternion.identity, enemyMask);

            for (int i = 0; i < EnemiesInRange.Length; i++)
            {
                // Below is my old code to get the Enemy script for taking damage
                //EnemiesInRange[i].transform.parent.gameObject.GetComponent<Enemy_Controller>().TakeDamageMethod(playerDamage);

                //Debug.Log(EnemiesInRange[i]);
                EnemiesInRange[i].GetComponent<EnemyManager>().TakeDamage(2, 50, dir);
            }
        }
    }

    void atkChargeFalse()
    {
        animatorChad.SetBool("AtkCharge", false);
    }

    void ClassAbility()
    {
        /*if (cooldownTimer < abilityCooldown)
        {
            return;
        }
        
        cooldownTimer = 0f;*/

        switch (playerClass)
        {
            case PlayerClass.Magician:
                Instantiate(magicianAbilityPrefab, transform.position, Quaternion.identity);
                break;

            case PlayerClass.Brawler: // Brwaler Aggro Taunt
                if (Physics.CheckSphere(this.transform.position, 6, enemyMask))
                {
                    Collider[] EnemiesInRange = Physics.OverlapSphere(this.transform.position, 6, enemyMask);

                    for (int i = 0; i < EnemiesInRange.Length; i++)
                    {
                        //EnemiesInRange[i].GetComponent<EnemyManager>().SwitchAggro(5);
                        //EnemiesInRange[i].GetComponent<EnemyManager>().StartCoroutine(AggroCoroutine(5));
                        StartCoroutine(EnemiesInRange[i].GetComponent<EnemyManager>().AggroCoroutine(5));
                    }
                }
                break;
        }
    }
}

public enum PlayerClass
{
    Magician, Brawler
}
