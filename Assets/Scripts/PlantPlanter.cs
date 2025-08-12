using UnityEngine;

public class PlantPlanter : MonoBehaviour
{
    public PlantDefinition plantDefinition;
    public float spawnHeightOffset = 0.02f;
    public float initialMoisture = 0.5f;
    public float initialNutrients = 0.2f;
    public bool destroyOnPlant = true;

    void OnCollisionEnter(Collision collision)
    {
        if (!plantDefinition) return;
        if (!collision.collider.CompareTag("Ground")) return;

        ContactPoint cp = collision.contacts[0];
        Vector3 pos = cp.point + cp.normal * spawnHeightOffset;
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, cp.normal);

        GameObject plantRoot = new GameObject($"Plant_{plantDefinition.type}");
        plantRoot.transform.SetPositionAndRotation(pos, rot);

        var growth = plantRoot.AddComponent<PlantGrowth>();
        // Cấu hình NGAY lập tức
        growth.Configure(plantDefinition, initialMoisture, initialNutrients, null);

        if (destroyOnPlant) Destroy(gameObject);
    }
}
