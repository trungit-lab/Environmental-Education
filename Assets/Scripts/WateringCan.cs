using UnityEngine;

public class WateringCan : MonoBehaviour
{
    public Camera cam;
    [Tooltip("Lượng nước mỗi lần bấm.")]
    public float waterAmount = 0.3f;
    [Tooltip("Khoảng cách raycast.")]
    public float maxDistance = 20f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // chuột trái
        {
            if (!cam) cam = Camera.main;
            if (!cam) return;

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, maxDistance))
            {
                var growth = hit.collider.GetComponentInParent<PlantGrowth>();
                if (growth)
                {
                    growth.ApplyWater(waterAmount);
                    // (Tuỳ chọn) VFX giọt nước:
                    // SpawnWaterVFX(hit.point);
                }
            }
        }
    }
}
