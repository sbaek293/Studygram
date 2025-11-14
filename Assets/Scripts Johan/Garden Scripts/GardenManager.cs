using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// Hybrid Garden System: Levels up (visual improvements) AND unlocks new areas (expansion)
/// Perfect for shared study group gardens that evolve over time
/// </summary>
public class GardenManager : MonoBehaviour
{
    [System.Serializable]
    public class GardenArea
    {
        public string areaName;
        public GameObject areaObject; // The GameObject containing this area's objects
        public int unlockAtLevel = 1; // What garden level unlocks this area
        [HideInInspector] public bool isUnlocked = false;
    }
    
    [System.Serializable]
    public class VisualTier
    {
        public int minLevel; // This tier applies from this level onwards
        public Color grassColor = new Color(0.6f, 0.9f, 0.6f);
        public Material grassMaterial; // Optional custom material
        public GameObject[] decorationsToEnable; // Objects that appear at this tier
        public ParticleSystem ambientParticles; // Optional sparkles, butterflies, etc.
    }
    
    [Header("XP & Leveling")]
    [SerializeField] private int currentXP = 0;
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int baseXPPerLevel = 100; // XP needed for level 1→2
    [SerializeField] private float xpScaling = 1.5f; // Each level needs 1.5x more XP
    
    [Header("Garden Areas (Expansion)")]
    [SerializeField] private List<GardenArea> areas = new List<GardenArea>();
    
    [Header("Visual Improvements (Level-based)")]
    [SerializeField] private List<VisualTier> visualTiers = new List<VisualTier>();
    [SerializeField] private SpriteRenderer gardenGround; // Main ground sprite to recolor
    
    [Header("Garden Settings")]
    [SerializeField] private Vector2 gardenSize = new Vector2(20, 20);
    
    [Header("References")]
    [SerializeField] private PetController pet;
    [SerializeField] private CameraFollow cameraFollow;
    
    [Header("Events")]
    public UnityEvent<int> onLevelUp; // Triggered when garden levels up
    public UnityEvent<string> onAreaUnlocked; // Triggered when new area unlocks
    
    void Start()
    {
        LoadProgress();
        
        // Setup camera boundaries
        if (cameraFollow != null)
        {
            cameraFollow.SetBoundaries(gardenSize.x, gardenSize.y, Vector2.zero);
        }
        
        UpdateGardenVisuals();
        UpdateAreaVisibility();
    }
    
    /// <summary>
    /// Add XP to the garden (from study sessions, completing tasks, etc.)
    /// </summary>
    public void AddXP(int xp)
    {
        currentXP += xp;
        Debug.Log($"Garden gained {xp} XP! Total: {currentXP}/{GetXPForNextLevel()}");
        
        // Check for level up
        while (currentXP >= GetXPForNextLevel())
        {
            LevelUp();
        }
        
        SaveProgress();
    }
    
    void LevelUp()
    {
        currentXP -= GetXPForNextLevel();
        currentLevel++;
        
        Debug.Log($"🎉 GARDEN LEVELED UP! Now Level {currentLevel}");
        
        // Update visuals based on new level
        UpdateGardenVisuals();
        
        // Check for area unlocks
        CheckAreaUnlocks();
        
        // Trigger event for UI notifications, celebrations, etc.
        onLevelUp?.Invoke(currentLevel);
        
        SaveProgress();
    }
    
    public int GetXPForNextLevel()
    {
        // Progressive XP curve: Level 2 needs 100, Level 3 needs 150, Level 4 needs 225, etc.
        return Mathf.RoundToInt(baseXPPerLevel * Mathf.Pow(xpScaling, currentLevel - 1));
    }
    
    void CheckAreaUnlocks()
    {
        for (int i = 0; i < areas.Count; i++)
        {
            if (!areas[i].isUnlocked && currentLevel >= areas[i].unlockAtLevel)
            {
                UnlockArea(i);
            }
        }
    }
    
