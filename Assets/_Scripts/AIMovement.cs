using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class AIMovement : MonoBehaviour // Bạn có thể giữ tên AIMovement nếu muốn
{
    // --- CÀI ĐẶT ---
    [Header("Behavior Settings")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 180f; // Tốc độ xoay (độ/giây)
    public float minWalkTime = 3f;
    public float maxWalkTime = 6f;
    public float minWaitTime = 4f;
    public float maxWaitTime = 7f;
    public float gravity = -9.81f;

    // --- THAM CHIẾU ---
    private CharacterController controller;
    private Animator animator;

    // --- BIẾN NỘI BỘ ---
    private Vector3 moveDirection = Vector3.zero;
    private float walkCounter;
    private float waitCounter;
    private bool isWalking = false;

    private readonly int isRunningHash = Animator.StringToHash("isRunning");

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Bắt đầu bằng trạng thái chờ
        waitCounter = Random.Range(minWaitTime, maxWaitTime);
    }

    void Update()
    {
        // Luôn áp dụng trọng lực để AI không bay lơ lửng
        if (!controller.isGrounded)
        {
            controller.Move(new Vector3(0, gravity, 0) * Time.deltaTime);
        }

        if (isWalking)
        {
            // ---- TRẠNG THÁI DI CHUYỂN ----
            walkCounter -= Time.deltaTime;

            // Xoay người mượt mà về hướng di chuyển
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // Di chuyển nhân vật về phía trước
            controller.Move(transform.forward * moveSpeed * Time.deltaTime);

            // Cập nhật animation
            animator.SetBool(isRunningHash, true);

            // Nếu hết giờ đi, chuyển sang trạng thái chờ
            if (walkCounter <= 0)
            {
                isWalking = false;
                waitCounter = Random.Range(minWaitTime, maxWaitTime);
                animator.SetBool(isRunningHash, false);
            }
        }
        else
        {
            // ---- TRẠNG THÁI CHỜ ----
            waitCounter -= Time.deltaTime;

            // Nếu hết giờ chờ, chọn hướng mới và bắt đầu đi
            if (waitCounter <= 0)
            {
                isWalking = true;
                walkCounter = Random.Range(minWalkTime, maxWalkTime);
                ChooseNewDirection();
            }
        }
    }

    // Chọn một trong 4 hướng ngẫu nhiên
    void ChooseNewDirection()
    {
        int direction = Random.Range(0, 4);
        switch (direction)
        {
            case 0: // Tiến
                moveDirection = Vector3.forward;
                break;
            case 1: // Lùi
                moveDirection = Vector3.back;
                break;
            case 2: // Trái
                moveDirection = Vector3.left;
                break;
            case 3: // Phải
                moveDirection = Vector3.right;
                break;
        }
    }
}