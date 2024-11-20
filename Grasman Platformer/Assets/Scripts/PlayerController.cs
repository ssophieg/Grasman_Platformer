using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{

    public Rigidbody2D playerRigidbody;
    public float acceleration;
    public float maxSpeed = 200f;
    public float accelSpeed = 10f;

    //jump values
    public float apexHeight = 90;
    public float apexTime = 60; //one minute
    public float currTime;
    public Vector2 initialPosition;
    public Vector2 currentVelocity;

    public float initialJumpVelocity;
    public float gravity;

    float lastKey = 1;
    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {

        //jump position formula values calculations
        initialJumpVelocity = 2 * apexHeight / apexTime;
        gravity = -2 * apexHeight / (apexTime * apexTime); 

        //movement values
   
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currTime += 1;

        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.
        Vector2 playerInput = new Vector2();

        //Get player input
        if (Input.GetKey(KeyCode.A))
        {
            playerInput = Vector2.left;

            acceleration += accelSpeed;

            if (acceleration >= maxSpeed)
            {
                acceleration = maxSpeed;
            }
        }
        else if (Input.GetKey(KeyCode.D))
        {
            playerInput = Vector2.right;

            acceleration += accelSpeed;

            if (acceleration >= maxSpeed)
            {
                acceleration = maxSpeed;
            }
        }
        else
        {
            //Return acceleration to zero when nothing is pressed
            acceleration = 0;
        }

        if (Input.GetKey(KeyCode.W))
        {
            currTime = 0;
            initialPosition = playerRigidbody.position; 
            playerInput = Vector2.up;
            Debug.Log("jumped!");
        }

        MovementUpdate(playerInput);

    }

    private void MovementUpdate(Vector2 playerInput)
    {
        currentVelocity = playerRigidbody.velocity;

        if (playerInput == Vector2.left) //player move
        {
            currentVelocity.x = acceleration * -1 * Time.deltaTime;
        }
        else if (playerInput == Vector2.right)
        {
            currentVelocity.x = acceleration * 1 * Time.deltaTime;
        }
        else
        {
            currentVelocity.x = 0;
        }

        if (playerInput == Vector2.up && IsGrounded() == true) //player jump
        {
            currentVelocity.y += (gravity * currTime + initialJumpVelocity);
        }

        
        playerRigidbody.velocity = currentVelocity;
        //Debug.Log(playerRigidbody.velocity.x);
    }

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
    public bool IsGrounded()
    {
        if (Physics2D.Raycast(playerRigidbody.position, Vector2.down, 0.7f))
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

    public FacingDirection GetFacingDirection()
    {
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
}
