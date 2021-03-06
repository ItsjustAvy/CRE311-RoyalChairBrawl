﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class TSC_MoveTest : MonoBehaviour
{
    #region variables
    //Component
    [SerializeField] CharacterController controller; //[SerializeField] basically means you can see the variable and mess with it in the editor during gameplay,
    Animator anim;
    //you could also put a "public" before the variable type ("public float floatName" for example), but making lots of variables  public is generally considered bad form and you should only do that for variables that needs to be accessed by other scripts!

    //Boolean
    [SerializeField] private bool grounded = false;
    bool movementLock = false; //might be needed later

    //Float
    [SerializeField] float jumpStrength = 80f;
    [SerializeField] float speed = 20f; //how fast the character moves, the smoothing of the movement is handled by the "Gravity" and "Sensitivity" settings in the Unity Input manager
    float gravity = -35f; 
    float velocityDamp = 6f; //velocityDamp is the rate at which the forces applied to the player revert to their original values
    float InputX; //Left-Right input
    float InputMagnitude;
    //Vector3
    Vector3 movement; //the movement vector3
    Vector3 velocity; //basically all the gravity-based stuff affecting the player, velocity is added on top of the player movement vector3
    #endregion

    #region Initialization
    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>(); //Just assigns the variable "controller" to be the CharacterController attached to this gameObject.
        anim = gameObject.GetComponent<Animator>();
    }
    #endregion

    
    // Update is called once per frame
    void Update()
    {
       // anim.SetFloat("InputZ", InputZ, 0.0f, Time.deltaTime);
        anim.SetFloat("InputX", InputX, 0.0f, Time.deltaTime);
        anim.SetFloat("InputMagnitude", InputMagnitude, 0.0f, Time.deltaTime);
        InputX = Input.GetAxis("Horizontal");
        //InputY = Input.GetAxis("Vertical"); not needed right now
        if (!movementLock) //We might need to lock movement for dialogue or end-of-round stuff, so I just put this here as a precaution.
        {
            Movement();
        }
        CharacterPhysics(); //since this is called without any if statement requirements on update, CharacterPhysics is basically a glorified Update function at this point :p

        #region Grounding
        //for some reason directly checking the controller.isGrounded variable is really unreliable, so this just sets a bool to match the variable, which somehow works better idk
        if (controller.isGrounded)
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
        #endregion


        #region Jump Height Control
        //This section makes it so tapping and holding jump makes the player do a short hop or a much bigger jump
        if (Input.GetButton("Jump")) //the "big" jump script
        {
            if(velocity.y < 0)
            {
                velocityDamp = 4f; 
            }
            else if (velocity.y > 0 ){
                velocityDamp = 3f;
            }           
        }
        if (!Input.GetButton("Jump")) //the "hop" jump script
        {    
            velocityDamp = 6f;
        }
        #endregion
        float singleStep = speed * Time.deltaTime;

       // Vector3 targetRotation = transform.position + new Vector3(-InputX, 0f, 0f);
      //  Vector3 newRotation = Vector3.RotateTowards(gameObject.transform.position, targetRotation , 600f, 5f);

   

       // Debug.Log(newRotation);

        //transform.rotation = Quaternion.LookRotation(newRotation);
    }

    void Movement() //Guess what this one does.
    {
        if (controller != null)//make sure the script actually found a CharacterController on the gameobject
        {

            movement = new Vector3(-InputX, 0, 0);
            controller.Move(movement * speed * Time.deltaTime); //The left-right movement of the player is handled here.
            InputMagnitude = movement.magnitude;
            if (movement.magnitude > 0) { 
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15F);
            }

            if (Input.GetButtonDown("Jump"))  //This checks if the player has pressed the jump button!
            {
                  Jump(); //calls the Jump script which adds the jump velocity to the player 
            }    
        }
    }
    void CharacterPhysics()//this handles all the gravitational stuff affecting player, also jumping just adds a big burst of upwards momentum to the player
    {
        
        velocity = Vector3.Lerp(velocity, new Vector3(0, gravity, 0) , velocityDamp * Time.deltaTime);
        controller.Move(velocity * Time.deltaTime); 
    }

    void Jump() //Jumps.
    {
        if (grounded == true) //Check if grounded first so the player can't just quadruple jump into space, but we could perhaps have double-jumping  characters later down the line!
        { 
        velocity = velocity + new Vector3(0, jumpStrength, 0); //simply adds the force of "JumpStrength" to the velocity, making the player "jump" upwards
        }
       
    }

}
