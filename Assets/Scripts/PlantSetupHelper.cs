using UnityEngine;

public class PlantSetupHelper : MonoBehaviour
{
    [Header("Setup")]
    public bool autoSetupOnStart = true;
    public bool addHarvesterComponent = true;
    public bool addCollider = true;
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupPlant();
        }
    }
    
    [ContextMenu("Setup Plant")]
    public void SetupPlant()
    {
        // Thêm PlantHarvester component
        if (addHarvesterComponent && GetComponent<PlantHarvester>() == null)
        {
            PlantHarvester harvester = gameObject.AddComponent<PlantHarvester>();
            Debug.Log($"Đã thêm PlantHarvester component cho {gameObject.name}");
        }
        
        // Thêm Collider nếu chưa có
        if (addCollider && GetComponent<Collider>() == null)
        {
            SphereCollider collider = gameObject.AddComponent<SphereCollider>();
            collider.radius = 0.5f;
            Debug.Log($"Đã thêm Collider cho {gameObject.name}");
        }
        
        // Đảm bảo có PlantGrowth component
        if (GetComponent<PlantGrowth>() == null)
        {
            Debug.LogWarning($"Plant {gameObject.name} không có PlantGrowth component!");
        }
    }
    
    // Method để setup tất cả plant trong scene
    [ContextMenu("Setup All Plants")]
    public static void SetupAllPlants()
    {
        PlantGrowth[] plants = FindObjectsOfType<PlantGrowth>();
        
        foreach (PlantGrowth plant in plants)
        {
            PlantSetupHelper helper = plant.GetComponent<PlantSetupHelper>();
            if (helper == null)
            {
                helper = plant.gameObject.AddComponent<PlantSetupHelper>();
            }
            helper.SetupPlant();
        }
        
        Debug.Log($"Đã setup {plants.Length} plants trong scene");
    }
}
