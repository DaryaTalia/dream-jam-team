using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChainLightning : MonoBehaviour
{
    [SerializeField] private bool canRepeatHit = false;
    [SerializeField] private int stepsToChain = 2;
    [SerializeField] private float radius = 1.0f;
    [SerializeField] LayerMask enemyMask;
    
    private void Start()
    {
        Vector3 origin = transform.position;
        List<Collider> enemiesInRange = Physics.OverlapSphere(origin, radius, enemyMask, QueryTriggerInteraction.Ignore).ToList();
        for (int i = 0; i < stepsToChain; i++)
        {
            (origin, enemiesInRange) = StepChain(origin, enemiesInRange);
            if (enemiesInRange == null || enemiesInRange.Count == 0)
            {
                break;
            }
        }

        //TODO: This is to allow debug line to show up. Will need to change to accomodate vfx
        StartCoroutine(DelayDestroy()); 
    }

    private IEnumerator DelayDestroy()
    {
        yield return new WaitForSeconds(Random.Range(1f, 2f));
        Destroy(gameObject);
    }

    private (Vector3, List<Collider>) StepChain(Vector3 origin, List<Collider> enemiesInRange)
    {
        var closestEnemy = FindClosestEnemy(enemiesInRange);
        if (closestEnemy == null)
        {
            return (Vector3.zero, null);
        }
        var enemyPosition = closestEnemy.transform.position;
        
        //TODO: Apply dmg to enemy
        //TODO: Do real vfx
        Debug.DrawLine(origin, enemyPosition, Color.blue, 1f);


        if (!canRepeatHit)
        {
            enemiesInRange.Remove(closestEnemy);
        }
        
        return (enemyPosition, enemiesInRange);
    }

    private Collider FindClosestEnemy(List<Collider> enemies)
    {
        Collider closestEnemy = null;
        foreach (var enemy in enemies)
        {
            var distanceFromEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            var closestDistance = closestEnemy != null ? Vector3.Distance(transform.position, closestEnemy.transform.position) : 1000f;
            if (distanceFromEnemy < closestDistance)
            {
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }
}
