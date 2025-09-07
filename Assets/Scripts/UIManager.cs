using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages all UI elements and interactions for the farm game
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject mainUIPanel;
    [SerializeField] private GameObject plantingOptionsPanel;
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private GameObject timerPanel;
    [SerializeField] private GameObject scorePanel;
    
    [Header("Planting Options")]
    [SerializeField] private Button carrotButton;
    [SerializeField] private Button grassButton;
    [SerializeField] private Button treeButton;
    [SerializeField] private Button cancelPlantingButton;
    
    [Header("Message System")]
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float messageDisplayTime = 3f;
    
    [Header("Timer System")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Slider timerSlider;
    [SerializeField] private Dictionary<FoodBase, GameObject> activeTimers;
    
    [Header("Score Display")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI totalScoreText;
    
    // Singleton instance
    public static UIManager Instance { get; private set; }
    
    // Current state
    private FoodType selectedFoodType = FoodType.None;
    private CellLogic currentPlantingCell;
    private Coroutine messageCoroutine;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        InitializeUI();
    }
    
    private void Start()
    {
        SetupEventListeners();
        HideAllPanels();
        ShowMainUI();
    }
    
    /// <summary>
    /// Initialize UI components
    /// </summary>
    private void InitializeUI()
    {
        activeTimers = new Dictionary<FoodBase, GameObject>();
        
        // Create UI elements if they don't exist
        if (mainUIPanel == null)
        {
            CreateMainUIPanel();
        }
        
        if (plantingOptionsPanel == null)
        {
            CreatePlantingOptionsPanel();
        }
        
        if (messagePanel == null)
        {
            CreateMessagePanel();
        }
        
        if (timerPanel == null)
        {
            CreateTimerPanel();
        }
        
        if (scorePanel == null)
        {
            CreateScorePanel();
        }
    }
    
    /// <summary>
    /// Setup event listeners for UI buttons
    /// </summary>
    private void SetupEventListeners()
    {
        if (carrotButton != null)
            carrotButton.onClick.AddListener(() => SelectFoodType(FoodType.Carrot));
            
        if (grassButton != null)
            grassButton.onClick.AddListener(() => SelectFoodType(FoodType.Grass));
            
        if (treeButton != null)
            treeButton.onClick.AddListener(() => SelectFoodType(FoodType.Tree));
            
        if (cancelPlantingButton != null)
            cancelPlantingButton.onClick.AddListener(CancelPlanting);
    }
    
    /// <summary>
    /// Show planting options for a cell
    /// </summary>
    public void ShowPlantingOptions(CellLogic cell)
    {
        currentPlantingCell = cell;
        selectedFoodType = FoodType.None;
        
        ShowPlantingOptionsPanel();
    }
    
    /// <summary>
    /// Select a food type for planting
    /// </summary>
    public void SelectFoodType(FoodType foodType)
    {
        selectedFoodType = foodType;
        
        if (currentPlantingCell != null)
        {
            // Tell player controller to plant
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.SetTask(PlayerTask.Plant);
            }
        }
        
        HidePlantingOptionsPanel();
    }
    
    /// <summary>
    /// Cancel planting operation
    /// </summary>
    public void CancelPlanting()
    {
        selectedFoodType = FoodType.None;
        currentPlantingCell = null;
        HidePlantingOptionsPanel();
    }
    
    /// <summary>
    /// Get the currently selected food type
    /// </summary>
    public FoodType GetSelectedFoodType()
    {
        return selectedFoodType;
    }
    
    /// <summary>
    /// Show a message to the player
    /// </summary>
    public void ShowMessage(string message)
    {
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
        }
        
        messageCoroutine = StartCoroutine(ShowMessageCoroutine(message));
    }
    
    /// <summary>
    /// Coroutine for showing messages with auto-hide
    /// </summary>
    private IEnumerator ShowMessageCoroutine(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
        }
        
        ShowMessagePanel();
        
        yield return new WaitForSeconds(messageDisplayTime);
        
        HideMessagePanel();
    }
    
    /// <summary>
    /// Show grow timer for a food item
    /// </summary>
    public void ShowGrowTimer(FoodBase food)
    {
        if (activeTimers.ContainsKey(food))
        {
            return; // Timer already exists
        }
        
        // Create timer UI element
        GameObject timerElement = CreateTimerElement(food);
        activeTimers.Add(food, timerElement);
        
        ShowTimerPanel();
    }
    
    /// <summary>
    /// Update grow timer display
    /// </summary>
    public void UpdateGrowTimer(FoodBase food, float currentTime, float totalTime)
    {
        if (activeTimers.ContainsKey(food))
        {
            GameObject timerElement = activeTimers[food];
            UpdateTimerElement(timerElement, currentTime, totalTime);
        }
    }
    
    /// <summary>
    /// Hide grow timer for a food item
    /// </summary>
    public void HideGrowTimer(FoodBase food)
    {
        if (activeTimers.ContainsKey(food))
        {
            Destroy(activeTimers[food]);
            activeTimers.Remove(food);
        }
        
        if (activeTimers.Count == 0)
        {
            HideTimerPanel();
        }
    }
    
    /// <summary>
    /// Update score display
    /// </summary>
    public void UpdateScoreDisplay(int currentScore, int totalScore)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
        }
        
        if (totalScoreText != null)
        {
            totalScoreText.text = $"Total: {totalScore}";
        }
    }
    
    /// <summary>
    /// Create timer UI element
    /// </summary>
    private GameObject CreateTimerElement(FoodBase food)
    {
        GameObject timerElement = new GameObject($"Timer_{food.name}");
        timerElement.transform.SetParent(timerPanel.transform);
        
        // Add timer components
        TextMeshProUGUI timerText = timerElement.AddComponent<TextMeshProUGUI>();
        timerText.text = $"{food.FoodType}: Growing...";
        timerText.fontSize = 14;
        timerText.color = Color.white;
        
        return timerElement;
    }
    
    /// <summary>
    /// Update timer element display
    /// </summary>
    private void UpdateTimerElement(GameObject timerElement, float currentTime, float totalTime)
    {
        TextMeshProUGUI text = timerElement.GetComponent<TextMeshProUGUI>();
        if (text != null)
        {
            float remainingTime = totalTime - currentTime;
            text.text = $"Growing: {remainingTime:F1}s";
        }
    }
    
    // Panel visibility methods
    private void ShowMainUI() => mainUIPanel?.SetActive(true);
    private void HideMainUI() => mainUIPanel?.SetActive(false);
    
    private void ShowPlantingOptionsPanel() => plantingOptionsPanel?.SetActive(true);
    private void HidePlantingOptionsPanel() => plantingOptionsPanel?.SetActive(false);
    
    private void ShowMessagePanel() => messagePanel?.SetActive(true);
    private void HideMessagePanel() => messagePanel?.SetActive(false);
    
    private void ShowTimerPanel() => timerPanel?.SetActive(true);
    private void HideTimerPanel() => timerPanel?.SetActive(false);
    
    private void ShowScorePanel() => scorePanel?.SetActive(true);
    private void HideScorePanel() => scorePanel?.SetActive(false);
    
    private void HideAllPanels()
    {
        HidePlantingOptionsPanel();
        HideMessagePanel();
        HideTimerPanel();
        HideScorePanel();
    }
    
    // UI Creation methods (for when prefabs aren't assigned)
    private void CreateMainUIPanel()
    {
        mainUIPanel = new GameObject("MainUIPanel");
        mainUIPanel.transform.SetParent(transform);
        
        Canvas canvas = mainUIPanel.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        mainUIPanel.AddComponent<CanvasScaler>();
        mainUIPanel.AddComponent<GraphicRaycaster>();
    }
    
    private void CreatePlantingOptionsPanel()
    {
        plantingOptionsPanel = new GameObject("PlantingOptionsPanel");
        plantingOptionsPanel.transform.SetParent(mainUIPanel.transform);
        
        // Create buttons
        carrotButton = CreateButton("CarrotButton", "Carrot", new Vector2(100, 50));
        grassButton = CreateButton("GrassButton", "Grass", new Vector2(100, 50));
        treeButton = CreateButton("TreeButton", "Tree", new Vector2(100, 50));
        cancelPlantingButton = CreateButton("CancelButton", "Cancel", new Vector2(100, 30));
        
        // Position buttons
        carrotButton.transform.localPosition = new Vector3(-150, 50, 0);
        grassButton.transform.localPosition = new Vector3(0, 50, 0);
        treeButton.transform.localPosition = new Vector3(150, 50, 0);
        cancelPlantingButton.transform.localPosition = new Vector3(0, -50, 0);
    }
    
    private void CreateMessagePanel()
    {
        messagePanel = new GameObject("MessagePanel");
        messagePanel.transform.SetParent(mainUIPanel.transform);
        
        messageText = messagePanel.AddComponent<TextMeshProUGUI>();
        messageText.text = "";
        messageText.fontSize = 18;
        messageText.color = Color.white;
        messageText.alignment = TextAlignmentOptions.Center;
        
        messagePanel.transform.localPosition = new Vector3(0, 200, 0);
    }
    
    private void CreateTimerPanel()
    {
        timerPanel = new GameObject("TimerPanel");
        timerPanel.transform.SetParent(mainUIPanel.transform);
        
        timerPanel.transform.localPosition = new Vector3(-300, 0, 0);
    }
    
    private void CreateScorePanel()
    {
        scorePanel = new GameObject("ScorePanel");
        scorePanel.transform.SetParent(mainUIPanel.transform);
        
        scoreText = scorePanel.AddComponent<TextMeshProUGUI>();
        scoreText.text = "Score: 0";
        scoreText.fontSize = 16;
        scoreText.color = Color.white;
        
        scorePanel.transform.localPosition = new Vector3(300, 200, 0);
    }
    
    private Button CreateButton(string name, string text, Vector2 size)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(plantingOptionsPanel.transform);
        
        Button button = buttonObj.AddComponent<Button>();
        Image image = buttonObj.AddComponent<Image>();
        image.color = Color.gray;
        
        // Create text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform);
        
        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.fontSize = 14;
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;
        
        // Set rect transform
        RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
        rectTransform.sizeDelta = size;
        
        return button;
    }
}
