using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all plantable food items with growth and harvest logic
/// </summary>
public class FoodBase : MonoBehaviour
{
    [Header("Food Settings")]
    [SerializeField] private FoodType foodType = FoodType.Carrot;
    [SerializeField] private int harvestPoints = 10;
    [SerializeField] private float growthProgress = 0f;
    
    [Header("Visual Components")]
    [SerializeField] private Renderer foodRenderer;
    [SerializeField] private GameObject seedVisual;
    [SerializeField] private GameObject growingVisual;
    [SerializeField] private GameObject ripeVisual;
    [SerializeField] private ParticleSystem harvestParticles;
    [SerializeField] private ParticleSystem growthParticles;
    
    [Header("Growth Settings")]
    [SerializeField] private Vector3 seedScale = Vector3.one * 0.1f;
    [SerializeField] private Vector3 growingScale = Vector3.one * 0.5f;
    [SerializeField] private Vector3 ripeScale = Vector3.one;
    [SerializeField] private Color seedColor = Color.brown;
    [SerializeField] private Color growingColor = Color.green;
    [SerializeField] private Color ripeColor = Color.orange;
    
    // State management
    private StateMachine stateMachine;
    private Material foodMaterial;
    private Transform foodTransform;
    
    // Events
    public System.Action<FoodBase> OnGrowthComplete;
    public System.Action<FoodBase> OnHarvested;
    
    // Properties
    public FoodType FoodType => foodType;
    public IState CurrentState => stateMachine?.CurrentState;
    public float GrowthProgress => growthProgress;
    public int HarvestPoints => harvestPoints;
    public bool IsRipe => CurrentState is RipeState;
    public bool IsGrowing => CurrentState is GrowState;
    
    private void Start()
    {
        InitializeFood();
    }
    
    private void Update()
    {
        stateMachine?.UpdateState();
    }
    
    /// <summary>
    /// Initialize the food object
    /// </summary>
    private void InitializeFood()
    {
        // Get or create state machine
        stateMachine = GetComponent<StateMachine>();
        if (stateMachine == null)
        {
            stateMachine = gameObject.AddComponent<StateMachine>();
        }
        
        // Get components
        foodTransform = transform;
        if (foodRenderer == null)
        {
            foodRenderer = GetComponent<Renderer>();
        }
        
        // Create material if needed
        if (foodRenderer != null)
        {
            foodMaterial = new Material(foodRenderer.material);
            foodRenderer.material = foodMaterial;
        }
        
        // Set initial visual state
        SetVisualState(FoodVisualState.Seed);
    }
    
    /// <summary>
    /// Initialize food with specific type
    /// </summary>
    public void InitializeFoodType(FoodType type)
    {
        foodType = type;
        
        // Set harvest points based on food type
        switch (foodType)
        {
            case FoodType.Carrot:
                harvestPoints = 15;
                break;
            case FoodType.Grass:
                harvestPoints = 10;
                break;
            case FoodType.Tree:
                harvestPoints = 50;
                break;
        }
        
        // Set appropriate colors
        ripeColor = GetRipeColorForType(foodType);
    }
    
    /// <summary>
    /// Initialize food with a specific state
    /// </summary>
    public void Initialize(IState initialState)
    {
        if (stateMachine != null)
        {
            stateMachine.Initialize(initialState);
        }
    }
    
    /// <summary>
    /// Change the current state
    /// </summary>
    public void ChangeState(IState newState)
    {
        stateMachine?.ChangeState(newState);
    }
    
    /// <summary>
    /// Update growth progress (called by GrowState)
    /// </summary>
    public void UpdateGrowthProgress(float progress)
    {
        growthProgress = Mathf.Clamp01(progress);
        
        // Update visual scale based on progress
        Vector3 targetScale = Vector3.Lerp(seedScale, growingScale, growthProgress);
        foodTransform.localScale = targetScale;
        
        // Update color based on progress
        if (foodMaterial != null)
        {
            Color currentColor = Color.Lerp(seedColor, growingColor, growthProgress);
            foodMaterial.color = currentColor;
        }
    }
    
