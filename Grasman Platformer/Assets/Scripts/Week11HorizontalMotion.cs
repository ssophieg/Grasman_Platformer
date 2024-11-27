using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Week11HorizontalMotion : MonoBehaviour
{
    public enum FacingDirection
    {
        left, right
    }

    public float timeToReachMaxSpeed;
    public float maxSpeed;
    public float timeToDecelerate;


    private float acceleration;
    private float deceleration;
    private Rigidbody2D playerRB;

    // Start is called before the first frame update
    void Start()
    {
        acceleration = maxSpeed / timeToReachMaxSpeed;
        deceleration = maxSpeed / timeToDecelerate;
        playerRB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.
        Vector2 playerInput = new Vector2();
        MovementUpdate(playerInput);
    }

    private void MovementUpdate(Vector2 playerInput)
    {
        Vector2 currentVelocity = playerRB.velocity;

        //if move the character to the left
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            //do stuff to currentVelocity
            currentVelocity += acceleration * Vector2.left * Time.deltaTime;
        }

        //if the character is not currently accelerating:
        //decelerate


        //if move the character to the right
        //do stuff to currentVelocity

        //if grounded and player jumps 
        //do stuff to currentVelocity

        playerRB.velocity = currentVelocity;

    }

    public bool IsWalking()
    {
        return false;
    }
    public bool IsGrounded()
    {
        return true;
    }

    public FacingDirection GetFacingDirection()
    {
        return FacingDirection.left;
    }
}