    void UnlockArea(int areaIndex)
    {
        if (areaIndex >= 0 && areaIndex < areas.Count)
        {
            areas[areaIndex].isUnlocked = true;
            
            if (areas[areaIndex].areaObject != null)
            {
                areas[areaIndex].areaObject.SetActive(true);
            }
            
            Debug.Log($"🗺️ NEW AREA UNLOCKED: {areas[areaIndex].areaName}!");
            onAreaUnlocked?.Invoke(areas[areaIndex].areaName);
            
            SaveProgress();
        }
    }
    
    void UpdateAreaVisibility()
    {
        for (int i = 0; i < areas.Count; i++)
        {
            if (areas[i].areaObject != null)
            {
                areas[i].areaObject.SetActive(areas[i].isUnlocked);
            }
        }
    }
    
    void UpdateGardenVisuals()
    {
        // Find the highest tier that applies to current level
        VisualTier currentTier = null;
        for (int i = visualTiers.Count - 1; i >= 0; i--)
        {
            if (currentLevel >= visualTiers[i].minLevel)
            {
                currentTier = visualTiers[i];
                break;
            }
        }
        
        if (currentTier != null)
        {
            // Update ground color/material
            if (gardenGround != null)
            {
                gardenGround.color = currentTier.grassColor;
                
                if (currentTier.grassMaterial != null)
                {
                    gardenGround.material = currentTier.grassMaterial;
                }
            }
            
            // Enable tier-specific decorations
            if (currentTier.decorationsToEnable != null)
            {
                foreach (GameObject deco in currentTier.decorationsToEnable)
                {
                    if (deco != null)
                    {
                        deco.SetActive(true);
                    }
                }
            }
            
            // Enable ambient particles
            if (currentTier.ambientParticles != null)
            {
                currentTier.ambientParticles.gameObject.SetActive(true);
            }
        }
    }
    
    // Public getters
    public int GetCurrentLevel() => currentLevel;
    public int GetCurrentXP() => currentXP;
    public float GetLevelProgress() => (float)currentXP / GetXPForNextLevel();
    
    public bool IsAreaUnlocked(string areaName)
    {
        foreach (var area in areas)
        {
            if (area.areaName == areaName)
            {
                return area.isUnlocked;
            }
        }
        return false;
    }
    
    // Save/Load
    void SaveProgress()
    {
        PlayerPrefs.SetInt("GardenLevel", currentLevel);
        PlayerPrefs.SetInt("GardenXP", currentXP);
        
        for (int i = 0; i < areas.Count; i++)
        {
            PlayerPrefs.SetInt($"Area_{i}_Unlocked", areas[i].isUnlocked ? 1 : 0);
        }
        
        PlayerPrefs.Save();
    }
    
    void LoadProgress()
    {
        currentLevel = PlayerPrefs.GetInt("GardenLevel", 1);
        currentXP = PlayerPrefs.GetInt("GardenXP", 0);
        
        for (int i = 0; i < areas.Count; i++)
        {
            // First area always starts unlocked
            bool defaultUnlocked = areas[i].unlockAtLevel <= 1;
            areas[i].isUnlocked = PlayerPrefs.GetInt($"Area_{i}_Unlocked", defaultUnlocked ? 1 : 0) == 1;
        }
    }
    
    // Testing helpers
    [ContextMenu("Add 50 XP (Test)")]
    public void TestAddXP()
    {
        AddXP(50);
    }
    
    [ContextMenu("Level Up (Test)")]
    public void TestLevelUp()
    {
        AddXP(GetXPForNextLevel());
    }
    
    [ContextMenu("Reset Progress")]
    public void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
        currentLevel = 1;
        currentXP = 0;
        
        for (int i = 0; i < areas.Count; i++)
        {
            areas[i].isUnlocked = areas[i].unlockAtLevel <= 1;
        }
        
        UpdateGardenVisuals();
        UpdateAreaVisibility();
        
        Debug.Log("Garden progress reset!");
    }
}
