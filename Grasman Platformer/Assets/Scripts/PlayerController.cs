using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public Rigidbody2D playerRigidbody;
    public float acceleration;
    public float maxSpeed = 5f;
    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.
        Vector2 playerInput = new Vector2();
        Vector2 previousInput = new Vector2();

        if (Input.GetKey(KeyCode.A))
        {
            playerInput = Vector2.left;
            previousInput = Vector2.left;

            acceleration += 0.01f;

            if (acceleration >= maxSpeed)
            {
                acceleration = maxSpeed;
            }
        }
        else if (Input.GetKey(KeyCode.D))
        {
            playerInput = Vector2.right;
            previousInput = Vector2.right;

            acceleration += 0.01f;

            if (acceleration >= maxSpeed)
            {
                acceleration = maxSpeed;
            }
        }
        else if (Input.GetKey(KeyCode.W))
        {
            playerInput = Vector2.up;
        }
        else
        {
            acceleration -= 0.01f;
            if (acceleration <= 0)
            {
                acceleration = 0;
            }
        }

        if (acceleration > 0)
        {
            playerInput = previousInput;
        }
        else if (acceleration == 0)
        {
            playerInput = Vector2.zero;
        }
        
        MovementUpdate(playerInput);
    }

    private void MovementUpdate(Vector2 playerInput)
    {

        playerRigidbody.velocity = acceleration * playerInput;
 
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
