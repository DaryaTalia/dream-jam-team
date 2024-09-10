using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    #region Variables
    [SerializeField] Transform target; // Aim to Mouse Cursor, should this be the GameObject cursor or point to script for moving the cursor

    public float chargeTime;
    public float chargeTimeMax;
    public bool chargingAttack = false;

    #endregion

    // Update is called once per frame
    void Update()
    {

        #region Basic and Charge Attack
        if (Input.GetMouseButtonDown(0)) // This should count as being held down unless you have MouseButtonUp
        {
            //Debug.Log("Basic Attack");
            chargingAttack = true;
        }

        if(Input.GetMouseButtonUp(0))
        {
            chargingAttack = false;
            
            if(chargeTime >= chargeTimeMax)
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
        ClassAbility();
        #endregion
    }

    void BasicAttack()
    {
        Debug.Log("Basic Attack");
        Vector3 pos = target.position;
        Vector3 dir = (this.transform.position - pos).normalized;
        Debug.DrawLine(pos, pos + dir * 10, Color.red, Mathf.Infinity);

        // Create Checksphere located in Direction of the MouseCursor (target)
        // CheckSphere(Vector3 position, float radius, int layerMask = DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal);
        // First Position is Player transform.position + offset for Normalized Direction for MouseCursor
        // radius is size of attack
        // layermask is for Enemies layer
        // Don't need TriggerInteraction
    }

    void ChargeAttack()
    {
        Debug.Log("Charge Attack");

        // Create Checksphere located in Direction of the MouseCursor (target)
    }

    void ClassAbility()
    {


        // Create Checksphere located in Direction of the MouseCursor (target) depending on Class
    }
}
