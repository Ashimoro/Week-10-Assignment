using System.Collections;
using System.Collections.Generic;
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
    //public CharacterState previousCharacterState = CharacterState.idle;


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




    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        acceleration = maxSpeed / accelerationTime;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(coyoteTime);
        //previousCharacterState = currentCharacterState;

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
                    //We know we need to make a transition because we're not grounded anymore
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
                    Debug.Log("1");
                }
                //Are we jumping?
                if (!IsGrounded())
                {
                    currentCharacterState = CharacterState.jump;
                    Debug.Log("2");
                }
                break;
            case CharacterState.idle:
                //Are we walking?
                if (IsWalking())
                {
                    currentCharacterState = CharacterState.walk;
                    Debug.Log("3");
                }
                //Are we jumping?
                if (!IsGrounded())
                {
                    currentCharacterState = CharacterState.jump;
                    Debug.Log("4");
                }

                break;

        }

        if (collisionExit == true && coyoteTime > 0)
        {
            coyoteTime -= 5*Time.deltaTime;

            if (coyoteTime <= 0)
            {
                onTheGround = false;
            }
        }

        //Debug.Log(currentCharacterState);
    }

    private void FixedUpdate()
    {
        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.
        Vector2 playerInput = new Vector2();
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            playerInput += Vector2.left;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            playerInput += Vector2.right;
        }
        if (isJumping)
        {
            Debug.Log("Player is jumping woohoo!!");
            playerRB.velocity += Vector2.up * jumpForce;
            currentCharacterState = CharacterState.jump;
            isJumping = false;
        }


        MovementUpdate(playerInput);
    }



    private void MovementUpdate(Vector2 playerInput)
    {
        Vector2 velocity = playerRB.velocity;
       // Debug.Log(playerInput.ToString());
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collisionExit = false;
        onTheGround = true;
        coyoteTime = 2;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {

        collisionExit = true;
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
        if (!onTheGround)
        {
            return false;
        }
        return true;
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
}