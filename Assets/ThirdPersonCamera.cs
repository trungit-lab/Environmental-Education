using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Tham Chiếu")]
    public Transform target; // Kéo đối tượng Player vào đây
    public Transform cameraPivot; // Một điểm xoay, thường là đầu nhân vật

    [Header("Cài Đặt Camera")]
    [Tooltip("Tốc độ camera đi theo nhân vật")]
    public float followSpeed = 15f;
    [Tooltip("Tốc độ xoay camera bằng chuột")]
    public float rotationSpeed = 3f;
    [Tooltip("Khoảng cách mặc định của camera tới nhân vật")]
    public float defaultDistance = 6f;
    [Tooltip("Khoảng cách tối thiểu khi có vật cản")]
    public float minDistance = 1f;
    [Tooltip("Khoảng cách tối đa khi zoom")]
    public float maxDistance = 10f;

    [Header("Góc Nhìn")]
    public float minVerticalAngle = -45f;
    public float maxVerticalAngle = 80f;

    [Header("Vật Cản")]
    public LayerMask collisionMask; // Layer của những vật cản (tường, đất...)

    // Biến nội bộ
    private float rotationX;
    private float rotationY;
    private float currentDistance;

    void Start()
    {
        // Lấy góc xoay ban đầu
        rotationY = transform.eulerAngles.y;
        rotationX = transform.eulerAngles.x;
        currentDistance = defaultDistance;

        // Khóa và ẩn chuột
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null || cameraPivot == null) return;

        // 1. Lấy input từ chuột để tính toán góc xoay
        rotationY += Input.GetAxis("Mouse X") * rotationSpeed;
        rotationX -= Input.GetAxis("Mouse Y") * rotationSpeed;
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

        // Tạo quaternion cho góc xoay
        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);

        // 2. Xử lý va chạm với vật cản
        // Tính toán vị trí mong muốn của camera
        Vector3 desiredPosition = cameraPivot.position - (rotation * Vector3.forward * defaultDistance);
        RaycastHit hit;

        // Bắn một tia từ điểm xoay tới vị trí camera mong muốn
        if (Physics.Linecast(cameraPivot.position, desiredPosition, out hit, collisionMask))
        {
            // Nếu có vật cản, đặt khoảng cách camera ngay tại điểm va chạm
            currentDistance = Mathf.Clamp(hit.distance * 0.9f, minDistance, maxDistance);
        }
        else
        {
            // Nếu không có vật cản, quay về khoảng cách mặc định
            currentDistance = defaultDistance;
        }

        // 3. Cập nhật vị trí và góc xoay của camera
        Vector3 finalPosition = cameraPivot.position - (rotation * Vector3.forward * currentDistance);

        // Di chuyển camera một cách mượt mà
        transform.position = Vector3.Lerp(transform.position, finalPosition, followSpeed * Time.deltaTime);
        transform.rotation = rotation;
    }
}