using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour


{
    public Rigidbody2D rb;

    private bool moving;
    public float jumpForce = 1500f;

    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {
         rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
 
        float hInput = Input.GetAxis("Horizontal");
        float speed = 100 * Time.deltaTime;
        Vector2 playerInput = new Vector2(hInput * speed, 0);
        MovementUpdate(playerInput);

        if (hInput >= 0.01 || hInput <= -0.01)
        {
            moving = true;
        } else
        {
            moving = false;
        }

        if (Input.GetKeyDown("space"))
        {
            Jump();
        }

        //Debug.Log(rb.velocity);
    }


    public void Jump()
    {

            rb.AddForce (Vector2.up * jumpForce, ForceMode2D.Force);
    }

    private void MovementUpdate(Vector2 playerInput)
    {
        rb.MovePosition(rb.position + playerInput);
    }


    public bool IsWalking()
    {
        if (moving == true)
        {
            return true;
        }
        return false;
    }
    public bool IsGrounded()
    {
        if (rb.velocity == Vector2.zero)
        {
            return true;
        }
        return false;
        
    }

    public FacingDirection GetFacingDirection()
    {
        return FacingDirection.left;
    }
}

