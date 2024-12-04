using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum FacingDirection
    {
        left, right
    }
    public FacingDirection currentFacingDirection = FacingDirection.right;

    public enum CharacterState
    {
        idle, walk, jump, die
    }
    public CharacterState currentCharacterState = CharacterState.idle;
    public CharacterState previousCharacterState = CharacterState.idle;


    public float accelerationTime;
    public float decelerationTime;
    public float maxSpeed;
    public float jumpForce;
    public float terminalSpeed;
    public float coyoteTime;


    public int health = 10;

    private Rigidbody2D playerRB;
    private float acceleration;
    private bool isJumping = false;
    private bool onTheGround;
    private bool collisionExit;
    private bool walking;

    private float groundCheck = 1f;
    private float wallCheck = 1f;
    public LayerMask groundLayer;

    private bool isTouchingWall = false;

    public float dashSpeed = 10f;
    public float dashDuration = 0.2f;
    private bool isDashing = false;
    private float dashCooldown = 1f;
    private float dashCooldownTimer = 0f;

    private bool walkingLeft = false;
    private bool walkingRight = false;

    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        acceleration = maxSpeed / accelerationTime;
    }


    void Update()
    {

        previousCharacterState = currentCharacterState;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheck, groundLayer);
        RaycastHit2D wallHitLeft = Physics2D.Raycast(transform.position, Vector2.left, wallCheck, groundLayer);
        RaycastHit2D wallHitRight = Physics2D.Raycast(transform.position, Vector2.right, wallCheck, groundLayer);
        isTouchingWall = wallHitLeft.collider != null || wallHitRight.collider != null;

        if (hit.collider != null)
        {
            onTheGround = true;
            coyoteTime = 1;
        } else if (hit.collider == null) {
            {
                coyoteTime -= 5 * Time.deltaTime;

                if (coyoteTime <= 0)
                {
                    onTheGround = false;
                }
            }
        }

        if (isTouchingWall && !onTheGround && Input.GetKeyDown(KeyCode.UpArrow))
        {
            isJumping = true;
        }



        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }


        if (IsGrounded() && Input.GetKeyDown(KeyCode.UpArrow))
        {
            isJumping = true;
        }


        switch (currentCharacterState)
        {
            case CharacterState.die:

                break;
            case CharacterState.jump:

                if (IsGrounded())
                {

                    if (IsWalking())
                    {
                        currentCharacterState = CharacterState.walk;
                    }
                    else
                    {
                        currentCharacterState = CharacterState.idle;
                    }
                }

                break;
            case CharacterState.walk:
                if (!IsWalking())
                {
                    currentCharacterState = CharacterState.idle;
                }

                if (!IsGrounded())
                {
                    currentCharacterState = CharacterState.jump;
                }
                break;
            case CharacterState.idle:

                if (IsWalking())
                {
                    currentCharacterState = CharacterState.walk;
                }

                if (!IsGrounded())
                {
                    currentCharacterState = CharacterState.jump;
                }

                break;

        }
    }

    private void FixedUpdate()
    {
        
        Vector2 playerInput = new Vector2();
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            playerInput += Vector2.left;
            walkingLeft = true;
            walkingRight = false;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            playerInput += Vector2.right;
            walkingRight = true;
            walkingLeft = false;
        }
        if (isJumping)
        {
            playerRB.velocity += Vector2.up * jumpForce;
            isJumping = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0 && !isDashing)
        {
            StartCoroutine(Dashing());
        }


        MovementUpdate(playerInput);
    }



    private void MovementUpdate(Vector2 playerInput)
    {
        Vector2 velocity = playerRB.velocity;
        if (playerInput.x != 0)
        {
            velocity += playerInput * acceleration * Time.fixedDeltaTime;
            if (IsGrounded())
            {
                walking = true;
                
            }

        }
        else
        {
            velocity = new Vector2(0, velocity.y);
            walking = false;
        }

        if (velocity.y < 0)
        {
            velocity.y = Mathf.Max(velocity.y, -terminalSpeed);
        }

        playerRB.velocity = velocity;
    }

    public bool IsWalking()
    {
        if (walking == true)
        {
            return true;
        }
        return false;
    }
    public bool IsGrounded()
    {

        if (onTheGround == true)
        {
            return true;
        }
        return false;
    }




    public bool IsDead()
    {
        return health <= 0;
    }

    public FacingDirection GetFacingDirection()
    {
        if (playerRB.velocity.x > 0)
        {
            currentFacingDirection = FacingDirection.right;
        }
        else if (playerRB.velocity.x < 0)
        {
            currentFacingDirection = FacingDirection.left;
        }

        return currentFacingDirection;
    }

    private IEnumerator Dashing()
    {

        isDashing = true;
        float dashingDirection = 0;
        
        
        if (walkingLeft == true && walkingRight == false)
        {
            dashingDirection = -1;
        } else if (walkingLeft == false && walkingRight == true)
        {
            dashingDirection = 1;
        }


        playerRB.velocity = new Vector2(dashingDirection * dashSpeed, 0);

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

    }

}