using UnityEngine;
using UnityEngine.UI;

public class PlantUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject plantInfoPanel;
    public Slider progressSlider;
    public Slider moistureSlider;
    public Slider nutrientsSlider;
    public Text plantNameText;
    public Text stageText;
    
    [Header("Settings")]
    public float updateInterval = 0.5f;
    
    private PlantGrowth currentPlant;
    private float updateTimer;
    
    void Start()
    {
        if (plantInfoPanel)
            plantInfoPanel.SetActive(false);
    }
    
    void Update()
    {
        updateTimer += Time.deltaTime;
        
        if (updateTimer >= updateInterval)
        {
            updateTimer = 0f;
            UpdateUI();
        }
    }
    
    public void SetTargetPlant(PlantGrowth plant)
    {
        currentPlant = plant;
        
        if (plant && plantInfoPanel)
        {
            plantInfoPanel.SetActive(true);
            UpdateUI();
        }
        else if (plantInfoPanel)
        {
            plantInfoPanel.SetActive(false);
        }
    }
    
    void UpdateUI()
    {
        if (!currentPlant || !plantInfoPanel.activeSelf) return;
        
        // Cập nhật progress
        if (progressSlider)
        {
            progressSlider.value = currentPlant.progress;
        }
        
        // Cập nhật moisture
        if (moistureSlider)
        {
            moistureSlider.value = currentPlant.moisture;
        }
        
        // Cập nhật nutrients
        if (nutrientsSlider)
        {
            nutrientsSlider.value = currentPlant.nutrients;
        }
        
        // Cập nhật tên cây
        if (plantNameText && currentPlant.definition)
        {
            plantNameText.text = $"Cây: {currentPlant.definition.type}";
        }
        
        // Cập nhật stage
        if (stageText && currentPlant.definition && currentPlant.definition.stages != null)
        {
            if (currentPlant.currentStageIndex >= 0 && currentPlant.currentStageIndex < currentPlant.definition.stages.Length)
            {
                string stageName = currentPlant.definition.stages[currentPlant.currentStageIndex].name;
                stageText.text = $"Giai đoạn: {stageName} ({currentPlant.currentStageIndex + 1}/{currentPlant.definition.stages.Length})";
            }
            else
            {
                stageText.text = "Giai đoạn: Chưa xác định";
            }
        }
    }
    
    // Helper method để format percentage
    string FormatPercentage(float value)
    {
        return $"{value * 100:F0}%";
    }
}
