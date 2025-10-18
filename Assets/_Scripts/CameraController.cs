using UnityEngine;

public class CameraController : MonoBehaviour
{
    // --- THAM CHIẾU ---
    public Transform playerBody; // Kéo đối tượng Player vào đây

    // --- CÀI ĐẶT ---
    [Header("Settings")]
    public float mouseSensitivity = 200f;

    // --- BIẾN NỘI BỘ ---
    private float xRotation = 0f;
    private bool isMouseLocked = true;

    void Start()
    {
        // Khóa và ẩn chuột khi bắt đầu game
        LockMouse(true);
    }

    void Update()
    {
        // 1. Lấy input từ chuột
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 2. Xoay người chơi sang trái/phải (xoay quanh trục Y)
        playerBody.Rotate(Vector3.up * mouseX);

        // 3. Xoay camera lên/xuống (xoay quanh trục X)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -40f, 80f); // Giới hạn góc nhìn để không bị lộn ngược

        // Áp dụng góc xoay cho camera
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 4. Xử lý khóa/mở khóa chuột
        if (Input.GetKeyDown(KeyCode.L))
        {
            isMouseLocked = !isMouseLocked;
            LockMouse(isMouseLocked);
        }
    }

    private void LockMouse(bool isLocked)
    {
        if (isLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}