using UnityEngine;

public class PlantGrowth : MonoBehaviour
{
    [Header("Data")]
    public PlantDefinition definition;

    [Header("Sunlight")]
    public Light sunLight;
    public float sunFactor = 0.7f;

    [Header("Water & Fertilizer")]
    public float moistureDecayPerSec = 0.02f;
    public float nutrientDecayPerSec = 0.01f;
    public float waterBoost = 0.5f;
    public float fertilizerBoost = 0.8f;

    [Header("Animation")]
    public AnimationCurve stageScaleCurve;
    [Tooltip("Scale ban đầu khi instantiate prefab (0 = từ 0, 1 = từ scale gốc của prefab)")]
    [Range(0f, 1f)] public float firstScale = 0f;

    [Range(0f, 1f)] public float progress;
    [Range(0f, 1f)] public float moisture = 0.5f;
    [Range(0f, 1f)] public float nutrients = 0.2f;

    public int currentStageIndex = -1;

    // Để private để tránh kéo nhầm prefab vào Inspector
    GameObject currentStageGO;

    float _baseRate;
    bool _initialized;

    void Awake() { /* để trống, chờ được cấu hình */ }

    void Start() { TrySetup(); }

    // Cho phép Planter gọi sau khi AddComponent
    public void Configure(PlantDefinition def, float initMoisture, float initNutrients, Light sun = null)
    {
        definition = def;
        moisture = Mathf.Clamp01(initMoisture);
        nutrients = Mathf.Clamp01(initNutrients);
        if (sun) sunLight = sun;
        TrySetup(force: true);
    }

    void TrySetup(bool force = false)
    {
        if (_initialized && !force) return;
        if (!definition || definition.stages == null || definition.stages.Length == 0) return; // chờ được set

        if (!sunLight) sunLight = FindObjectOfType<Light>();
        _baseRate = 1f / Mathf.Max(1f, definition.baseTotalGrowTime);

        // đảm bảo không giữ reference asset
        SafeDestroyCurrentStage();
        UpdateStage(forceRefresh: true);
        _initialized = true;
    }

    void Update()
    {
        if (!_initialized) { TrySetup(); return; }

        moisture = Mathf.Clamp01(moisture - moistureDecayPerSec * Time.deltaTime);
        nutrients = Mathf.Clamp01(nutrients - nutrientDecayPerSec * Time.deltaTime);

        float sunMult = 1f;
        if (sunLight && sunLight.type == LightType.Directional)
        {
            float normI = Mathf.Clamp01(sunLight.intensity / 1.2f);
            sunMult = 1f + normI * sunFactor;
        }

        float rate = _baseRate * (1f + moisture * waterBoost) * (1f + nutrients * fertilizerBoost) * sunMult;

        if (progress < 1f)
            progress = Mathf.Clamp01(progress + rate * Time.deltaTime);

        UpdateStage();
        UpdateStageScale(); // Uncomment để sử dụng scale animation với firstScale
    }

    void SafeDestroyCurrentStage()
    {
        if (!currentStageGO) return;
        // Chỉ destroy instance trong scene, không phá asset
        if (currentStageGO.scene.IsValid())
            Destroy(currentStageGO);
        currentStageGO = null;
    }

    void UpdateStage(bool forceRefresh = false)
    {
        if (!definition || definition.stages == null || definition.stages.Length == 0) return;

        // Tìm stage phù hợp với progress hiện tại
        int stageIndex = 0; // Bắt đầu từ stage đầu tiên
        for (int i = 0; i < definition.stages.Length; i++)
        {
            if (progress >= definition.stages[i].threshold)
            {
                stageIndex = i; // Cập nhật stage index khi đủ threshold
            }
            else
            {
                break; // Dừng khi gặp threshold chưa đạt
            }
        }

        if (forceRefresh || stageIndex != currentStageIndex)
        {
            currentStageIndex = stageIndex;
            var stage = definition.stages[currentStageIndex];

            SafeDestroyCurrentStage();

            if (stage.prefab)
            {
                currentStageGO = Instantiate(stage.prefab, transform);
                currentStageGO.transform.localPosition = Vector3.zero;
                currentStageGO.transform.localRotation = Quaternion.identity;
                
                // Sử dụng firstScale để linh hoạt với các loại cây khác nhau
                Vector3 originalScale = stage.prefab.transform.localScale;
                currentStageGO.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, firstScale);
            }

            if (definition.stageTransitionVFX)
            {
                var vfx = Instantiate(definition.stageTransitionVFX, transform.position, Quaternion.identity);
                Destroy(vfx, 3f);
            }
        }
    }

    void UpdateStageScale()
    {
        if (!currentStageGO) return;

        float stageStart = definition.stages[currentStageIndex].threshold;
        float stageEnd = (currentStageIndex < definition.stages.Length - 1)
                       ? definition.stages[currentStageIndex + 1].threshold
                       : 1f;

        float span = Mathf.Max(0.0001f, stageEnd - stageStart);
        float t = Mathf.Clamp01((progress - stageStart) / span);

        if (stageScaleCurve != null && stageScaleCurve.keys.Length > 0)
            t = Mathf.Clamp01(stageScaleCurve.Evaluate(t));

        // Sử dụng firstScale để linh hoạt với các loại cây
        Vector3 originalScale = definition.stages[currentStageIndex].prefab.transform.localScale;
        Vector3 targetScale = originalScale;
        Vector3 startScale = Vector3.Lerp(Vector3.zero, originalScale, firstScale);
        
        currentStageGO.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
    }

    public void ApplyWater(float amount)  { moisture  = Mathf.Clamp01(moisture  + amount); }
    public void ApplyFertilizer(float amount) { nutrients = Mathf.Clamp01(nutrients + amount); }
}
