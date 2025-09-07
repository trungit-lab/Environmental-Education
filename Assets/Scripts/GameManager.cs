using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main game manager that coordinates all systems and manages game flow
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private bool isGameActive = false;
    [SerializeField] private float gameTime = 0f;
    [SerializeField] private int maxCells = 25; // 5x5 grid
    
    [Header("System References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private CellSelector cellSelector;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameStatView gameStatView;
    
    [Header("Cell Grid")]
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private Transform cellGridParent;
    [SerializeField] private Vector3 gridStartPosition = Vector3.zero;
    [SerializeField] private float cellSpacing = 2f;
    [SerializeField] private int gridSize = 5;
    
    // Cell management
    private List<CellLogic> allCells;
    private CellLogic[,] cellGrid;
    
    // Events
    public System.Action OnGameStarted;
    public System.Action OnGamePaused;
    public System.Action OnGameResumed;
    public System.Action OnGameEnded;
    
    private void Start()
    {
        InitializeGame();
    }
    
    private void Update()
    {
        if (isGameActive)
        {
            gameTime += Time.deltaTime;
        }
    }
    
    /// <summary>
    /// Initialize the game and all systems
    /// </summary>
    private void InitializeGame()
    {
        // Find or create system components
        FindSystemComponents();
        
        // Create cell grid
        CreateCellGrid();
        
        // Setup event connections
        SetupEventConnections();
        
        // Start the game
        StartGame();
    }
    
    /// <summary>
    /// Find or create all required system components
    /// </summary>
    private void FindSystemComponents()
    {
        // Find player controller
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
            if (playerController == null)
            {
                Debug.LogError("PlayerController not found! Please add one to the scene.");
            }
        }
        
        // Find cell selector
        if (cellSelector == null)
        {
            cellSelector = FindObjectOfType<CellSelector>();
            if (cellSelector == null)
            {
                Debug.LogError("CellSelector not found! Please add one to the scene.");
            }
        }
        
        // Find UI manager
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
            if (uiManager == null)
            {
                Debug.LogError("UIManager not found! Please add one to the scene.");
            }
        }
        
        // Find game stat view
        if (gameStatView == null)
        {
            gameStatView = FindObjectOfType<GameStatView>();
            if (gameStatView == null)
            {
                Debug.LogError("GameStatView not found! Please add one to the scene.");
            }
        }
    }
    
    /// <summary>
    /// Create the cell grid for the farm
    /// </summary>
    private void CreateCellGrid()
    {
        allCells = new List<CellLogic>();
        cellGrid = new CellLogic[gridSize, gridSize];
        
        // Create grid parent if needed
        if (cellGridParent == null)
        {
            GameObject gridParent = new GameObject("CellGrid");
            cellGridParent = gridParent.transform;
        }
        
        // Create cells in a grid pattern
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                Vector3 cellPosition = gridStartPosition + new Vector3(x * cellSpacing, 0, z * cellSpacing);
                CellLogic cell = CreateCell(cellPosition, x, z);
                
                allCells.Add(cell);
                cellGrid[x, z] = cell;
            }
        }
        
        Debug.Log($"Created {allCells.Count} cells in {gridSize}x{gridSize} grid");
    }
    
    /// <summary>
    /// Create a single cell at the specified position
    /// </summary>
    private CellLogic CreateCell(Vector3 position, int gridX, int gridZ)
    {
        GameObject cellObject;
        
        if (cellPrefab != null)
        {
            cellObject = Instantiate(cellPrefab, position, Quaternion.identity, cellGridParent);
        }
        else
        {
            // Create a simple cell if no prefab is provided
            cellObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cellObject.name = $"Cell_{gridX}_{gridZ}";
            cellObject.transform.position = position;
            cellObject.transform.SetParent(cellGridParent);
            
            // Scale the cell
            cellObject.transform.localScale = new Vector3(1.8f, 0.1f, 1.8f);
            
            // Set material
            Renderer renderer = cellObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material cellMaterial = new Material(Shader.Find("Standard"));
                cellMaterial.color = Color.gray;
                renderer.material = cellMaterial;
            }
        }
        
        // Add cell logic component
        CellLogic cellLogic = cellObject.GetComponent<CellLogic>();
        if (cellLogic == null)
        {
            cellLogic = cellObject.AddComponent<CellLogic>();
        }
        
        // Add state machine if not present
        StateMachine stateMachine = cellObject.GetComponent<StateMachine>();
        if (stateMachine == null)
        {
            stateMachine = cellObject.AddComponent<StateMachine>();
        }
        
        return cellLogic;
    }
    
    /// <summary>
    /// Setup connections between different systems
    /// </summary>
    private void SetupEventConnections()
    {
        // Connect cell events
        foreach (CellLogic cell in allCells)
        {
            cell.OnFoodPlanted += OnFoodPlanted;
            cell.OnFoodHarvested += OnFoodHarvested;
            cell.OnCellStateChanged += OnCellStateChanged;
        }
        
        // Connect player events
        if (playerController != null)
        {
            playerController.OnTaskChanged += OnPlayerTaskChanged;
            playerController.OnCellReached += OnPlayerReachedCell;
        }
        
        // Connect cell selector events
        if (cellSelector != null)
        {
            cellSelector.OnCellSelected += OnCellSelected;
            cellSelector.OnCellHovered += OnCellHovered;
        }
        
        // Connect game stat events
        if (gameStatView != null)
        {
            gameStatView.OnScoreChanged += OnScoreChanged;
            gameStatView.OnAchievementUnlocked += OnAchievementUnlocked;
        }
    }
    
    /// <summary>
    /// Start the game
    /// </summary>
    public void StartGame()
    {
        isGameActive = true;
        gameTime = 0f;
        
        // Reset current score
        if (gameStatView != null)
        {
            gameStatView.ResetCurrentScore();
        }
        
        // Show welcome message
        if (uiManager != null)
        {
            uiManager.ShowMessage("Welcome to the Farm Game! Click on cells to plant and harvest.");
        }
        
        OnGameStarted?.Invoke();
        
        Debug.Log("Game started!");
    }
    
    /// <summary>
    /// Pause the game
    /// </summary>
    public void PauseGame()
    {
        isGameActive = false;
        Time.timeScale = 0f;
        OnGamePaused?.Invoke();
    }
    
    /// <summary>
    /// Resume the game
    /// </summary>
    public void ResumeGame()
    {
        isGameActive = true;
        Time.timeScale = 1f;
        OnGameResumed?.Invoke();
    }
    
    /// <summary>
    /// End the game
    /// </summary>
    public void EndGame()
    {
        isGameActive = false;
        OnGameEnded?.Invoke();
        
        // Show final score
        if (uiManager != null && gameStatView != null)
        {
            int finalScore = gameStatView.GetCurrentScore();
            uiManager.ShowMessage($"Game Over! Final Score: {finalScore}");
        }
    }
    
    /// <summary>
    /// Reset the game (clear all cells)
    /// </summary>
    public void ResetGame()
    {
        foreach (CellLogic cell in allCells)
        {
            cell.ForceClear();
        }
        
        if (gameStatView != null)
        {
            gameStatView.ResetCurrentScore();
        }
        
        Debug.Log("Game reset - all cells cleared");
    }
    
    // Event handlers
    private void OnFoodPlanted(FoodBase food)
    {
        if (gameStatView != null)
        {
            gameStatView.RecordPlant(food.FoodType);
        }
        
        Debug.Log($"Food planted: {food.FoodType}");
    }
    
    private void OnFoodHarvested(FoodBase food)
    {
        if (gameStatView != null)
        {
            gameStatView.RecordHarvest(food.FoodType);
        }
        
        Debug.Log($"Food harvested: {food.FoodType}");
    }
    
    private void OnCellStateChanged(CellLogic cell)
    {
        // Handle cell state changes
        Debug.Log($"Cell state changed: {cell.GetStatusDescription()}");
    }
    
    private void OnPlayerTaskChanged(PlayerTask task)
    {
        Debug.Log($"Player task changed to: {task}");
    }
    
    private void OnPlayerReachedCell(CellLogic cell)
    {
        Debug.Log($"Player reached cell: {cell.name}");
    }
    
    private void OnCellSelected(CellLogic cell)
    {
        Debug.Log($"Cell selected: {cell.name}");
    }
    
    private void OnCellHovered(CellLogic cell)
    {
        // Handle cell hover (could show tooltip, etc.)
    }
    
    private void OnScoreChanged(int newScore)
    {
        Debug.Log($"Score updated: {newScore}");
    }
    
    private void OnAchievementUnlocked(Achievement achievement)
    {
        Debug.Log($"Achievement unlocked: {achievement.Name}");
    }
    
    // Public getters
    public bool IsGameActive => isGameActive;
    public float GameTime => gameTime;
    public List<CellLogic> AllCells => allCells;
    public CellLogic[,] CellGrid => cellGrid;
    public int GridSize => gridSize;
    
    /// <summary>
    /// Get cell at grid position
    /// </summary>
    public CellLogic GetCellAt(int x, int z)
    {
        if (x >= 0 && x < gridSize && z >= 0 && z < gridSize)
        {
            return cellGrid[x, z];
        }
        return null;
    }
    
    /// <summary>
    /// Get grid position of a cell
    /// </summary>
    public Vector2Int GetCellGridPosition(CellLogic cell)
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                if (cellGrid[x, z] == cell)
                {
                    return new Vector2Int(x, z);
                }
            }
        }
        return new Vector2Int(-1, -1);
    }
}
