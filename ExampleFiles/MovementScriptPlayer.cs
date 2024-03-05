using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScriptPlayer : MonoBehaviour
{
    [SerializeField] CharacterController playerController;
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask playerMask;
    [SerializeField] StaminaBarScript staminaBar;
	[SerializeField] Canvas inventoryCanvas;
	[SerializeField] float gravity = -9.81f;
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] float jumpHeight = 1;
    [SerializeField] float shiftScale = 0.8f;
    [SerializeField] float staminaUsedPerSecond = 10f;
    [SerializeField] float staminaRegenedPerSecond = 5f;
    [SerializeField] float shiftSpeed = 2.5f;
    [SerializeField] float sprintSpeed = 7.5f;
    [SerializeField] float normalSpeed = 5f;
    public float speed = 5f;

    float horizontalInput;
    float verticalInput;
    bool isShifting = false;
    bool isSprinting = false;
    bool isInventoryOpen = false;
    Vector3 velocity;
    // Start is called before the first frame update
    void Start()
    {
        
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

        //Inventory Section
        if (Input.GetKey(KeyCode.Tab))
        {
            isInventoryOpen = !isInventoryOpen;
        }

        if (isInventoryOpen)
        {
            if (inventoryCanvas.hideFlags != HideFlags.None)
            {
                inventoryCanvas.hideFlags = HideFlags.None;
            }
        } 
        else
        {
            if (inventoryCanvas.hideFlags != HideFlags.HideAndDontSave)
            {
                inventoryCanvas.hideFlags = HideFlags.HideAndDontSave;
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
        else
        {
            if (isShifting) 
            {
                playerTransform.localScale = new Vector3(1, 1, 1);
                isShifting = false;
            }
        }

        //Sprint if ctrl is pressed, not shifting, and on ground
        if (Input.GetKey(KeyCode.LeftControl) && !isShifting && isGrounded && staminaBar.stamina >= staminaUsedPerSecond * Time.deltaTime)
        {
            if (!isSprinting)
            {
                isSprinting = true;
                staminaBar.isVisible = true;
            }
            staminaBar.stamina -= staminaUsedPerSecond * Time.deltaTime;
            speed = sprintSpeed;
        }
        //If not pressing ctrl stop sprinting
        else
        {
            if (isSprinting)
            {
                staminaBar.isVisible = false;
                isSprinting = false;
            }
        }

        //If not sprinting and stamina not full regen stamina
        if (!isSprinting && staminaBar.stamina < staminaBar.maxStamina && !Input.GetKey(KeyCode.LeftControl) || isShifting || !isGrounded)
        {
            staminaBar.stamina += staminaRegenedPerSecond * Time.deltaTime;
            //Set stamina to max stamina if over max stamina
            if (staminaBar.stamina > staminaBar.maxStamina)
            {
                staminaBar.stamina = staminaBar.maxStamina;
            }
        }

        //Setting stamina to 0 if its less than 0
        if (staminaBar.stamina < 0)
        {
            staminaBar.stamina = 0;
        }

        //Speed section end

        //Applying gravity
        velocity.y += gravity * Time.deltaTime;

        //Get input from the game
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        //Setting input to a vector3
        Vector3 move = transform.right * horizontalInput + transform.forward * verticalInput;
        
        //Player jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        //Moving the player based on the vector3
        playerController.Move(move * speed * Time.deltaTime);

        //Moving the player based on previously calculated velocity
        playerController.Move(velocity * Time.deltaTime);
    }
}
