using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using Firebase.Database;
using System.Threading.Tasks;

/// <summary>
/// Hybrid System: Uses Progress Points for passive Garden Upgrades, and Coins for Card Shop.
/// SHARED garden progress via Firebase, LOCAL coins for personal use.
/// </summary>
public class GardenManager : MonoBehaviour
{
    [System.Serializable]
    public class VisualTier
    {
        public int minLevel;
        public Sprite gardenSprite;
        public GameObject[] decorationsToEnable; 
    }
    
    [Header("1. PROGRESS & LEVELING (Garden Visuals)")]
    [SerializeField] private int currentProgressPoints = 0;
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int baseProgressPerLevel = 100;
    [SerializeField] private float scalingFactor = 1.5f;
    
    [Header("2. CURRENCY (Card Shop)")]
    [SerializeField] private int currentCoins = 0;

    [Header("3. VISUAL TIERS")]
    [SerializeField] private List<VisualTier> visualTiers = new List<VisualTier>();
    [SerializeField] private SpriteRenderer gardenGround; 

    [Header("4. EVENTS")]
    public UnityEvent<int> onLevelUp;
    public UnityEvent<int> onCoinsChanged;
    
    [Header("5. FIREBASE SYNC")]
    private string currentGroupId;
    private DatabaseReference db;
    private bool isInitialized = false;

    public static GardenManager Instance;
    private void Awake()
{
    // Just set the static reference so other scripts can find it
    Instance = this;
    
    // REMOVED: DontDestroyOnLoad(gameObject); 
    // We WANT this to be destroyed when we leave the scene so a fresh one can load next time.
}

    void Start()
    {
        db = FirebaseDatabase.DefaultInstance.RootReference;
        InitializeGarden();
    }
    
    async void InitializeGarden()
    {
        // Get user's group ID
        string userId = AppContext.UserId;
        
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("No user ID found! User must complete quiz first.");
            return;
        }
        
        // Fetch user's active group from Firebase
        var userSnapshot = await db.Child("users").Child(userId).GetValueAsync();
        
        if (!userSnapshot.Child("activeGroup").Exists)
        {
            Debug.LogWarning("User has no active group yet");
            return;
        }
        
        currentGroupId = userSnapshot.Child("activeGroup").Value.ToString();
        Debug.Log($"Garden initialized for group: {currentGroupId}");
        
        // Load shared garden progress from Firebase
        await LoadProgressFromFirebase();
        
        // Listen for changes
        ListenForGardenUpdates();
        
        isInitialized = true;
        UpdateGardenVisuals();
        
