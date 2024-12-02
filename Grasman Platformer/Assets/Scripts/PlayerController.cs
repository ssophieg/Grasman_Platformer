using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //collectables UI
    public Text collectablesFound;
    public float coins;

    //player's rigidbody
    public Rigidbody2D playerRigidbody;

    //speed values
    public float acceleration;
    public float maxSpeed = 7f;
    public float accelSpeed = 1f;

    //jump values
    public float apexHeight = 4;
    public float apexTime = 60; //one minute
    public Vector2 currentVelocity;

    public float initialJumpVelocity;
    public float gravity;

    public float terminalSpeed = -6;

    //last horizontal key pressed (to determine player direction)
    float lastKey = 1;

    //coyote time timer

    public float coyoteTimer = 1;

    //player health
    public int health = 10;

    Vector2 playerInput = new Vector2();

    //MOVEMENT BOOLS
    public bool jumped = false;
    public bool movedLeft = false;
    public bool movedRight = false;

    //DASH BOOLS
    public bool dashLeft = false;
    public bool dashRight = false;

    //dash timer
    public float dashTime = 5;
    public float maxDashTime = 5;
    public float dashSpeed = 600;

    //acceleration before the dash
    public float previousAcceleration;

    //double jump availability
    public bool doubleJump = false;
    public bool doubleJumpAvailable = false;

    //wall jump availability
    public float wallJumpAvailable = 0.5f;

    public enum FacingDirection
    {
        left, right
    }

    public enum CharacterState
    {
        idle, jump, walk, die
    }

    public CharacterState currentCharacterState = CharacterState.idle;
    public CharacterState previousCharacterState = CharacterState.idle;

    // Start is called before the first frame update
    void Start()
    {

        //jump position formula values calculations
        initialJumpVelocity = 2 * apexHeight / apexTime;
        gravity = -2 * apexHeight / (apexTime * apexTime);

        //set double jump to false in order to prevent jumping upon startup
        doubleJump = false;
   
    }

    private void Update()
    {

        previousCharacterState = currentCharacterState;

        //set double jump to available if grounded or attatched to any walls
        if (IsGrounded() == true || AgainstWall() == true)
        {
            doubleJumpAvailable = true;
        }

        //Get player dash input

        //Left dash
        if(Input.GetKeyDown(KeyCode.Q) && Input.GetKey(KeyCode.A) && dashTime == maxDashTime) 
        {
            dashLeft = true;
            previousAcceleration = acceleration;
        }

        //Right dash
        if (Input.GetKeyDown(KeyCode.Q) && Input.GetKey(KeyCode.D) && dashTime == maxDashTime)
        {
            dashRight = true;
            previousAcceleration = acceleration;
        }

        //dash timer reset

        if (dashLeft == false && dashRight == false)
        {
            dashTime = maxDashTime;
        }
        else if (dashLeft == true || dashRight == true)
        {
            dashTime -= 0.1f;
        }

        //stop dash when timer ends
        if (dashTime <= 0)
        {
            dashLeft = false;
            dashRight = false;
            acceleration = previousAcceleration;
        }

        //Get player jump input
        if (Input.GetKeyDown(KeyCode.W) && coyoteTimer >= 0 && dashTime == maxDashTime)
        {
            jumped = true;
            wallJumpAvailable = 0.5f;
        }

        //set wall jump to available if attached to wall
        if (AgainstWall() == true && jumped)
        {

            wallJumpAvailable -= 0.01f;

            if (wallJumpAvailable <= 0)
            {
                jumped = false;
            }
            
        }

        //Get player double jump input
        if (Input.GetKeyDown(KeyCode.W) && dashTime == maxDashTime && IsGrounded() == false && AgainstWall() == false)
        {
            doubleJump = true;
        }

        //Get player horizontal input
        if (Input.GetKey(KeyCode.A) && dashTime == maxDashTime)
        {
            movedLeft = true;
            movedRight = false;

            acceleration += accelSpeed;

        }
        else if (Input.GetKey(KeyCode.D) && dashTime == maxDashTime)
        {
            movedRight = true;
            movedLeft = false;

            acceleration += accelSpeed;

        }
        else
        {
            //Return acceleration to zero when nothing is pressed
            acceleration = 0;
            movedRight = false;
            movedLeft = false;
        }

        //animation character states - set states depending on current state and whether or not the player
        //is jumping, idle, walking, or on the gorund
        switch (currentCharacterState)
        {
            case CharacterState.die:
                //we dead ):
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

                //is walking?

                if (IsWalking())
                {
                    currentCharacterState = CharacterState.walk;
                }

                //is jumping?
                if (!IsGrounded())
                {
                    currentCharacterState = CharacterState.jump;
                }
                break;

        }

        //if player health is zero or less
        if (IsDead()) 
        {
            currentCharacterState = CharacterState.die;
        }


    }
    // Update is called once per frame
    void FixedUpdate()
    {        

        //set coyote timer to full if player is still grounded
        if (IsGrounded() == true)
        {
            coyoteTimer = 1f;
        }
        else if (IsGrounded() == false) 
        {
            //start counting down when not grounded
            coyoteTimer -= 0.1f;
        }

        //Debug.Log(coyoteTimer);
        MovementUpdate(playerInput);

    }

    //ALL MOVEMENT IS SET HERE BASED ON BOOLEANS THAT ARE TURNED TO TRUE BASED ON PLAYER'S INPUT

    //PLAYER'S INPUT AND BOOL CHANGES ARE DONE IN UPDATES
    private void MovementUpdate(Vector2 playerInput)
    {
        currentVelocity = playerRigidbody.velocity;

        //player jump 
        if (jumped && coyoteTimer >= 0 && AgainstWall() == false)
        {
            currentVelocity.y += (initialJumpVelocity);

            //coyote timer no longer allowed
            coyoteTimer = -1;

            //jumped bool set to false
            jumped = false;
        }

        if (movedLeft) //player move
        {
            //left movement
            currentVelocity.x += acceleration * -1 * Time.deltaTime;
        }
        else if (movedRight)
        {
            //right movement
            currentVelocity.x += acceleration * 1 * Time.deltaTime;
        }
        else
        {
            //idle
            currentVelocity.x = 0;
        }

        //set max speed - player velocity cannot go over this value
        if(currentVelocity.x >= maxSpeed)
        {
            currentVelocity.x = maxSpeed;
        }
        else if(currentVelocity.x <= -maxSpeed)
        {
            currentVelocity.x = -maxSpeed;
        }

        //left dash 
        if (dashLeft == true && dashTime > 0)
        {
            //set y axis velocity to zero for duration of dash
            currentVelocity.y = 0;
            playerRigidbody.velocity = currentVelocity;
            currentVelocity.x += dashSpeed * -1 * Time.deltaTime;
        }

        //right dash
        if (dashRight == true && dashTime > 0)
        {
            currentVelocity.y = 0;
            playerRigidbody.velocity = currentVelocity;
            currentVelocity.x += dashSpeed * 1 * Time.deltaTime;
        }

        //double jump
        if (doubleJump == true && doubleJumpAvailable == true)
        {
            doubleJumpAvailable = false;
            doubleJump = false;

            //set y axis velocity to zero to prevent negative velocity carrying over to double jump
            currentVelocity.y = 0;
            currentVelocity.y += (initialJumpVelocity);

            coyoteTimer = -1;
        } 

        //apply gravity
        if (AgainstWall() == false)
        {
            //apply gravity as normal when not against wall
            playerRigidbody.velocity = currentVelocity + gravity * Vector2.up * Time.deltaTime;
        }
        else
        {
            //slowly increment player velocity downwards when attached to wall to make them slide down
            currentVelocity.y -= 0.01f;
            playerRigidbody.velocity = currentVelocity;

            //set coyote timer above zero so player may jump
            coyoteTimer = 1;
        }

        //set terminal falling speed - player cannot fall faster than terminalSpeed
        if (playerRigidbody.velocity.y <= terminalSpeed)
        {
            playerRigidbody.velocity = new Vector2(currentVelocity.x + gravity * Time.deltaTime, terminalSpeed);
        } 
        //Debug.Log(playerRigidbody.velocity.y);

    }

    //test if the player is attached to a wall on either side
    public bool AgainstWall()
    {
        if (Physics2D.Raycast(playerRigidbody.position, Vector2.left, 0.65f) || (Physics2D.Raycast(playerRigidbody.position, Vector2.right, 0.65f)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //test if the player is currently walking
    public bool IsWalking()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //test if the player is on the ground
    public bool IsGrounded()
    {
        if (Physics2D.Raycast(playerRigidbody.position - new Vector2 (-0.3f, 0), Vector2.down, 0.7f) ||
            Physics2D.Raycast(playerRigidbody.position - new Vector2(0.3f, 0), Vector2.down, 0.7f))
        {
            //Debug.Log("Grounded");
            return true;
        }
        else
        {
            //Debug.Log("Not grounded");
            return false;
        }
    }

    //test if the player is dead
    public bool IsDead()
    {
        return health <= 0;
    }

    //set player object inactive once player death animation is complete
    public void OnDeathAnimationComplete()
    {
        gameObject.SetActive(false);
    }

    //test which direction the player is facing
    public FacingDirection GetFacingDirection()
    {
        //direction is returned depending on the last horizontal key pressed by the player
        if (Input.GetKey(KeyCode.A))
        {
            lastKey = 1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            lastKey = 2;
        }

        if (lastKey == 1)
        {
            return FacingDirection.left;
        }
        else
        {
            return FacingDirection.right;
        }
    }

    //detect if a coin is picked up
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Coin")
        {
            //add one to the total amount of coins found and set the text UI counter to this value
            coins += 1;
            collectablesFound.text = "" + (coins);
        }
    }

}
