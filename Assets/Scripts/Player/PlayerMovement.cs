using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    public bool DBNO_move = false;

    [SerializeField] private float moveSpeed;
    private Vector3 moveDirection;

    [SerializeField] private float dashMultBase;
    [SerializeField] private float dashMultMax = 3;
    [SerializeField] private float dashMultReset = 1;
    [SerializeField] private float dashTimerMax = .3f;
    [SerializeField] private float dashTimer;
    [SerializeField] private float dashCooldown = 0;
    [SerializeField] private float dashCooldownMax = 1f;

    [SerializeField] private Animator animator;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {

        #region Dash Timer and Strength Controls
        if (dashTimer > 0) // This is for the length of the dash in seconds, Timer
        {
            dashTimer -= Time.deltaTime;
        }
        if (dashCooldown > 0) // This is for the Dash Cooldown Timer
        {
            dashCooldown -= Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldown <= 0)
        {
            // Will need to make camera lag a bit because hyper movement is jarring if the camera is rigid to the player
            dashMultBase = dashMultMax; // Strength of the Dash
            dashTimer = dashTimerMax; // Sets timer to start countdown (see above)
            dashCooldown = dashCooldownMax; // Sets timer to start countdown (see above)
            GetComponentInParent<PlayerStats>().invicibilityTimer = GetComponentInParent<PlayerStats>().invicibilityTimerMax / 2;
        }
        if (dashTimer <= 0)
        {
            dashMultBase = dashMultReset; // reset to base, non multiplier
        }
        #endregion

        moveDirection = Vector3.zero; // Sets movement to Zero so it'll stop after movekey is released
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) moveDirection.z = +1f * dashMultBase;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) moveDirection.z = -1f * dashMultBase;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) 
        { 
            moveDirection.x = -1f * dashMultBase; 
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            moveDirection.x = +1f * dashMultBase;
        }
        

        Vector3 moveDir = transform.forward * moveDirection.z + transform.right * moveDirection.x;

        if(moveDir != new Vector3(0,0,0))
        {
            //Debug.Log("Is Moving");
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        if (!DBNO_move)
        {
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }
    }

}
