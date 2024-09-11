using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChainLightning : MonoBehaviour
{
    [SerializeField] private bool canRepeatHit = false;
    [SerializeField] private int stepsToChain = 2;
    [SerializeField] private float radius = 1.0f;
    [SerializeField] LayerMask enemyMask;
    
    private void Start()
    {
        List<Collider> enemies = new List<Collider>();
        Vector3 origin = transform.position;
        for (int i = 0; i < stepsToChain; i++)
        {
            (origin, enemies) = StepChain(origin);
            if (enemies == null || enemies.Count == 0)
            {
                break;
            }
        }   
    }

    private (Vector3, List<Collider>) StepChain(Vector3 origin)
    {
        List<Collider> enemiesInRange = Physics.OverlapSphere(origin, radius, enemyMask, QueryTriggerInteraction.Ignore).ToList();
        var closestEnemy = FindClosestEnemy(enemiesInRange);
        if (closestEnemy == null)
        {
            return (Vector3.zero, null);
        }
        var enemyPosition = closestEnemy.transform.position;
        
        // Apply dmg to enemy
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