    /// <summary>
    /// Show ripe visual effects
    /// </summary>
    public void ShowRipeVisual()
    {
        SetVisualState(FoodVisualState.Ripe);
        
        // Play growth completion effect
        if (growthParticles != null)
        {
            growthParticles.Play();
        }
        
        OnGrowthComplete?.Invoke(this);
    }
    
    /// <summary>
    /// Enable particle effects for ripe state
    /// </summary>
    public void EnableParticleEffects()
    {
        if (harvestParticles != null)
        {
            harvestParticles.gameObject.SetActive(true);
        }
    }
    
    /// <summary>
    /// Interact with the food (harvest)
    /// </summary>
    public HarvestResult Interact()
    {
        if (!IsRipe)
        {
            return new HarvestResult(false, 0, "Food is not ready for harvest!");
        }
        
        // Play harvest effect
        if (harvestParticles != null)
        {
            harvestParticles.Play();
        }
        
        // Return harvest result
        HarvestResult result = new HarvestResult(true, harvestPoints, $"Harvested {foodType}!");
        
        OnHarvested?.Invoke(this);
        
        return result;
    }
    
    /// <summary>
    /// Set visual state of the food
    /// </summary>
    private void SetVisualState(FoodVisualState state)
    {
        // Disable all visual states
        if (seedVisual != null) seedVisual.SetActive(false);
        if (growingVisual != null) growingVisual.SetActive(false);
        if (ripeVisual != null) ripeVisual.SetActive(false);
        
        // Enable appropriate visual state
        switch (state)
        {
            case FoodVisualState.Seed:
                if (seedVisual != null)
                {
                    seedVisual.SetActive(true);
                    foodTransform.localScale = seedScale;
                }
                if (foodMaterial != null)
                {
                    foodMaterial.color = seedColor;
                }
                break;
                
            case FoodVisualState.Growing:
                if (growingVisual != null)
                {
                    growingVisual.SetActive(true);
                    foodTransform.localScale = growingScale;
                }
                if (foodMaterial != null)
                {
                    foodMaterial.color = growingColor;
                }
                break;
                
            case FoodVisualState.Ripe:
                if (ripeVisual != null)
                {
                    ripeVisual.SetActive(true);
                    foodTransform.localScale = ripeScale;
                }
                if (foodMaterial != null)
                {
                    foodMaterial.color = ripeColor;
                }
                break;
        }
    }
    
    /// <summary>
    /// Get ripe color for specific food type
    /// </summary>
    private Color GetRipeColorForType(FoodType type)
    {
        switch (type)
        {
            case FoodType.Carrot:
                return Color.orange;
            case FoodType.Grass:
                return Color.green;
            case FoodType.Tree:
                return Color.brown;
            default:
                return Color.white;
        }
    }
    
    /// <summary>
    /// Start growth coroutine
    /// </summary>
    public void StartGrowth(float duration)
    {
        StartCoroutine(GrowthCoroutine(duration));
    }
    
    /// <summary>
    /// Coroutine for handling growth over time
    /// </summary>
    private IEnumerator GrowthCoroutine(float duration)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            
            UpdateGrowthProgress(progress);
            
            yield return null;
        }
        
        // Growth complete
        UpdateGrowthProgress(1f);
        ShowRipeVisual();
    }
    
    /// <summary>
    /// Get food description
    /// </summary>
    public string GetDescription()
    {
        string stateDescription = "";
        
        if (IsRipe)
            stateDescription = "Ready for harvest!";
        else if (IsGrowing)
            stateDescription = $"Growing... {(growthProgress * 100):F0}%";
        else
            stateDescription = "Seed planted";
            
        return $"{foodType}: {stateDescription}";
    }
    
    /// <summary>
    /// Force complete growth (for debugging)
    /// </summary>
    public void ForceCompleteGrowth()
    {
        if (IsGrowing)
        {
            UpdateGrowthProgress(1f);
            ShowRipeVisual();
            ChangeState(new RipeState(this));
        }
    }
}

/// <summary>
/// Enum for food visual states
/// </summary>
public enum FoodVisualState
{
    Seed,
    Growing,
    Ripe
}