        // Load personal coins from local storage
        currentCoins = UserManager.Instance.coins;
    }
    
    // --- PROGRESS LOGIC (SHARED VIA FIREBASE) ---
    
    public async void AddProgressPoints(int points)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("Garden not initialized yet");
            return;
        }
        
        currentProgressPoints += points;
        Debug.Log($"Group gained {points} Progress Points! Total: {currentProgressPoints}");
        
        while (currentProgressPoints >= GetPointsForNextLevel())
        {
            await LevelUp();
        }
        
        await SaveProgressToFirebase();
    }
    
    async Task LevelUp()
    {
        currentProgressPoints -= GetPointsForNextLevel();
        currentLevel++;
        Debug.Log($"🎉 GARDEN UPGRADED! Now Level {currentLevel}");
        UpdateGardenVisuals();
        onLevelUp?.Invoke(currentLevel);
    }
    
    public int GetPointsForNextLevel()
    {
        return Mathf.RoundToInt(baseProgressPerLevel * Mathf.Pow(scalingFactor, currentLevel - 1));
    }
    
    // --- COIN LOGIC (LOCAL ONLY) ---

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        onCoinsChanged?.Invoke(currentCoins);
        PlayerPrefs.SetInt("Coins", currentCoins);
        PlayerPrefs.Save();
        Debug.Log($"💰 Personal Wallet gained {amount} coins! Total: {currentCoins}");
    }
    
    // --- FIREBASE SAVE/LOAD ---

    async Task SaveProgressToFirebase()
    {
        if (string.IsNullOrEmpty(currentGroupId)) return;
        
        var updates = new Dictionary<string, object>
        {
            { $"groups/{currentGroupId}/gardenLevel", currentLevel },
            { $"groups/{currentGroupId}/gardenProgress", currentProgressPoints }
        };
        
        await db.UpdateChildrenAsync(updates);
        Debug.Log($"Saved garden progress to Firebase: Level {currentLevel}");
    }

    async Task LoadProgressFromFirebase()
    {
        if (string.IsNullOrEmpty(currentGroupId)) return;
        
        var groupSnapshot = await db.Child("groups").Child(currentGroupId).GetValueAsync();
        
        if (groupSnapshot.Exists)
        {
            if (groupSnapshot.Child("gardenLevel").Exists)
            {
                currentLevel = System.Convert.ToInt32(groupSnapshot.Child("gardenLevel").Value);
            }
            
            if (groupSnapshot.Child("gardenProgress").Exists)
            {
                currentProgressPoints = System.Convert.ToInt32(groupSnapshot.Child("gardenProgress").Value);
            }
            
            Debug.Log($"Loaded garden from Firebase: Level {currentLevel}, Progress {currentProgressPoints}");
        }
        else
        {
            Debug.Log("No existing garden data, starting fresh");
            await SaveProgressToFirebase();
        }
    }

    void ListenForGardenUpdates()
    {
        if (string.IsNullOrEmpty(currentGroupId)) return;
        
        db.Child("groups").Child(currentGroupId).Child("gardenLevel")
            .ValueChanged += HandleGardenLevelChanged;
        
        db.Child("groups").Child(currentGroupId).Child("gardenProgress")
            .ValueChanged += HandleGardenProgressChanged;
    }

    void HandleGardenLevelChanged(object sender, ValueChangedEventArgs e)
    {
        if (e.Snapshot.Exists)
        {
            int newLevel = System.Convert.ToInt32(e.Snapshot.Value);
            if (newLevel != currentLevel)
            {
                Debug.Log($"Garden level updated by another member: {newLevel}");
                currentLevel = newLevel;
                UpdateGardenVisuals();
                onLevelUp?.Invoke(currentLevel);
            }
        }
    }

    void HandleGardenProgressChanged(object sender, ValueChangedEventArgs e)
    {
        if (e.Snapshot.Exists)
        {
            int newProgress = System.Convert.ToInt32(e.Snapshot.Value);
            if (newProgress != currentProgressPoints)
            {
                Debug.Log($"Garden progress updated: {newProgress}");
                currentProgressPoints = newProgress;
            }
        }
    }
    
    // --- VISUALS ---

    void UpdateGardenVisuals()
    {
        VisualTier currentTier = null;
        for (int i = visualTiers.Count - 1; i >= 0; i--)
        {
            if (currentLevel >= visualTiers[i].minLevel)
            {
                currentTier = visualTiers[i];
                break;
            }
        }
        
        if (currentTier != null && gardenGround != null && currentTier.gardenSprite != null)
        {
            gardenGround.sprite = currentTier.gardenSprite;
        }
    }
    
    // --- GETTERS ---
    public int GetCurrentLevel() => currentLevel;
    public int GetCurrentProgressPoints() => currentProgressPoints;
    public float GetProgress() => (float)currentProgressPoints / GetPointsForNextLevel();
    public int GetCoins() => currentCoins;
    
    // --- CLEANUP ---

    void OnDestroy()
    {
        if (!string.IsNullOrEmpty(currentGroupId) && db != null)
        {
            db.Child("groups").Child(currentGroupId).Child("gardenLevel")
                .ValueChanged -= HandleGardenLevelChanged;
            db.Child("groups").Child(currentGroupId).Child("gardenProgress")
                .ValueChanged -= HandleGardenProgressChanged;
        }
    }
    
    // --- TESTING HELPERS ---
    [ContextMenu("Add 50 Progress Points (Test)")]
    public void TestAddProgress() { AddProgressPoints(50); }
    
    [ContextMenu("Add 500 Coins (Test)")]
    public void TestAddCoins() { AddCoins(500); }
    
    [ContextMenu("Reset Progress")]
    public void ResetProgress() 
    { 
        PlayerPrefs.DeleteAll(); 
        currentLevel = 1; 
        currentProgressPoints = 0; 
        currentCoins = 0; 
        UpdateGardenVisuals(); 
        Debug.Log("Progress reset!"); 
    }
}