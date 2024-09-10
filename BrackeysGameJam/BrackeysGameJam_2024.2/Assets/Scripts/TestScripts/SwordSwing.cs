using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSwing : MonoBehaviour
{
    Animator swingAnim;

    // Start is called before the first frame update
    void Start()
    {
        swingAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SwingEnd()
    {
        //Debug.Log("Swing End");
        swingAnim.SetBool("isAttacking", false);
    }
}
