using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerMovement : MonoBehaviour
{
    public CharacterController characterController;
    public float speed = 5f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private Vector3 velocity;
    private bool isGround;

    public Animator animator;

    // Input variables to store values from Update
    private float horizontalInput;
    private float verticalInput;
    private bool jumpInput;
    
    // Speed tracking for animation
    private float inputSpeed;

    private void Update()
    {
        // Handle input in Update (runs every frame)
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        jumpInput = Input.GetButtonDown("Jump");

        // Calculate input-based speed for animation
        Vector3 moveInput = new Vector3(horizontalInput, 0, verticalInput);
        inputSpeed = moveInput.magnitude * speed; // Tốc độ dựa trên input

        // Handle animation state changes
        if(moveInput.magnitude > 0)
        {
            animator.SetBool("isMove", true);
            // Set Speed parameter for Run/Walk animation based on input
            animator.SetFloat("Speed", inputSpeed);
        }
        else
        {
            animator.SetBool("isMove", false);
            animator.SetFloat("Speed", 0f);
        }

        // Handle scene management
        if(gameObject.transform.position.y <= -10)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void FixedUpdate()
    {
        // Physics-based operations in FixedUpdate (runs at fixed time intervals)
        
        // Ground check
        isGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        if(isGround && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Handle jump input
        if(jumpInput && isGround)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.fixedDeltaTime;

        // Calculate movement
        Vector3 move = transform.right * horizontalInput + transform.forward * verticalInput;
        
        // Apply movement using CharacterController (physics-based)
        characterController.Move(move * speed * Time.fixedDeltaTime);
        characterController.Move(velocity * Time.fixedDeltaTime);
    }
}
