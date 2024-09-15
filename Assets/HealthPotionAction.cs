using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class HealthPotionAction : MonoBehaviour
{
    [SerializeField] int healthAmount = 30;
    void Start()
    {
        GameManager.Instance.playerStats.AddHealth(healthAmount);
        Destroy(gameObject);
    }
}
