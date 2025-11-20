using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Displays both passive progress (Level/Bar) and active currency (Coins).
/// </summary>
public class GardenUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GardenManager gardenManager;
    
    [Header("1. PROGRESS BAR (Passive Garden Progress)")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Slider progressSlider;
    
    [Header("2. COIN DISPLAY (Active Currency)")]
    [SerializeField] private TextMeshProUGUI coinText; 
    
    [Header("3. LEVEL UP NOTIFICATION")]
    [SerializeField] private GameObject levelUpPanel;
    [SerializeField] private TextMeshProUGUI levelUpText;
    [SerializeField] private float notificationDuration = 3f;
    
    private float notificationTimer = 0f;
    
    void Start()
    {
        if (gardenManager == null) gardenManager = FindObjectOfType<GardenManager>();
        
        if (gardenManager != null)
        {
            // Passive Progress listener
            gardenManager.onLevelUp.AddListener(OnGardenLevelUp);
            // Active Coin listener
            gardenManager.onCoinsChanged.AddListener(UpdateCoinUI); 
        }
        
        if (levelUpPanel != null) levelUpPanel.SetActive(false);
        
        // Initial draw
        UpdateProgressUI();
        UpdateCoinUI(gardenManager.GetCoins());
    }
    
    void Update()
    {
        UpdateProgressUI();
        
        // Handle notification timer
        if (notificationTimer > 0)
        {
            notificationTimer -= Time.deltaTime;
            if (notificationTimer <= 0) HideNotifications();
        }
    }
    
    void UpdateProgressUI()
    {
        if (gardenManager == null) return;
        
        int level = gardenManager.GetCurrentLevel();
        int currentPoints = gardenManager.GetCurrentProgressPoints();
        int pointsForNext = gardenManager.GetPointsForNextLevel();
        float progress = gardenManager.GetProgress();
        
        // Update level text
        if (levelText != null) levelText.text = $"Level {level}";
        
        // Update progress text
        if (progressText != null) progressText.text = $"{currentPoints} / {pointsForNext} Progress";
        
        // Update progress bar
        if (progressSlider != null) progressSlider.value = progress;
    }

    void UpdateCoinUI(int coins)
    {
        if (coinText != null) coinText.text = coins.ToString();
    }
    
    void OnGardenLevelUp(int newLevel)
    {
        Debug.Log($"🎉 UI: Garden reached Level {newLevel}!");
        
        if (levelUpPanel != null && levelUpText != null)
        {
            levelUpPanel.SetActive(true);
            levelUpText.text = $"Level {newLevel}!";
            notificationTimer = notificationDuration;
            
            // Optional: LeanTween animation removed for simplicity
        }
    }
    
    void HideNotifications()
    {
        if (levelUpPanel != null) levelUpPanel.SetActive(false);
    }
}