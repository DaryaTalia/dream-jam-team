using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class PlayerStats : MonoBehaviour
{
    #region Variables
    [SerializeField] private float playerHealthMax;
    [SerializeField] private float playerHealthCurrent;
    [SerializeField] private GameObject healthMask;

    private bool isInvincible = false;
    private bool DBNO = false;
    private float DBNOTimer = 0f;
    [SerializeField] private float DBNOTimerMax;

    [SerializeField] private int playerDamage;

    public bool underEffect;

    public float invicibilityTimer;
    public float invicibilityTimerMax;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        Init();   
    }

    public void Init()
    {
        playerHealthCurrent = playerHealthMax;
        underEffect = false;
        isInvincible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(DBNOTimer > 0 && DBNO)
        {
            DBNOTimer -= Time.deltaTime;
            //Debug.Log(DBNOTimer);
        }

        if(DBNOTimer <= 0)
        {
            //Debug.Log("Revive");
            DBNO = false;
            GetComponent<PlayerMovement>().DBNO_move = false;
            GetComponent<PlayerAttack>().DBNO_atk = false;
            playerHealthCurrent = playerHealthMax; // Need to change cause this refills hp every turn
            DBNOTimer = DBNOTimerMax;
        }

        if(invicibilityTimer > 0)
        {
            invicibilityTimer -= Time.deltaTime;
        }
    }

    public void ToggleInvincibility()
    {
        if(isInvincible)
        {
            isInvincible = false;
        }
        else
        {
            isInvincible = true;
        }
    }

    public void TakeDamage(int dmg)
    {
        // Check if player can be damaged based on vulnerability checks
        if (invicibilityTimer <= 0 && !isInvincible)
        {
            playerHealthCurrent -= dmg;
            healthMask.GetComponentInChildren<Image>().fillAmount = playerHealthCurrent / playerHealthMax;
            //Debug.Log(healthMask.GetComponentInChildren<Image>().fillAmount + " currenthp: " + playerHealthCurrent + " maxhp: " + playerHealthMax);
        }

        // Iterate natural invincibility based on health
        if (invicibilityTimer <= 0)
        {
            if (playerHealthCurrent <= 0 && !DBNO)
            {
                //Debug.Log("Player DBNO");
                DBNO = true;
                GetComponent<PlayerMovement>().DBNO_move = true;
                GetComponent<PlayerAttack>().DBNO_atk = true;
                DBNOTimer = DBNOTimerMax;
            }
            invicibilityTimer = invicibilityTimerMax;
        }
    }
    
    public void AddHealth(int amount)
    {
        playerHealthCurrent += amount;
        healthMask.GetComponentInChildren<Image>().fillAmount = playerHealthCurrent / playerHealthMax;
    }

    
    public IEnumerator TickDmg(int noTicks)
    {
        int maxTicks = noTicks;

        if (!underEffect)
        {
            underEffect = true;

            while (maxTicks > 0)
            {
                TakeDamage(1);
                //Debug.Log("Tick 1 Dmg");
                maxTicks--;
                
                yield return new WaitForSeconds(.75f);

                if (maxTicks <= 0)
                {
                    underEffect = false;
                }
            }
        }
        else
        {
            yield break;
        }
        
    }
}
