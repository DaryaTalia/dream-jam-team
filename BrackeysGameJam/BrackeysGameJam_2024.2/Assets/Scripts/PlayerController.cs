using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    #region Variables
    [SerializeField] private float moveSpeed;
    [SerializeField] private int playerHealthMax;
    [SerializeField] private int playerHealthCurrent;
    [SerializeField] private int playerDamage;

    [SerializeField] private float dashMult = 1;
    [SerializeField] private float dashTimerMax = .3f;
    [SerializeField] private float dashTimer;
    [SerializeField] private float dashCooldown = 0;
    [SerializeField] private float dashCooldownMax = 1f;

    private Vector3 moveDirection;

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
        if (dashTimer > 0)
        {
            dashTimer -= Time.deltaTime;
        }
        if (dashCooldown > 0)
        {
            dashCooldown -= Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldown <= 0)
        {
            dashMult = 4; // have to make the camera lag a bit because hyper movement is jarring
            dashTimer = dashTimerMax;
            dashCooldown = dashCooldownMax;
        }
        if (dashTimer <= 0)
        {
            dashMult = 1; // reset to base, non mult
        }

        moveDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) moveDirection.z = +1f * dashMult;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) moveDirection.z = -1f * dashMult;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) moveDirection.x = -1f * dashMult;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) moveDirection.x = +1f * dashMult;
            
        Vector3 moveDir = transform.forward * moveDirection.z + transform.right * moveDirection.x;

        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

}
