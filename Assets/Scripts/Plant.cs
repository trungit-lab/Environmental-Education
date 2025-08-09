using UnityEngine;

[System.Serializable]
public class GrowthStage
{
    public GameObject stagePrefab; // model cây ở stage này
    public float stageDuration;    // thời gian ở stage này (giây)
    public Vector3 startScale = Vector3.one * 0.5f;
    public Vector3 endScale = Vector3.one;
}

[CreateAssetMenu(fileName = "TreeData", menuName = "Environment/Tree Data")]
public class TreeData : ScriptableObject
{
    public string treeName;
    public GrowthStage[] stages;
}

public class Plant : MonoBehaviour, IGrowable
{
    public string plantName;
    public float growthProgress = 0f; // 0 -> 100%
    public float waterLevel = 50f;
    public float sunlightLevel = 50f;
    public float health = 100f;

    public float growthRate = 1f; // tốc độ phát triển cơ bản

    public void Water(float amount)
    {
        waterLevel = Mathf.Clamp(waterLevel + amount, 0, 100);
    }

    public void AddFertilizer(float amount)
    {
        growthRate += amount * 0.1f;
    }

    public void AddSunlight(float hours)
    {
        sunlightLevel = Mathf.Clamp(sunlightLevel + hours * 10, 0, 100);
    }

    public void ApplyPesticide()
    {
        health = Mathf.Min(health + 20, 100);
    }

    public void UpdateGrowth(float deltaTime)
    {
        if (waterLevel > 30 && sunlightLevel > 30 && health > 50)
        {
            growthProgress += growthRate * deltaTime;
            growthProgress = Mathf.Min(growthProgress, 100);
        }
    }
}
