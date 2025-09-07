using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Main player controller that handles input, state management, and coordinates
/// between different player components (Animator, NavMeshAgent, StateMachine)
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private StateMachine stateMachine;
    
    [Header("Settings")]
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private LayerMask cellLayerMask = 1;
    
    // Current task and target
    private PlayerTask currentTask = PlayerTask.None;
    private Vector3 targetPosition;
    private CellLogic targetCell;
    private FoodBase targetFood;
    
    // Events
    public System.Action<PlayerTask> OnTaskChanged;
    public System.Action<CellLogic> OnCellReached;
    
    private void Start()
    {
        InitializeComponents();
        SetupStateMachine();
    }
    
    private void Update()
    {
        HandleInput();
        UpdateStateMachine();
        CheckDestinationReached();
    }
    
    /// <summary>
    /// Initialize all required components
    /// </summary>
    private void InitializeComponents()
    {
        if (playerAnimator == null)
            playerAnimator = GetComponent<Animator>();
            
        if (navMeshAgent == null)
            navMeshAgent = GetComponent<NavMeshAgent>();
            
        if (stateMachine == null)
            stateMachine = GetComponent<StateMachine>();
    }
    
    /// <summary>
    /// Setup the state machine with initial idle state
    /// </summary>
    private void SetupStateMachine()
    {
        stateMachine.Initialize(new IdleState(this));
        playerAnimator.SetTrigger("Idle");
    }
    
    /// <summary>
    /// Handle player input for movement and interaction
    /// </summary>
    private void HandleInput()
    {
        // Handle escape key for game exit
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            return;
        }
        
        // Only handle input if in idle state
        if (stateMachine.CurrentState is IdleState)
        {
            HandleMovementInput();
        }
    }
    
    /// <summary>
    /// Handle movement and interaction input
    /// </summary>
    private void HandleMovementInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, cellLayerMask))
            {
                CellLogic cell = hit.collider.GetComponent<CellLogic>();
                if (cell != null)
                {
                    HandleCellClick(cell);
                }
            }
        }
    }
    
    /// <summary>
    /// Handle cell click and determine action
    /// </summary>
    private void HandleCellClick(CellLogic cell)
    {
        targetCell = cell;
        targetPosition = cell.transform.position;
        
        if (cell.CurrentState is FreeState)
        {
            // Cell is free - can plant
            UIManager.Instance.ShowPlantingOptions(cell);
        }
        else if (cell.CurrentState is PlantedState)
        {
            FoodBase food = cell.GetPlantedFood();
            if (food != null && food.CurrentState is RipeState)
            {
                // Food is ripe - can harvest
                SetTask(PlayerTask.Harvest);
            }
            else
            {
                // Food not ready
                UIManager.Instance.ShowMessage("Not ready for harvest yet!");
            }
        }
    }
    
    /// <summary>
    /// Set a new task for the player
    /// </summary>
    public void SetTask(PlayerTask task)
    {
        currentTask = task;
        OnTaskChanged?.Invoke(task);
        
        switch (task)
        {
            case PlayerTask.Plant:
            case PlayerTask.Harvest:
                MoveToTarget();
                break;
        }
    }
    
    /// <summary>
    /// Move player to target position
    /// </summary>
    private void MoveToTarget()
    {
        navMeshAgent.SetDestination(targetPosition);
        stateMachine.ChangeState(new WalkState(this));
        playerAnimator.SetTrigger("Walk");
    }
    
    /// <summary>
    /// Check if player has reached destination
    /// </summary>
    private void CheckDestinationReached()
    {
        if (navMeshAgent.hasPath && navMeshAgent.remainingDistance < 0.1f)
        {
            OnDestinationReached();
        }
    }
    
    /// <summary>
    /// Called when player reaches destination
    /// </summary>
    private void OnDestinationReached()
    {
        OnCellReached?.Invoke(targetCell);
        
        switch (currentTask)
        {
            case PlayerTask.Plant:
                PerformPlanting();
                break;
            case PlayerTask.Harvest:
                PerformHarvesting();
                break;
        }
        
        // Return to idle state
        stateMachine.ChangeState(new IdleState(this));
        playerAnimator.SetTrigger("Idle");
        currentTask = PlayerTask.None;
    }
    
    /// <summary>
    /// Perform planting action
    /// </summary>
    private void PerformPlanting()
    {
        stateMachine.ChangeState(new PlantState(this));
        playerAnimator.SetTrigger("Plant");
        
        // Get selected food type from UI
        FoodType selectedFood = UIManager.Instance.GetSelectedFoodType();
        if (selectedFood != FoodType.None)
        {
            targetCell.Plant(selectedFood);
        }
    }
    
    /// <summary>
    /// Perform harvesting action
    /// </summary>
    private void PerformHarvesting()
    {
        stateMachine.ChangeState(new PickupState(this));
        playerAnimator.SetTrigger("Pickup");
        
        targetCell.Harvest();
    }
    
    /// <summary>
    /// Update state machine
    /// </summary>
    private void UpdateStateMachine()
    {
        stateMachine.UpdateState();
    }
    
    // Public properties for state access
    public Animator PlayerAnimator => playerAnimator;
    public NavMeshAgent NavMeshAgent => navMeshAgent;
    public PlayerTask CurrentTask => currentTask;
    public CellLogic TargetCell => targetCell;
    public FoodBase TargetFood => targetFood;
}

/// <summary>
/// Enum representing different player tasks
/// </summary>
public enum PlayerTask
{
    None,
    Plant,
    Harvest
}

/// <summary>
/// Enum representing different food types
/// </summary>
public enum FoodType
{
    None,
    Carrot,
    Grass,
    Tree
}
