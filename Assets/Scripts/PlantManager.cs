using UnityEngine;
using System.Collections.Generic;

public class PlantManager : MonoBehaviour
{
    public static PlantManager Instance;
    public List<GameObject> plants;
    
    [Header("Plant Tracking")]
    public Dictionary<PlantType, int> plantCounts = new Dictionary<PlantType, int>();
    public Dictionary<PlantType, int> plantedCounts = new Dictionary<PlantType, int>();

    public int currentPlantIndex = 0;

    private void Awake()
    {
        Instance = this;
        InitializePlantCounts();
    }

    private void InitializePlantCounts()
    {
        // Khởi tạo số lượng ban đầu cho mỗi loại plant
        plantCounts[PlantType.TreeA] = 10;
        plantCounts[PlantType.TreeB] = 10;
        plantCounts[PlantType.TreeC] = 10;
        plantCounts[PlantType.TreeD] = 10;

        // Khởi tạo số lượng đã trồng
        plantedCounts[PlantType.TreeA] = 0;
        plantedCounts[PlantType.TreeB] = 0;
        plantedCounts[PlantType.TreeC] = 0;
        plantedCounts[PlantType.TreeD] = 0;
    }

    public void PlantATree(Vector3 position)
    {
        if (currentPlantIndex >= plants.Count) return;
        
        PlantType plantType = GetPlantTypeFromIndex(currentPlantIndex);
        
        // Kiểm tra xem còn hạt giống không
        if (plantCounts[plantType] <= 0)
        {
            Debug.Log($"Hết hạt giống {plantType}!");
            return;
        }

        Instantiate(plants[currentPlantIndex], position, Quaternion.identity);
        
        // Giảm số lượng hạt giống
        plantCounts[plantType]--;
        
        // Tăng số lượng đã trồng
        plantedCounts[plantType]++;
        
        currentPlantIndex++;
        if (currentPlantIndex >= plants.Count)
        {
            currentPlantIndex = 0;
        }
        
        // Thông báo UI cập nhật
        UpdatePlantUI();
    }

    public void DestroyPlant(PlantGrowth plant)
    {
        if (plant == null || plant.definition == null) return;
        
        PlantType plantType = plant.definition.type;
        
        // Giảm số lượng đã trồng
        if (plantedCounts.ContainsKey(plantType))
        {
            plantedCounts[plantType]--;
        }
        
        // Destroy plant
        Destroy(plant.gameObject);
        
        // Thông báo UI cập nhật
        UpdatePlantUI();
        
        Debug.Log($"Đã thu hoạch {plantType}! Số lượng còn lại: {plantedCounts[plantType]}");
    }

    private PlantType GetPlantTypeFromIndex(int index)
    {
        // Map index sang PlantType (cần điều chỉnh theo thứ tự trong plants list)
        switch (index)
        {
            case 0: return PlantType.TreeA;
            case 1: return PlantType.TreeB;
            case 2: return PlantType.TreeC;
            case 3: return PlantType.TreeD;
            default: return PlantType.TreeA;
        }
    }

    private void UpdatePlantUI()
    {
        // Tìm PlantListUI và cập nhật
        PlantListUI plantListUI = FindObjectOfType<PlantListUI>();
        if (plantListUI != null)
        {
            plantListUI.UpdatePlantCounts();
        }
    }

    public int GetPlantCount(PlantType plantType)
    {
        return plantCounts.ContainsKey(plantType) ? plantCounts[plantType] : 0;
    }

    public int GetPlantedCount(PlantType plantType)
    {
        return plantedCounts.ContainsKey(plantType) ? plantedCounts[plantType] : 0;
    }
}
