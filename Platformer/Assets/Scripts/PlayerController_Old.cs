/*
using System;

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour


{
    public Rigidbody2D rb;

    private bool moving;
    private bool movingLeft;
    private bool movingRight;
    private bool jumping;

    private float acceleration;
    public float jumpForce;


    public LayerMask groundLayer;
    public Transform groundCheck;
    public float checkRadius = 0.7f;

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
        Debug.Log(playerInput);
        MovementUpdate(playerInput);
        IsGrounded();

        if (hInput <= -0.01 || hInput >= 0.01)
        {
            moving = true;
        }
        else
        {
            moving = false;
        }


        if (hInput <= -0.01)
        {
            movingLeft = true;
        }
        else
        {
            movingLeft = false;
        }


        if (hInput >= 0.01)
        {
            movingRight = true;
        }
        else
        {
            movingRight = false;
        }




        //Debug.Log(jumping);
        //Debug.Log(rb.velocity);
        //Debug.Log(hInput);
    }


    public void FixedUpdate()
    {

        if (Input.GetKeyDown("space"))
        {
            if (IsGrounded())
            {
                Debug.Log(rb.velocity);
                rb.velocity = Vector2.up * jumpForce;
            }
        }


    }

    public void Jump()
    {

    }

    private void MovementUpdate(Vector2 playerInput)
    {
        Vector2 velocity = rb.velocity;


        if (playerInput.x != 0)
        {
            velocity += playerInput * acceleration * Time.fixedDeltaTime;
        }
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
        if (jumping == true)
        {
            return false;
        }
        return true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        jumping = false;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        jumping = true;
    }



    public FacingDirection GetFacingDirection()
    {
        if (movingLeft == true && movingRight == false)
        {
            return FacingDirection.left;
        }
        else
        {
            return FacingDirection.right;
        }
    }
}
*/
