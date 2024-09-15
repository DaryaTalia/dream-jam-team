using UnityEngine;

public class PoisonAOE : MonoBehaviour
{

    #region
    private float maxScale = 20;
    private float currentScale = 4;

    private float deathTimer = 5;

    private int aoeRadius = 3;

    private Vector3 lerpStart;
    private Vector3 lerpEnd;

    [SerializeField] private LayerMask playerMask;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(this.transform.localScale.x);
        this.transform.localScale = new Vector3 (4,4,1);
        //this.transform.position = new Vector3(this.transform.position.x, 0.1f, this.transform.position.z);
        this.transform.Rotate(90, 0, 0);

        //Debug.DrawLine(this.transform.position, this.transform.position + new Vector3(3, 0, 0), Color.red, Mathf.Infinity); // To see how far aoeRadius is

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

        //transform.position = Vector3.MoveTowards(transform.position, lerpEnd, 10f * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        Vector3 smoothLerp = Vector3.Lerp(transform.position, lerpEnd, .4f * Time.deltaTime);
        transform.position = smoothLerp;
        //Debug.Log("Pos: " + transform.position + " start: " + lerpStart + " end: " + lerpEnd);

        //transform.position = Vector3.MoveTowards(lerpStart, lerpEnd, 1f * Time.deltaTime);

        //transform.position = Vector3.MoveTowards(transform.position, currentTarget.transform.position, moveSpeed * Time.deltaTime);
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

    public void lerpToDestination(Vector3 start, Vector3 end)
    {
        //transform.position = Vector3.Lerp(start, end, .3f);
        lerpStart = start;
        lerpEnd = end;
    }

}
