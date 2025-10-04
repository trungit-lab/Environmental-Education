using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    // --- THAM CHIẾU ---
    private CharacterController controller;
    private Animator animator;
    public Transform cameraTransform;

    // --- CÀI ĐẶT TỐC ĐỘ ---
    [Header("Movement Speeds")]
    public float walkSpeed = 4f;    // Tốc độ đi bộ
    public float runSpeed = 8f;     // Tốc độ chạy (mặc định)
    public float sprintSpeed = 12f; // Tốc độ chạy nước rút

    [Header("Rotation")]
    public float rotationSpeed = 720f;

    [Header("Jump and Gravity")]
    public float jumpHeight = 3f;
    public float gravity = -19.62f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    // --- BIẾN NỘI BỘ ---
    private Vector3 velocity;
    private bool isGrounded;

    // --- HASH ANIMATOR ---
    private readonly int moveXHash = Animator.StringToHash("MoveX");
    private readonly int moveYHash = Animator.StringToHash("MoveY");
    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int jumpHash = Animator.StringToHash("Jump");

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        if (cameraTransform == null) cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        HandleGravityAndGroundCheck();
        HandleMovementAndAnimation(); // Hàm này đã được viết lại hoàn toàn
        HandleJump();
        HandleFallRespawn();
    }

    private void HandleGravityAndGroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // HÀM CHÍNH ĐỂ XỬ LÝ DI CHUYỂN VÀ ANIMATION
    private void HandleMovementAndAnimation()
    {
        // 1. LẤY INPUT
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical);

        // 2. XÁC ĐỊNH TRẠNG THÁI (WALK/RUN/SPRINT) VÀ TỐC ĐỘ MONG MUỐN
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = runSpeed; // Mặc định là chạy
        float targetAnimSpeed = 0.5f;  // Tương ứng với ngưỡng "Run" trong Blend Tree của bạn

        // Nếu có input di chuyển
        if (inputDirection.magnitude >= 0.1f)
        {
            if (isSprinting)
            {
                currentSpeed = sprintSpeed;
                targetAnimSpeed = 1.0f; // Tương ứng với ngưỡng "Sprint"
            }
            // (Bạn có thể thêm logic cho đi bộ ở đây nếu muốn, ví dụ: giữ phím Ctrl)
            // else if (isWalking) { currentSpeed = walkSpeed; targetAnimSpeed = 0.25f; }
        }
        else // Nếu không có input
        {
            targetAnimSpeed = 0f; // Đứng yên
        }

        // 3. CẬP NHẬT TẤT CẢ CÁC THAM SỐ ANIMATOR
        // Cập nhật Speed cho các SubTree 1D (Walk/Run/Sprint)
        animator.SetFloat(speedHash, targetAnimSpeed, 0.1f, Time.deltaTime);

        // Cập nhật MoveX và MoveY cho LocomotionTree 2D
        // Cực kỳ quan trọng: gửi giá trị input thô (raw) vào đây
        animator.SetFloat(moveXHash, horizontal, 0.1f, Time.deltaTime);
        animator.SetFloat(moveYHash, vertical, 0.1f, Time.deltaTime);

        // 4. XOAY VÀ DI CHUYỂN NHÂN VẬT (chỉ khi có input)
        if (inputDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

            // Di chuyển nhân vật với tốc độ vật lý đã xác định (currentSpeed)
            controller.Move(transform.forward * currentSpeed * Time.deltaTime);
        }
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger(jumpHash);
        }
    }

    private void HandleFallRespawn()
    {
        if (transform.position.y <= -10)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}