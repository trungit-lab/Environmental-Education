using UnityEngine;
using UnityEngine.UI;

public class TreeGrowth : MonoBehaviour
{
    [Header("Tree Models by Stage (Small -> Medium -> Large)")]
    public GameObject[] growthStages; // 0 = nhỏ, 1 = vừa, 2 = lớn

    [Header("Growth Settings")]
    public float totalGrowthTime = 60f; // Thời gian từ 0% -> 100% phát triển
    public float growthSpeedMultiplier = 1f; // Hệ số tăng tốc khi được chăm sóc
    public float weatherMultiplier = 1f; // Ảnh hưởng bởi thời tiết

    [Header("UI")]
    public Slider growthSlider;

    [Header("Animal Rescue")]
    public GameObject animalPrefab;
    public Transform animalSpawnPoint;
    public int spawnStageIndex = 1; // Stage xuất hiện động vật

    private float currentGrowth = 0f; // 0 -> totalGrowthTime
    private int currentStage = 0;
    private bool animalSpawned = false;

    void Start()
    {
        // Khởi tạo stage ban đầu
        for (int i = 0; i < growthStages.Length; i++)
        {
            growthStages[i].SetActive(i == 0);
        }
        UpdateUI();
    }

    void Update()
    {
        // Tăng trưởng
        currentGrowth += Time.deltaTime * growthSpeedMultiplier * weatherMultiplier;
        float progress = currentGrowth / totalGrowthTime;
        UpdateUI();

        // Cập nhật scale từ nhỏ -> full size stage hiện tại
        if (currentStage < growthStages.Length)
        {
            float stageProgress = Mathf.Clamp01((progress - (float)currentStage / growthStages.Length) * growthStages.Length);
            growthStages[currentStage].transform.localScale = Vector3.Lerp(Vector3.one * 0.5f, Vector3.one, stageProgress);
        }

        // Kiểm tra đổi stage
        int newStage = Mathf.FloorToInt(progress * growthStages.Length);
        if (newStage != currentStage && newStage < growthStages.Length)
        {
            growthStages[currentStage].SetActive(false);
            currentStage = newStage;
            growthStages[currentStage].SetActive(true);

            // Spawn động vật nếu đạt stage yêu cầu
            if (currentStage == spawnStageIndex && !animalSpawned)
            {
                SpawnAnimal();
                animalSpawned = true;
            }
        }
    }

    void UpdateUI()
    {
        if (growthSlider != null)
        {
            growthSlider.value = currentGrowth / totalGrowthTime;
        }
    }

    // ===== Các hành động chăm sóc cây =====
    public void WaterPlant()
    {
        growthSpeedMultiplier += 0.2f;
    }

    public void GiveSunlight()
    {
        growthSpeedMultiplier += 0.1f;
    }

    public void SprayPesticide()
    {
        growthSpeedMultiplier += 0.15f;
    }

    // ===== Hệ thống thời tiết =====
    public void SetSunny()
    {
        weatherMultiplier = 0.8f; // Nắng hạn giảm tốc
    }

    public void SetRainy()
    {
        weatherMultiplier = 1.3f; // Mưa tăng tốc
    }

    // ===== Event cứu động vật =====
    void SpawnAnimal()
    {
        if (animalPrefab != null && animalSpawnPoint != null)
        {
            Instantiate(animalPrefab, animalSpawnPoint.position, Quaternion.identity);
        }
    }
}
