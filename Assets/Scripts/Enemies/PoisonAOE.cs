using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonAOE : MonoBehaviour
{

    #region
    private float maxScale = 20;
    private float currentScale = 4;

    private float deathTimer = 5;

    private int aoeRadius = 3;

    [SerializeField] private LayerMask playerMask;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(this.transform.localScale.x);
        this.transform.localScale = new Vector3 (4,4,1);

        Debug.DrawLine(this.transform.position, this.transform.position + new Vector3(3, 0, 0), Color.red, Mathf.Infinity);

        InvokeRepeating("TickDmgAOE", .2f, .2f);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (currentScale < maxScale)
        {
            currentScale += .05f;
            this.transform.localScale = new Vector3(currentScale, currentScale, 1);
        }

        deathTimer -= Time.deltaTime;

        if (deathTimer < 0)
        {
            Destroy(gameObject);
        }

        
    }

    void TickDmgAOE()
    {
        //Debug.Log("Tick");
        Collider[] PlayerInRange = Physics.OverlapSphere(this.transform.position, aoeRadius, playerMask);
        if (PlayerInRange.Length > 0)
        {
            //PlayerInRange[0].GetComponent<PlayerStats>().TakeDamage(1);
            StartCoroutine(PlayerInRange[0].GetComponent<PlayerStats>().TickDmg(2));
        }
    }



}
