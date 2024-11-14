using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{

    public Rigidbody2D playerRigidbody;
    public float acceleration;
    public float maxSpeed = 200f;
    public float accelSpeed = 10f;
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
            acceleration = 0;
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            acceleration = 0;
        }
        
        MovementUpdate(playerInput);

    }

    private void MovementUpdate(Vector2 playerInput)
    {
        if (playerInput == Vector2.left || playerInput == Vector2.right)
        {
            playerRigidbody.velocity = acceleration * playerInput * Time.deltaTime;
        }
        else if (playerInput == Vector2.up)
        {
            playerRigidbody.velocity = 450 * Vector2.up * Time.deltaTime;
        }

        //playerRigidbody.velocity = acceleration * playerInput * Time.deltaTime;
    }

    public bool IsWalking()
    {
        return false;
    }
    public bool IsGrounded()
    {
        return false;
    }

    public FacingDirection GetFacingDirection()
    {
        return FacingDirection.left;
    }
}
