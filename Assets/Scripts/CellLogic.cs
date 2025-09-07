using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the logic and state of individual farm cells
/// </summary>
public class CellLogic : MonoBehaviour
{
    [Header("Cell Settings")]
    [SerializeField] private Color defaultColor = Color.gray;
    [SerializeField] private Color freeColor = Color.green;
    [SerializeField] private Color plantedColor = Color.yellow;
    [SerializeField] private Color ripeColor = Color.red;
    
    [Header("Components")]
    [SerializeField] private Renderer cellRenderer;
    [SerializeField] private Transform foodSpawnPoint;
    
    // Cell state management
    private StateMachine stateMachine;
    private FoodBase plantedFood;
    private Material originalMaterial;
    private Color originalColor;
    
    // Events
    public System.Action<CellLogic> OnCellStateChanged;
    public System.Action<FoodBase> OnFoodPlanted;
    public System.Action<FoodBase> OnFoodHarvested;
    
    // Properties
    public IState CurrentState => stateMachine?.CurrentState;
    public FoodBase PlantedFood => plantedFood;
    public bool IsFree => CurrentState is FreeState;
    public bool IsPlanted => CurrentState is PlantedState;
    public bool IsRipe => plantedFood != null && plantedFood.CurrentState is RipeState;
    
    private void Start()
    {
        InitializeCell();
    }
    
    private void Update()
    {
        stateMachine?.UpdateState();
    }
    
    /// <summary>
    /// Initialize the cell with default state
    /// </summary>
    private void InitializeCell()
    {
        // Get or create state machine
        stateMachine = GetComponent<StateMachine>();
        if (stateMachine == null)
        {
            stateMachine = gameObject.AddComponent<StateMachine>();
        }
        
        // Get renderer component
        if (cellRenderer == null)
        {
            cellRenderer = GetComponent<Renderer>();
        }
        
        // Store original material and color
        if (cellRenderer != null)
        {
            originalMaterial = cellRenderer.material;
            originalColor = originalMaterial.color;
        }
        
        // Set up food spawn point if not assigned
        if (foodSpawnPoint == null)
        {
            foodSpawnPoint = transform;
        }
        
        // Initialize with free state
        stateMachine.Initialize(new FreeState(this));
    }
    
    /// <summary>
    /// Plant food in this cell
    /// </summary>
    public void Plant(FoodType foodType)
    {
        if (!IsFree)
        {
            Debug.LogWarning("Cannot plant in non-free cell!");
            return;
        }
        
        // Create food instance
        plantedFood = FoodFactory.CreateFood(foodType, foodSpawnPoint.position);
        if (plantedFood != null)
        {
            plantedFood.transform.SetParent(transform);
            
            // Change cell state to planted
            stateMachine.ChangeState(new PlantedState(this));
            
            // Initialize food with grow state
            float growthTime = GetGrowthTimeForFoodType(foodType);
            plantedFood.Initialize(new GrowState(plantedFood, growthTime));
            
            OnFoodPlanted?.Invoke(plantedFood);
            OnCellStateChanged?.Invoke(this);
            
            Debug.Log($"Planted {foodType} in cell {name}");
        }
    }
    
    /// <summary>
    /// Harvest food from this cell
    /// </summary>
    public void Harvest()
    {
        if (!IsRipe)
        {
            Debug.LogWarning("Cannot harvest non-ripe food!");
            return;
        }
        
        if (plantedFood != null)
        {
            // Get harvest result
            HarvestResult result = plantedFood.Interact();
            
            if (result.Success)
            {
                // Add points to game stats
                GameStatView.Instance.AddPoints(result.Points);
                
                // Destroy food object
                Destroy(plantedFood.gameObject);
                plantedFood = null;
                
                // Change cell state back to free
                stateMachine.ChangeState(new FreeState(this));
                
                OnFoodHarvested?.Invoke(plantedFood);
                OnCellStateChanged?.Invoke(this);
                
                Debug.Log($"Harvested food for {result.Points} points!");
            }
        }
    }
    
    /// <summary>
    /// Get the planted food in this cell
    /// </summary>
    public FoodBase GetPlantedFood()
    {
        return plantedFood;
    }
    
    /// <summary>
    /// Set highlight color for the cell
    /// </summary>
    public void SetHighlightColor(Color color)
    {
        if (cellRenderer != null && cellRenderer.material != null)
        {
            cellRenderer.material.color = color;
        }
    }
    
    /// <summary>
    /// Clear highlight and restore original color
    /// </summary>
    public void ClearHighlight()
    {
        if (cellRenderer != null && originalMaterial != null)
        {
            cellRenderer.material.color = originalColor;
        }
    }
    
    /// <summary>
    /// Get growth time for specific food type
    /// </summary>
    private float GetGrowthTimeForFoodType(FoodType foodType)
    {
        switch (foodType)
        {
            case FoodType.Carrot:
                return 30f; // 30 seconds
            case FoodType.Grass:
                return 45f; // 45 seconds
            case FoodType.Tree:
                return 120f; // 2 minutes
            default:
                return 60f; // Default 1 minute
        }
    }
    
    /// <summary>
    /// Force clear the cell (for debugging or reset)
    /// </summary>
    public void ForceClear()
    {
        if (plantedFood != null)
        {
            Destroy(plantedFood.gameObject);
            plantedFood = null;
        }
        
        stateMachine.ChangeState(new FreeState(this));
        OnCellStateChanged?.Invoke(this);
    }
    
    /// <summary>
    /// Check if cell can be planted
    /// </summary>
    public bool CanPlant()
    {
        return IsFree;
    }
    
    /// <summary>
    /// Check if cell can be harvested
    /// </summary>
    public bool CanHarvest()
    {
        return IsRipe;
    }
    
    /// <summary>
    /// Get cell status description
    /// </summary>
    public string GetStatusDescription()
    {
        if (IsFree)
            return "Empty - Ready for planting";
        else if (IsPlanted)
        {
            if (plantedFood != null)
            {
                if (plantedFood.CurrentState is GrowState)
                    return "Growing...";
                else if (plantedFood.CurrentState is RipeState)
                    return "Ready for harvest!";
                else
                    return "Planted";
            }
            return "Planted";
        }
        else
            return "Unknown state";
    }
}

/// <summary>
/// Factory class for creating food instances
/// </summary>
public static class FoodFactory
{
    /// <summary>
    /// Create a food instance based on food type
    /// </summary>
    public static FoodBase CreateFood(FoodType foodType, Vector3 position)
    {
        GameObject foodObject = new GameObject($"{foodType}Food");
        foodObject.transform.position = position;
        
        FoodBase food = foodObject.AddComponent<FoodBase>();
        food.InitializeFoodType(foodType);
        
        return food;
    }
}

/// <summary>
/// Result of harvesting operation
/// </summary>
public struct HarvestResult
{
    public bool Success;
    public int Points;
    public string Message;
    
    public HarvestResult(bool success, int points, string message = "")
    {
        Success = success;
        Points = points;
        Message = message;
    }
}
