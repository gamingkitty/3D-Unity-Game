using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScriptPlayer : MonoBehaviour
{
    [SerializeField] CharacterController playerController;
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask playerMask;
    [SerializeField] Camera playerCamera;
    [SerializeField] int normalFOV = 60;
    [SerializeField] float FOVMutliplier = 2f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] float jumpHeight = 1;
    [SerializeField] float shiftScale = 0.8f;
    [SerializeField] float shiftSpeed = 5.0f;
    [SerializeField] float normalSpeed = 10f;
    [SerializeField] float airSpeed = 1.0f;
    [SerializeField] float groundFriction = 0.9f;
    [SerializeField] float airFriction = 0.95f;
    [SerializeField] float groundShiftFriction = 0.8f;
    [SerializeField] float airShiftFriction = 0.85f;
    [SerializeField] float sprintSpeed = 16f;
    [SerializeField] float FOVAnimationSpeed = 120f;
    public float speed = 10f;
    float horizontalInput;
    float verticalInput;
    bool isShifting = false;
    bool isSprinting = false;
    int desiredFOV = 0;
    float currentFOV = 0f;
    Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        desiredFOV = normalFOV;
        currentFOV = normalFOV;
    }

    // Update is called once per frame
    void Update()
    {
        //Check if on ground
        bool isGrounded = false;
        if (Physics.CheckSphere(groundCheck.position, groundCheckRadius, playerMask))
        {
            isGrounded = true;
            //Check if not jumping, and keeping on ground if so
            if (velocity.y < 0)
            {
                velocity.y = -2f;
            }
        }

        //Speed section, changing speed based on sprinting and crouching etc

        //Setting speed to normal speed as default
        speed = normalSpeed;

        //Check if pressing left shift and crouching
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (!isShifting)
            {
                playerTransform.localScale = new Vector3(1, playerTransform.localScale.y * shiftScale, 1);
                playerTransform.localPosition = new Vector3(playerTransform.localPosition.x, playerTransform.localPosition.y * shiftScale, playerTransform.localPosition.z);
                isShifting = true;
            }
            speed = shiftSpeed;
        }
        //Uncrouching if not pressing left shift
        else if (isShifting)
        {
            playerTransform.localScale = new Vector3(1, 1, 1);
            isShifting = false;
        }
        if (!isGrounded)
        {
            speed = airSpeed;
        }
        //Check for sprint
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (!isSprinting)
            {
                isSprinting = true;
            }
            if (!isShifting) speed = sprintSpeed;
            else speed = normalSpeed;
        }
        else if (isSprinting)
        {
            isSprinting = false;
        }
        //Speed section end
        //FOV handler for Sprint
        if (currentFOV < desiredFOV) currentFOV += FOVAnimationSpeed * Time.deltaTime;
        else if (currentFOV > desiredFOV) currentFOV -= FOVAnimationSpeed * Time.deltaTime;
        playerCamera.fieldOfView = (int)currentFOV;
        //Applying gravity
        velocity.y += gravity * Time.deltaTime;

        //Get input from the game
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        //Setting input to a vector3
        Vector3 move = playerTransform.right * horizontalInput + playerTransform.forward * verticalInput;
        velocity += move * speed * Time.deltaTime;
        if (!isShifting)
        {
            if (isGrounded)
            {
                velocity.x *= (float)Math.Pow(groundFriction, Time.deltaTime * 10);
                velocity.z *= (float)Math.Pow(groundFriction, Time.deltaTime * 10);
            }
            else
            {
                velocity.x *= (float)Math.Pow(airFriction, Time.deltaTime * 10);
                velocity.z *= (float)Math.Pow(airFriction, Time.deltaTime * 10);
            }
        }
        else
        {
            if (isGrounded)
            {
                velocity.x *= (float)Math.Pow(groundShiftFriction, Time.deltaTime * 10);
                velocity.z *= (float)Math.Pow(groundShiftFriction, Time.deltaTime * 10);
            }
            else
            {
                velocity.x *= (float)Math.Pow(airShiftFriction, Time.deltaTime * 10);
                velocity.z *= (float)Math.Pow(airShiftFriction, Time.deltaTime * 10);
            }
        }

        //Player jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
        float avgSpeed = Math.Abs(velocity.x + velocity.z) / 2;
        avgSpeed = Math.Max((float)Math.Log(avgSpeed), 0);
        desiredFOV = (int)(normalFOV + avgSpeed * FOVMutliplier);

        //Moving the player based on the vector3
        //playerController.Move(move * speed * Time.deltaTime);

        //Moving the player based on previously calculated velocity
        playerController.Move(velocity * Time.deltaTime);
    }
}
