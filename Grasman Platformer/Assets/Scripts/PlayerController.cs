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

    float lastKey = 1;
    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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
        else if (Input.GetKey(KeyCode.W))
        {
            playerInput = Vector2.up;
            Debug.Log("jumped!");
        }
        else
        {
            //Return acceleration to zero when nothing is pressed
            acceleration = 0;
        }
        
        MovementUpdate(playerInput);

    }

    private void MovementUpdate(Vector2 playerInput)
    {
        if (playerInput == Vector2.left || playerInput == Vector2.right) //player move
        {
            playerRigidbody.velocity = acceleration * playerInput * Time.deltaTime;
        }
        else if (playerInput == Vector2.up) //player jump
        {
            playerRigidbody.velocity = 300 * Vector2.up * Time.deltaTime;
        }

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
            Debug.Log("Grounded");
            return true;
        }
        else
        {
            Debug.Log("Not grounded");
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
