using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State machine that manages different player states
/// </summary>
public class StateMachine : MonoBehaviour
{
    private IState currentState;
    
    /// <summary>
    /// Current active state
    /// </summary>
    public IState CurrentState => currentState;
    
    /// <summary>
    /// Initialize the state machine with a starting state
    /// </summary>
    public void Initialize(IState startingState)
    {
        currentState = startingState;
        currentState?.Enter();
    }
    
    /// <summary>
    /// Change to a new state
    /// </summary>
    public void ChangeState(IState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }
    
    /// <summary>
    /// Update the current state
    /// </summary>
    public void UpdateState()
    {
        currentState?.Update();
    }
}

/// <summary>
/// Interface for all states
/// </summary>
public interface IState
{
    void Enter();
    void Update();
    void Exit();
}

/// <summary>
/// Idle state - player is not performing any action
/// </summary>
public class IdleState : IState
{
    private PlayerController playerController;
    
    public IdleState(PlayerController controller)
    {
        playerController = controller;
    }
    
    public void Enter()
    {
        // Player is idle, ready for input
    }
    
    public void Update()
    {
        // Handle input in PlayerController
    }
    
    public void Exit()
    {
        // Clean up if needed
    }
}

/// <summary>
/// Walk state - player is moving to a destination
/// </summary>
public class WalkState : IState
{
    private PlayerController playerController;
    
    public WalkState(PlayerController controller)
    {
        playerController = controller;
    }
    
    public void Enter()
    {
        // Walking animation is already set in PlayerController
    }
    
    public void Update()
    {
        // Movement is handled by NavMeshAgent
        // State change is handled in PlayerController when destination is reached
    }
    
    public void Exit()
    {
        // Stop walking animation if needed
    }
}

/// <summary>
/// Plant state - player is planting a seed
/// </summary>
public class PlantState : IState
{
    private PlayerController playerController;
    private float plantDuration = 1.5f;
    private float plantTimer;
    
    public PlantState(PlayerController controller)
    {
        playerController = controller;
    }
    
    public void Enter()
    {
        plantTimer = plantDuration;
        // Planting animation is already set in PlayerController
    }
    
    public void Update()
    {
        plantTimer -= Time.deltaTime;
        if (plantTimer <= 0)
        {
            // Planting complete, state will be changed in PlayerController
        }
    }
    
    public void Exit()
    {
        // Clean up planting effects
    }
}

/// <summary>
/// Pickup state - player is harvesting food
/// </summary>
public class PickupState : IState
{
    private PlayerController playerController;
    private float pickupDuration = 1.0f;
    private float pickupTimer;
    
    public PickupState(PlayerController controller)
    {
        playerController = controller;
    }
    
    public void Enter()
    {
        pickupTimer = pickupDuration;
        // Pickup animation is already set in PlayerController
    }
    
    public void Update()
    {
        pickupTimer -= Time.deltaTime;
        if (pickupTimer <= 0)
        {
            // Pickup complete, state will be changed in PlayerController
        }
    }
    
    public void Exit()
    {
        // Clean up pickup effects
    }
}

/// <summary>
/// Grow state - food is growing
/// </summary>
public class GrowState : IState
{
    private FoodBase food;
    private float growthTime;
    private float currentGrowthTime;
    
    public GrowState(FoodBase foodBase, float growthDuration)
    {
        food = foodBase;
        growthTime = growthDuration;
        currentGrowthTime = 0f;
    }
    
    public void Enter()
    {
        currentGrowthTime = 0f;
        UIManager.Instance.ShowGrowTimer(food);
    }
    
    public void Update()
    {
        currentGrowthTime += Time.deltaTime;
        
        // Update growth progress
        float progress = currentGrowthTime / growthTime;
        food.UpdateGrowthProgress(progress);
        
        // Update UI timer
        UIManager.Instance.UpdateGrowTimer(food, currentGrowthTime, growthTime);
        
        if (currentGrowthTime >= growthTime)
        {
            // Growth complete, change to ripe state
            food.ChangeState(new RipeState(food));
        }
    }
    
    public void Exit()
    {
        UIManager.Instance.HideGrowTimer(food);
    }
}

/// <summary>
/// Ripe state - food is ready for harvest
/// </summary>
public class RipeState : IState
{
    private FoodBase food;
    
    public RipeState(FoodBase foodBase)
    {
        food = foodBase;
    }
    
    public void Enter()
    {
        food.ShowRipeVisual();
        food.EnableParticleEffects();
        UIManager.Instance.HideGrowTimer(food);
    }
    
    public void Update()
    {
        // Food is ripe and waiting for harvest
    }
    
    public void Exit()
    {
        // Clean up ripe effects
    }
}

/// <summary>
/// Free state - cell is available for planting
/// </summary>
public class FreeState : IState
{
    private CellLogic cell;
    
    public FreeState(CellLogic cellLogic)
    {
        cell = cellLogic;
    }
    
    public void Enter()
    {
        cell.SetHighlightColor(Color.green);
    }
    
    public void Update()
    {
        // Cell is free and ready for planting
    }
    
    public void Exit()
    {
        cell.ClearHighlight();
    }
}

/// <summary>
/// Planted state - cell has a growing plant
/// </summary>
public class PlantedState : IState
{
    private CellLogic cell;
    
    public PlantedState(CellLogic cellLogic)
    {
        cell = cellLogic;
    }
    
    public void Enter()
    {
        cell.SetHighlightColor(Color.yellow);
    }
    
    public void Update()
    {
        // Cell has a plant growing
    }
    
    public void Exit()
    {
        cell.ClearHighlight();
    }
}
