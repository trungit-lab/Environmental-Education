using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages game statistics, score tracking, and achievements
/// </summary>
public class GameStatView : MonoBehaviour
{
    [Header("Score Settings")]
    [SerializeField] private int currentScore = 0;
    [SerializeField] private int totalScore = 0;
    [SerializeField] private int highScore = 0;
    
    [Header("Statistics")]
    [SerializeField] private int totalHarvests = 0;
    [SerializeField] private int totalPlants = 0;
    [SerializeField] private Dictionary<FoodType, int> foodTypeStats;
    
    [Header("Achievements")]
    [SerializeField] private List<Achievement> achievements;
    [SerializeField] private List<Achievement> unlockedAchievements;
    
    // Singleton instance
    public static GameStatView Instance { get; private set; }
    
    // Events
    public System.Action<int> OnScoreChanged;
    public System.Action<int> OnTotalScoreChanged;
    public System.Action<Achievement> OnAchievementUnlocked;
    public System.Action<FoodType> OnFoodHarvested;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeStats();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        LoadGameStats();
        UpdateUI();
    }
    
    /// <summary>
    /// Initialize statistics tracking
    /// </summary>
    private void InitializeStats()
    {
        foodTypeStats = new Dictionary<FoodType, int>();
        achievements = new List<Achievement>();
        unlockedAchievements = new List<Achievement>();
        
        // Initialize food type statistics
        foreach (FoodType foodType in System.Enum.GetValues(typeof(FoodType)))
        {
            if (foodType != FoodType.None)
            {
                foodTypeStats[foodType] = 0;
            }
        }
        
        // Setup achievements
        SetupAchievements();
    }
    
    /// <summary>
    /// Setup game achievements
    /// </summary>
    private void SetupAchievements()
    {
        achievements.Add(new Achievement("First Harvest", "Harvest your first crop", 1, AchievementType.TotalHarvests));
        achievements.Add(new Achievement("Farming Beginner", "Harvest 10 crops", 10, AchievementType.TotalHarvests));
        achievements.Add(new Achievement("Farming Expert", "Harvest 50 crops", 50, AchievementType.TotalHarvests));
        achievements.Add(new Achievement("Carrot Master", "Harvest 20 carrots", 20, AchievementType.CarrotHarvests));
        achievements.Add(new Achievement("Tree Farmer", "Harvest 5 trees", 5, AchievementType.TreeHarvests));
        achievements.Add(new Achievement("High Scorer", "Reach 1000 points", 1000, AchievementType.TotalScore));
    }
    
    /// <summary>
    /// Add points to the current score
    /// </summary>
    public void AddPoints(int points)
    {
        currentScore += points;
        totalScore += points;
        
        OnScoreChanged?.Invoke(currentScore);
        OnTotalScoreChanged?.Invoke(totalScore);
        
        // Check for high score
        if (currentScore > highScore)
        {
            highScore = currentScore;
            SaveGameStats();
        }
        
        // Update UI
        UpdateUI();
        
        // Check achievements
        CheckAchievements();
        
        Debug.Log($"Added {points} points. Current score: {currentScore}, Total: {totalScore}");
    }
    
    /// <summary>
    /// Record a food harvest
    /// </summary>
    public void RecordHarvest(FoodType foodType)
    {
        totalHarvests++;
        
        if (foodTypeStats.ContainsKey(foodType))
        {
            foodTypeStats[foodType]++;
        }
        
        OnFoodHarvested?.Invoke(foodType);
        
        // Check achievements
        CheckAchievements();
        
        Debug.Log($"Recorded harvest: {foodType}. Total harvests: {totalHarvests}");
    }
    
    /// <summary>
    /// Record a plant action
    /// </summary>
    public void RecordPlant(FoodType foodType)
    {
        totalPlants++;
        Debug.Log($"Recorded plant: {foodType}. Total plants: {totalPlants}");
    }
    
    /// <summary>
    /// Reset current score (for new game)
    /// </summary>
    public void ResetCurrentScore()
    {
        currentScore = 0;
        OnScoreChanged?.Invoke(currentScore);
        UpdateUI();
    }
    
    /// <summary>
    /// Get current score
    /// </summary>
    public int GetCurrentScore()
    {
        return currentScore;
    }
    
    /// <summary>
    /// Get total score
    /// </summary>
    public int GetTotalScore()
    {
        return totalScore;
    }
    
    /// <summary>
    /// Get high score
    /// </summary>
    public int GetHighScore()
    {
        return highScore;
    }
    
    /// <summary>
    /// Get total harvests
    /// </summary>
    public int GetTotalHarvests()
    {
        return totalHarvests;
    }
    
    /// <summary>
    /// Get total plants
    /// </summary>
    public int GetTotalPlants()
    {
        return totalPlants;
    }
    
    /// <summary>
    /// Get harvest count for specific food type
    /// </summary>
    public int GetFoodTypeHarvests(FoodType foodType)
    {
        return foodTypeStats.ContainsKey(foodType) ? foodTypeStats[foodType] : 0;
    }
    
    /// <summary>
    /// Get all unlocked achievements
    /// </summary>
    public List<Achievement> GetUnlockedAchievements()
    {
        return new List<Achievement>(unlockedAchievements);
    }
    
    /// <summary>
    /// Check for achievement unlocks
    /// </summary>
    private void CheckAchievements()
    {
        foreach (Achievement achievement in achievements)
        {
            if (!unlockedAchievements.Contains(achievement))
            {
                if (IsAchievementUnlocked(achievement))
                {
                    UnlockAchievement(achievement);
                }
            }
        }
    }
    
    /// <summary>
    /// Check if an achievement should be unlocked
    /// </summary>
    private bool IsAchievementUnlocked(Achievement achievement)
    {
        switch (achievement.Type)
        {
            case AchievementType.TotalHarvests:
                return totalHarvests >= achievement.Requirement;
                
            case AchievementType.TotalScore:
                return totalScore >= achievement.Requirement;
                
            case AchievementType.CarrotHarvests:
                return GetFoodTypeHarvests(FoodType.Carrot) >= achievement.Requirement;
                
            case AchievementType.TreeHarvests:
                return GetFoodTypeHarvests(FoodType.Tree) >= achievement.Requirement;
                
            default:
                return false;
        }
    }
    
    /// <summary>
    /// Unlock an achievement
    /// </summary>
    private void UnlockAchievement(Achievement achievement)
    {
        unlockedAchievements.Add(achievement);
        OnAchievementUnlocked?.Invoke(achievement);
        
        // Show achievement notification
        UIManager.Instance.ShowMessage($"Achievement Unlocked: {achievement.Name}!");
        
        Debug.Log($"Achievement unlocked: {achievement.Name} - {achievement.Description}");
    }
    
    /// <summary>
    /// Update UI display
    /// </summary>
    private void UpdateUI()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScoreDisplay(currentScore, totalScore);
        }
    }
    
    /// <summary>
    /// Save game statistics to PlayerPrefs
    /// </summary>
    private void SaveGameStats()
    {
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.SetInt("TotalScore", totalScore);
        PlayerPrefs.SetInt("TotalHarvests", totalHarvests);
        PlayerPrefs.SetInt("TotalPlants", totalPlants);
        
        // Save food type stats
        foreach (var kvp in foodTypeStats)
        {
            PlayerPrefs.SetInt($"FoodType_{kvp.Key}", kvp.Value);
        }
        
        // Save unlocked achievements
        string achievementString = "";
        foreach (Achievement achievement in unlockedAchievements)
        {
            achievementString += achievement.Name + ",";
        }
        PlayerPrefs.SetString("UnlockedAchievements", achievementString);
        
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// Load game statistics from PlayerPrefs
    /// </summary>
    private void LoadGameStats()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        totalScore = PlayerPrefs.GetInt("TotalScore", 0);
        totalHarvests = PlayerPrefs.GetInt("TotalHarvests", 0);
        totalPlants = PlayerPrefs.GetInt("TotalPlants", 0);
        
        // Load food type stats
        foreach (FoodType foodType in System.Enum.GetValues(typeof(FoodType)))
        {
            if (foodType != FoodType.None)
            {
                foodTypeStats[foodType] = PlayerPrefs.GetInt($"FoodType_{foodType}", 0);
            }
        }
        
        // Load unlocked achievements
        string achievementString = PlayerPrefs.GetString("UnlockedAchievements", "");
        if (!string.IsNullOrEmpty(achievementString))
        {
            string[] achievementNames = achievementString.Split(',');
            foreach (string name in achievementNames)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    Achievement achievement = achievements.Find(a => a.Name == name);
                    if (achievement != null)
                    {
                        unlockedAchievements.Add(achievement);
                    }
                }
            }
        }
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveGameStats();
        }
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveGameStats();
        }
    }
}

/// <summary>
/// Achievement class for tracking player accomplishments
/// </summary>
[System.Serializable]
public class Achievement
{
    public string Name;
    public string Description;
    public int Requirement;
    public AchievementType Type;
    
    public Achievement(string name, string description, int requirement, AchievementType type)
    {
        Name = name;
        Description = description;
        Requirement = requirement;
        Type = type;
    }
}

/// <summary>
/// Types of achievements
/// </summary>
public enum AchievementType
{
    TotalHarvests,
    TotalScore,
    CarrotHarvests,
    TreeHarvests,
    GrassHarvests
}
