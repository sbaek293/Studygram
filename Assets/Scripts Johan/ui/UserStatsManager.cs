using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages personal, offline-focused user stats like Daily Streaks and Energy.
/// </summary>
public class UserStatsManager : MonoBehaviour
{
    // --- Singleton Pattern (Optional but recommended for single managers) ---
    public static UserStatsManager Instance { get; private set; }

    [Header("1. DAILY STREAK")]
    [SerializeField] private int dailyStreak = 0;
    private long lastLoginTicks; // Used to track time for streak checks

    [Header("2. ENERGY")]
    [SerializeField] private int maxEnergy = 100;
    [SerializeField] private int currentEnergy;
    [SerializeField] private int energyRechargeRate = 5; // Energy restored per minute
    private const float rechargeInterval = 60f; // Check every 60 seconds
    private float rechargeTimer;

    [Header("3. EVENTS")]
    // Event to update UI when streaks change
    public UnityEvent<int> onStreakChanged; 
    // Event to update UI when energy changes
    public UnityEvent<int> onEnergyChanged; 
    
    private void Awake()
    {
        // Simple Singleton enforcement
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Keep it running across scenes

        // Set initial Energy only if it's the very first run
        if (PlayerPrefs.GetInt("IsFirstRun", 1) == 1)
        {
            currentEnergy = maxEnergy;
            PlayerPrefs.SetInt("IsFirstRun", 0);
            SaveStats();
        }

        LoadStats();
        CheckStreak();
    }

    private void Update()
    {
        // Handle Energy Recharge over time
        rechargeTimer += Time.deltaTime;
        if (rechargeTimer >= rechargeInterval)
        {
            RechargeEnergy();
            rechargeTimer = 0f;
        }
    }

    // --- ENERGY LOGIC ---

    public bool TryUseEnergy(int amount)
    {
        if (currentEnergy >= amount)
        {
            currentEnergy -= amount;
            onEnergyChanged?.Invoke(currentEnergy);
            SaveStats();
            return true;
        }
        return false;
    }

    private void RechargeEnergy()
    {
        if (currentEnergy < maxEnergy)
        {
            currentEnergy = Mathf.Min(currentEnergy + energyRechargeRate, maxEnergy);
            onEnergyChanged?.Invoke(currentEnergy);
            SaveStats();
            Debug.Log($"Energy recharged to: {currentEnergy}");
        }
    }

    // --- STREAK LOGIC ---

    private void CheckStreak()
    {
        System.DateTime now = System.DateTime.Now;
        System.DateTime lastLogin = new System.DateTime(lastLoginTicks);

        System.TimeSpan timeSinceLastLogin = now - lastLogin;

        if (timeSinceLastLogin.TotalHours >= 24 && timeSinceLastLogin.TotalHours < 48)
        {
            // User logged in yesterday
            dailyStreak++;
            onStreakChanged?.Invoke(dailyStreak);
            Debug.Log($"Streak continued! Now at: {dailyStreak} days.");
        }
        else if (timeSinceLastLogin.TotalHours >= 48)
        {
            // User missed a day
            dailyStreak = 1;
            onStreakChanged?.Invoke(dailyStreak);
            Debug.Log("Streak broken. Reset to 1 day.");
        }
        // If less than 24 hours, do nothing (streak preserved)

        lastLoginTicks = now.Ticks; // Update last login time
        SaveStats();
    }
    
    // --- SAVE/LOAD ---

  private void LoadStats()
    {
        dailyStreak = PlayerPrefs.GetInt("DailyStreak", 0);
        currentEnergy = PlayerPrefs.GetInt("CurrentEnergy", maxEnergy);

        // FIX: Load long from string instead of int
        string lastLoginString = PlayerPrefs.GetString("LastLoginTicks", "0");
        if (!long.TryParse(lastLoginString, out lastLoginTicks))
        {
            // If parsing fails (e.g., first run or corrupt data), set to current time
            lastLoginTicks = System.DateTime.Now.Ticks; 
        }
    }

    private void SaveStats()
    {
        PlayerPrefs.SetInt("DailyStreak", dailyStreak);
        PlayerPrefs.SetInt("CurrentEnergy", currentEnergy);
        
        // FIX: Saving the long value as a string to avoid the CS1503 error
        PlayerPrefs.SetString("LastLoginTicks", lastLoginTicks.ToString()); 
        PlayerPrefs.Save();
    }
    
    // Public getters for the UI
    public int GetDailyStreak() => dailyStreak;
    public int GetCurrentEnergy() => currentEnergy;
    public int GetMaxEnergy() => maxEnergy;
}