using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    #region Variables
    [SerializeField] private int playerHealthMax;
    [SerializeField] private int playerHealthCurrent;
    [SerializeField] private int playerDamage;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        Init();   
    }

    void Init()
    {
        playerHealthCurrent = playerHealthMax;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int dmg)
    {
        playerHealthCurrent -= dmg;

        if(playerHealthCurrent <= 0)
        {
            Debug.Log("Player DBNO");
            // Need to "stun" player and start timer and when that ends, they will recover X (all or some, idk) life and can continue fighting
        }
    }
}
