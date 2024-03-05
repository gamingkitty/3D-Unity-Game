using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementy : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
 
    private bool groundedPlayer;
    public Camera camera;
    [SerializeField] float playerSpeed = 2.0f;
    [SerializeField] float jumpHeight = 1.0f;
    [SerializeField] float gravityValue = -9.81f;

    private void Start()
    {
        controller = gameObject.AddComponent<CharacterController>();
    }

    void Update() 
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector3 move = new Vector3(0, 0, 0);

        move += camera.transform.forward * Input.GetAxis("Vertical") * playerSpeed * Time.deltaTime;
        move += camera.transform.right * Input.GetAxis("Horizontal") * playerSpeed * Time.deltaTime;

        move.y = 0;

        controller.Move(move);

        // if (move != Vector3.zero)
        // {
        // gameObject.transform.forward = move;
        // }

        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
}
