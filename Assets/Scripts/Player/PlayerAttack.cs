using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    #region Variables
    [SerializeField] Transform target; // Aim to Mouse Cursor, should this be the GameObject cursor or point to script for moving the cursor
    public Animator animator;
    
    public PlayerClass playerClass;
    public GameObject magicianAbilityPrefab;
    public float magicianAbilityCooldown = 0.5f;
    private float abilityCooldown = 0f;
    private float cooldownTimer = 0f;
    
    public float chargeTime;
    public float chargeTimeMax;
    public bool chargingAttack = false;
    public float playerRange;

    [SerializeField] LayerMask enemyMask;

    #endregion

    // Update is called once per frame
    void Update()
    {
        // Tick the cooldown timer
        cooldownTimer += Time.deltaTime;
        
        // We do this every frame to allow live tweaking the value, but still have this switch in one place
        switch (playerClass)
        {
            case PlayerClass.Magician:
                abilityCooldown = magicianAbilityCooldown;
                break;
        }

        #region Basic and Charge Attack
        if (Input.GetMouseButtonDown(0)) // This should count as being held down unless you have MouseButtonUp
        {
            //Debug.Log("Basic Attack");
            chargingAttack = true;
            animator.SetBool("isAttacking", true);
        }

        if(Input.GetMouseButtonUp(0))
        {
            chargingAttack = false;
            animator.SetBool("isAttacking", false);

            if (chargeTime >= chargeTimeMax)
            {
                ChargeAttack();
            }
            else
            {
                BasicAttack();
            }

            chargeTime = 0;
        }

        if (chargingAttack)
        {
            chargeTime += Time.deltaTime;
        }
        #endregion

        #region Class Ability
        if (Input.GetMouseButtonDown(1))
        {
            ClassAbility();
        }
        #endregion
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
                EnemiesInRange[i].GetComponent<EnemyManager>().TakeDamage(1, 0, Vector3.zero);
            }
        }
    }

    void ChargeAttack()
    {
        Debug.Log("Charge Attack");

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
                EnemiesInRange[i].GetComponent<EnemyManager>().TakeDamage(2, 3, dir);
            }
        }
    }

    void ClassAbility()
    {
        if (cooldownTimer < abilityCooldown)
        {
            return;
        }
        
        cooldownTimer = 0f;
        Debug.Log("Class Ability");

        // Create Checksphere located in Direction of the MouseCursor (target) depending on Class
        switch (playerClass)
        {
            case PlayerClass.Magician:
                Instantiate(magicianAbilityPrefab, transform.position, Quaternion.identity);
                break;
        }
    }

}

public enum PlayerClass
{
    Magician,
}
