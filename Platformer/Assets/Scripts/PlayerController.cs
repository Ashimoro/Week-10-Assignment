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
    private bool walking;

    private float groundCheck = 1f;
    private float wallCheck = 1f;
    public LayerMask groundLayer;

    private bool isTouchingWall = false;
    private bool canWallJump = true;

    public float dashSpeed = 10f;
    public float dashDuration = 0.2f;
    private bool isDashing = false;
    private float dashCooldown = 1f;
    private float dashCooldownTimer = 0f;

    private bool walkingLeft = false;
    private bool walkingRight = false;


    public float hookPullSpeed = 5f;
    public float hookDetectionRadius = 10f;
    private float hookStopDistance = 1f;
    private bool isGrappling = false;
    private Transform currentHook;


    Vector2 previousVelocity;
    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        acceleration = maxSpeed / accelerationTime;
    }


    void Update()
    {

        previousCharacterState = currentCharacterState;

        // Checking for walls and floors around the character
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheck, groundLayer);
        RaycastHit2D wallHitLeft = Physics2D.Raycast(transform.position, Vector2.left, wallCheck, groundLayer);
        RaycastHit2D wallHitRight = Physics2D.Raycast(transform.position, Vector2.right, wallCheck, groundLayer);
        isTouchingWall = wallHitLeft.collider != null || wallHitRight.collider != null;

        // Changes in the character's state during collision and during jumping
        if (hit.collider != null)
        {
            onTheGround = true;
            canWallJump = true;
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

        // The wall jump feature itself
        if (isTouchingWall && !onTheGround && Input.GetKeyDown(KeyCode.UpArrow) && canWallJump == true)
        {
            isJumping = true;
            canWallJump = false;
        }


        // Code part responsible for initialization of pull to hook
        if (Input.GetKeyDown(KeyCode.E) && !isGrappling) 
        {
            GameObject nearestHook = FindNearestHook(); //Find the nearest hook
            if (nearestHook != null)
            {
                Debug.Log("Grappling");
                StartGrapple(nearestHook.transform); //Call the pull method
            }
        }


        // Dash Cooldown
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
            walkingLeft = true; // These two variables are responsible for making the game understand which way to dash.
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


        // Call of the desh function as well as initialization of cooldown
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0 && !isDashing)
        {
            dashCooldownTimer = dashCooldown;
            StartCoroutine(Dashing());
        }


        // This part of the code is responsible for pulling the player to the nearest hook after all checks
        if (isGrappling && currentHook != null)
        {
            Vector2 direction = (currentHook.position - transform.position).normalized;
            Vector2 targetPosition = Vector2.MoveTowards(transform.position, currentHook.position, hookPullSpeed * Time.fixedDeltaTime);
            playerRB.MovePosition(targetPosition);

            if (Vector2.Distance(transform.position, currentHook.position) <= hookStopDistance)
            {
                StopGrapple();
            }
        }


        
        MovementUpdate(playerInput);
    }



    private void MovementUpdate(Vector2 playerInput)
    {
        Vector2 velocity = playerRB.velocity;
        if (playerInput.x != 0)
        {
            if (velocity.x <= maxSpeed && velocity.x >= -maxSpeed) // Check that the character's speed is not higher than the maximum speed
            {
                velocity += playerInput * acceleration * Time.fixedDeltaTime;
            }

            if (IsGrounded())
            {
                walking = true;
                
            }

        }
        else
        {
            velocity -= playerInput * acceleration * Time.fixedDeltaTime;
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
        if (playerRB.velocity.x > 0.01)
        {
            currentFacingDirection = FacingDirection.right;
        }
        else if (playerRB.velocity.x < -0.01)
        {
            currentFacingDirection = FacingDirection.left;
        }

        return currentFacingDirection;
    }

    private IEnumerator Dashing()
    {

        isDashing = true; // This boolean is responsible for making sure that the player cannot move more than 1 time.
        float dashingDirection = 0;

        // This part of the code is responsible for making the game understand in which direction the player makes a dash.
        if (walkingLeft == true && walkingRight == false)
        {
            dashingDirection = -1;
        } else if (walkingLeft == false && walkingRight == true)
        {
            dashingDirection = 1;
        }


        playerRB.velocity = new Vector2(dashingDirection * dashSpeed, 0); // How far character will move during the dash

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

    }

    // This whole function is responsible for finding the nearest hook
    GameObject FindNearestHook()
    {
        GameObject[] hooks = GameObject.FindGameObjectsWithTag("Hook"); // Creating a list of all objects with this tag
        GameObject nearestHook = null; // This function is initially empty
        float minDistance = Mathf.Infinity; // I used mathf.infinity to make any value lower than the original value

        foreach (GameObject hook in hooks) //In this loop, the program will check all objects on the scene to see which one is closest, returning the one with the shortest distance to nearestHook.
        {
            float distance = Vector2.Distance(transform.position, hook.transform.position);
            if (distance < minDistance && distance <= hookDetectionRadius)
            {
                minDistance = distance;
                nearestHook = hook;
            }
        }

        return nearestHook;
    }

    // This method is responsible for returning the value of closest hook to fixedUpdate
    void StartGrapple(Transform hook)
    {
        isGrappling = true;
        currentHook = hook;
        previousVelocity = playerRB.velocity;
    }

    // Resets the value of currentHook and turns off the isGrappling state
    void StopGrapple()
    {
        isGrappling = false;
        currentHook = null;
        playerRB.velocity = previousVelocity;
    }


}