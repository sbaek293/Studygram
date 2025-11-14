using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Displays garden level and XP progress in UI
/// Shows level-up animations and notifications
/// </summary>
public class GardenUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GardenManager gardenManager;
    
    [Header("Level Display")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI xpText;
    
    [Header("Progress Bar")]
    [SerializeField] private Slider xpProgressBar;
    [SerializeField] private Image progressFillImage;
    [SerializeField] private Gradient progressGradient; // Optional: Color changes with level
    
    [Header("Level Up Notification")]
    [SerializeField] private GameObject levelUpPanel;
    [SerializeField] private TextMeshProUGUI levelUpText;
    [SerializeField] private float notificationDuration = 3f;
    
    [Header("Area Unlock Notification")]
    [SerializeField] private GameObject areaUnlockPanel;
    [SerializeField] private TextMeshProUGUI areaUnlockText;
    
    private float notificationTimer = 0f;
    
    void Start()
    {
        // Auto-find garden manager if not assigned
        if (gardenManager == null)
        {
            gardenManager = FindObjectOfType<GardenManager>();
        }
        
        // Subscribe to events
        if (gardenManager != null)
        {
            gardenManager.onLevelUp.AddListener(OnGardenLevelUp);
            gardenManager.onAreaUnlocked.AddListener(OnAreaUnlocked);
        }
        
        // Hide notifications initially
        if (levelUpPanel != null) levelUpPanel.SetActive(false);
        if (areaUnlockPanel != null) areaUnlockPanel.SetActive(false);
        
        UpdateUI();
    }
    
    void Update()
    {
        UpdateUI();
        
        // Handle notification timer
        if (notificationTimer > 0)
        {
            notificationTimer -= Time.deltaTime;
            if (notificationTimer <= 0)
            {
                HideNotifications();
            }
        }
    }
    
    void UpdateUI()
    {
        if (gardenManager == null) return;
        
        int level = gardenManager.GetCurrentLevel();
        int currentXP = gardenManager.GetCurrentXP();
        int xpForNext = gardenManager.GetXPForNextLevel();
        float progress = gardenManager.GetLevelProgress();
        
        // Update level text
        if (levelText != null)
        {
            levelText.text = $"Level {level}";
        }
        
        // Update XP text
        if (xpText != null)
        {
            xpText.text = $"{currentXP} / {xpForNext} XP";
        }
        
        // Update progress bar
        if (xpProgressBar != null)
        {
            xpProgressBar.value = progress;
        }
        
        // Optional: Change progress bar color based on level
        if (progressFillImage != null && progressGradient != null)
        {
            float gradientPos = (float)(level % 10) / 10f; // Cycle through gradient every 10 levels
            progressFillImage.color = progressGradient.Evaluate(gradientPos);
        }
    }
    
    void OnGardenLevelUp(int newLevel)
    {
        Debug.Log($"🎉 UI: Garden reached Level {newLevel}!");
        
        if (levelUpPanel != null && levelUpText != null)
        {
            levelUpPanel.SetActive(true);
            levelUpText.text = $"Level {newLevel}!";
            notificationTimer = notificationDuration;
            
            // Optional: Add scale animation
            LeanTween.cancel(levelUpPanel);
            levelUpPanel.transform.localScale = Vector3.zero;
            LeanTween.scale(levelUpPanel, Vector3.one, 0.5f).setEaseOutBack();
        }
    }
    
    void OnAreaUnlocked(string areaName)
    {
        Debug.Log($"🗺️ UI: New area unlocked: {areaName}");
        
        if (areaUnlockPanel != null && areaUnlockText != null)
        {
            areaUnlockPanel.SetActive(true);
            areaUnlockText.text = $"New Area Unlocked!\n{areaName}";
            notificationTimer = notificationDuration + 1f; // Show area unlocks longer
            
            // Optional: Add scale animation
            LeanTween.cancel(areaUnlockPanel);
            areaUnlockPanel.transform.localScale = Vector3.zero;
            LeanTween.scale(areaUnlockPanel, Vector3.one, 0.5f).setEaseOutBack();
        }
    }
    
    void HideNotifications()
    {
        if (levelUpPanel != null)
        {
            LeanTween.scale(levelUpPanel, Vector3.zero, 0.3f).setEaseInBack()
                .setOnComplete(() => levelUpPanel.SetActive(false));
        }
        
        if (areaUnlockPanel != null)
        {
            LeanTween.scale(areaUnlockPanel, Vector3.zero, 0.3f).setEaseInBack()
                .setOnComplete(() => areaUnlockPanel.SetActive(false));
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (gardenManager != null)
        {
            gardenManager.onLevelUp.RemoveListener(OnGardenLevelUp);
            gardenManager.onAreaUnlocked.RemoveListener(OnAreaUnlocked);
        }
    }
}
