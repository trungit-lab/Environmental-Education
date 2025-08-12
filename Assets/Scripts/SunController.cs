using UnityEngine;

public class SunController : MonoBehaviour
{
    [Tooltip("Tốc độ quay độ/giây (yaw).")]
    public float rotationSpeed = 5f;
    [Tooltip("Biên độ cường độ ánh sáng (min..max).")]
    public Vector2 intensityRange = new Vector2(0.1f, 1.2f);

    float t;

    void Update()
    {
        // Quay quanh trục Y để giả lập mặt trời di chuyển
        transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime, Space.Self);

        // Đổi intensity nhẹ theo sin
        t += Time.deltaTime * 0.2f;
        var light = GetComponent<Light>();
        if (light)
        {
            float n = (Mathf.Sin(t) * 0.5f + 0.5f); // 0..1
            light.intensity = Mathf.Lerp(intensityRange.x, intensityRange.y, n);
        }
    }
}
