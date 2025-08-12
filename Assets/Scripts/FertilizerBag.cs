using UnityEngine;

public class FertilizerBag : MonoBehaviour
{
    public Camera cam;
    [Tooltip("Lượng phân mỗi lần bấm.")]
    public float fertilizerAmount = 0.25f;
    public float maxDistance = 20f;

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // chuột phải
        {
            if (!cam) cam = Camera.main;
            if (!cam) return;

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, maxDistance))
            {
                var growth = hit.collider.GetComponentInParent<PlantGrowth>();
                if (growth)
                {
                    growth.ApplyFertilizer(fertilizerAmount);
                    // (Tuỳ chọn) VFX bụi phân:
                    // SpawnFertilizerVFX(hit.point);
                }
            }
        }
    }
}
