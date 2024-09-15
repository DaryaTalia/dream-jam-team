using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    #region Variables
    [SerializeField] private int playerHealthMax;
    [SerializeField] private int playerHealthCurrent;
    
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

    void Init()
    {
        playerHealthCurrent = playerHealthMax;
        underEffect = false;
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

    public void TakeDamage(int dmg)
    {
        if(invicibilityTimer <= 0)
        {
            playerHealthCurrent -= dmg;

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
