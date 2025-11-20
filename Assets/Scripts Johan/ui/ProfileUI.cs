using UnityEngine;
using TMPro;

/// <summary>
/// Manages the visibility and display of the user's personal stats panel.
/// </summary>
public class ProfileUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject profilePanel;
    [SerializeField] private TextMeshProUGUI streakText;
    [SerializeField] private TextMeshProUGUI energyText;

    private UserStatsManager statsManager;

    void Start()
    {
        // Find the persistent manager instance
        statsManager = UserStatsManager.Instance;
        if (statsManager == null)
        {
            Debug.LogError("UserStatsManager not found! Is it loaded and persistent?");
            return;
        }

        // Subscribe to events to update UI automatically
        statsManager.onStreakChanged.AddListener(UpdateStreakDisplay);
        statsManager.onEnergyChanged.AddListener(UpdateEnergyDisplay);

        // Perform initial UI update
        UpdateAllDisplays();
    }
    
    // --- Public Toggle Functions ---
    
    public void TogglePanel(bool shouldShow)
    {
        if (profilePanel != null)
        {
            profilePanel.SetActive(shouldShow);
            if (shouldShow)
            {
                UpdateAllDisplays();
            }
        }
    }

    // --- Update Methods ---
    
    private void UpdateAllDisplays()
    {
        if (statsManager != null)
        {
            UpdateStreakDisplay(statsManager.GetDailyStreak());
            UpdateEnergyDisplay(statsManager.GetCurrentEnergy());
        }
    }

    public bool IsPanelOpen()
    {
        return profilePanel != null && profilePanel.activeSelf;
    }
    
    private void UpdateStreakDisplay(int newStreak)
    {
        if (streakText != null)
        {
            streakText.text = $"Daily Streak: {newStreak} Days";
        }
    }

    private void UpdateEnergyDisplay(int currentEnergy)
    {
        if (energyText != null)
        {
            energyText.text = $"Energy: {currentEnergy} / {statsManager.GetMaxEnergy()}";
        }
    }
}