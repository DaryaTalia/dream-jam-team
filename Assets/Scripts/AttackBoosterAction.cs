using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBoosterAction : MonoBehaviour
{
    [SerializeField]
    int boostAmount = 3;
    [SerializeField]
    int boostDuration = 5;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AttackBoost());
    }

    IEnumerator AttackBoost()
    {
        GameManager.Instance.playerAttack.AdjustBonusAttackDamage(boostAmount);
        yield return new WaitForSeconds(boostDuration);
        GameManager.Instance.playerAttack.AdjustBonusAttackDamage(0);
        Destroy(gameObject);
    }
}
