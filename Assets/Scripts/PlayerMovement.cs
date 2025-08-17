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

    // Debug info
    [Header("Debug Info")]
    public bool showDebugInfo = true;
    public float maxFallSpeed = -20f;

    private void Start()
    {
        // Ensure groundCheck is set if not assigned
        if (groundCheck == null)
        {
            // Create a child object for ground checking
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0, -0.9f, 0);
            groundCheck = groundCheckObj.transform;
        }

        // Set ground mask to Default layer (layer 0) if not set
        if (groundMask.value == 0)
        {
            groundMask = LayerMask.GetMask("Default");
        }
    }

    private void Update()
    {
        // Handle input in Update (runs every frame)
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        jumpInput = Input.GetButtonDown("Jump");

        // Ground check with improved detection
        isGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        // Debug ground detection
        if (showDebugInfo)
        {
            Debug.Log($"Ground Check: {isGround}, Position: {groundCheck.position}, Velocity Y: {velocity.y}");
        }
        
        // Reset velocity when grounded
        if(isGround && velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force to keep grounded
        }

        // Handle jump input
        if(jumpInput && isGround)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        
        // Clamp fall speed to prevent excessive falling
        velocity.y = Mathf.Max(velocity.y, maxFallSpeed);

        // Calculate movement - ONLY if on ground
        Vector3 move = Vector3.zero;
        if (isGround) // Chỉ cho phép di chuyển khi đang ở trên mặt đất
        {
            move = transform.right * horizontalInput + transform.forward * verticalInput;
            
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
        }
        else
        {
            // Khi không ở trên mặt đất, tắt animation di chuyển
            animator.SetBool("isMove", false);
            animator.SetFloat("Speed", 0f);
            inputSpeed = 0f;
        }
        
        // Apply movement using CharacterController with Time.deltaTime for smooth movement
        characterController.Move(move * speed * Time.deltaTime);
        characterController.Move(velocity * Time.deltaTime);

        // Handle scene management
        if(gameObject.transform.position.y <= -10)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    // Debug visualization for ground check
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGround ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
            
            // Draw a line from player to ground check
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, groundCheck.position);
        }
    }
}
