using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// Hybrid System: Uses Progress Points for passive Garden Upgrades, and Coins for Card Shop.
/// </summary>
public class GardenManager : MonoBehaviour
{
    [System.Serializable]
    public class VisualTier
    {
        public int minLevel; // This tier applies from this level onwards
        public Sprite gardenSprite; // The complete garden image for this tier
        public GameObject[] decorationsToEnable; 
    }
    
    [Header("1. PROGRESS & LEVELING (Garden Visuals)")]
    [SerializeField] private int currentProgressPoints = 0;
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int baseProgressPerLevel = 100; // Acts like base XP
    [SerializeField] private float scalingFactor = 1.5f;
    
    [Header("2. CURRENCY (Card Shop)")]
    [SerializeField] private int currentCoins = 0;

    [Header("3. VISUAL TIERS")]
    [SerializeField] private List<VisualTier> visualTiers = new List<VisualTier>();
    [SerializeField] private SpriteRenderer gardenGround; 

    [Header("4. EVENTS")]
    public UnityEvent<int> onLevelUp;
    public UnityEvent<int> onCoinsChanged;
    
    void Start()
    {
        LoadProgress();
        UpdateGardenVisuals();
    }
    
    // --- PROGRESS LOGIC (FOR GARDEN UPGRADES) ---
    
    public void AddProgressPoints(int points)
    {
        currentProgressPoints += points;
        Debug.Log($"Group gained {points} Progress Points! Total: {currentProgressPoints}");
        
        while (currentProgressPoints >= GetPointsForNextLevel())
        {
            LevelUp();
        }
        
        SaveProgress();
    }
    
    void LevelUp()
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
    
    // --- COIN LOGIC (FOR CARD SHOP) ---

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        onCoinsChanged?.Invoke(currentCoins);
        SaveProgress();
        Debug.Log($"💰 Personal Wallet gained {amount} coins! Total: {currentCoins}");
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
    
    // --- SAVE SYSTEM ---
    void SaveProgress()
    {
        PlayerPrefs.SetInt("GardenLevel", currentLevel);
        PlayerPrefs.SetInt("GardenProgress", currentProgressPoints);
        PlayerPrefs.SetInt("Coins", currentCoins);
        PlayerPrefs.Save();
    }
    
    void LoadProgress()
    {
        currentLevel = PlayerPrefs.GetInt("GardenLevel", 1);
        currentProgressPoints = PlayerPrefs.GetInt("GardenProgress", 0);
        currentCoins = PlayerPrefs.GetInt("Coins", 0);
    }
    
    // --- TESTING HELPERS ---
    [ContextMenu("Add 50 Progress Points (Test)")]
    public void TestAddProgress() { AddProgressPoints(50); }
    
    [ContextMenu("Add 500 Coins (Test)")]
    public void TestAddCoins() { AddCoins(500); }
    
    [ContextMenu("Reset Progress")]
    public void ResetProgress() { PlayerPrefs.DeleteAll(); currentLevel = 1; currentProgressPoints = 0; currentCoins = 0; UpdateGardenVisuals(); Debug.Log("Progress reset!"); }
}