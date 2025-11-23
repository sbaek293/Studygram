using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles the main menu interaction and scene navigation.
/// </summary>
public class MainMenuNavigation : MonoBehaviour
{
    [Header("Menu Panel References")]
    [SerializeField] private GameObject menuPanel; // The panel containing Social/Profile buttons
    
    [Header("Profile/Stats References")]
    [SerializeField] private ProfileUI profileUI; // Reference to the ProfileUI script

    [Header("Scene Names")]
    [SerializeField] private string groupGardenSceneName = "Group_Garden_Scene"; // Set this name in the Inspector

    private bool menuIsOpen = false;

    void Start()
    {
        // Ensure the menu is hidden and profile panel is deactivated on start
        if (menuPanel != null) menuPanel.SetActive(false);
        if (profileUI != null) profileUI.TogglePanel(false); 

        // CRITICAL: Check if the scene name is set
        if (string.IsNullOrEmpty(groupGardenSceneName) || groupGardenSceneName == "Group_Garden_Scene")
        {
            Debug.LogError("‼️ ERROR: groupGardenSceneName must be set in the Inspector!");
        }
    }

    // --- BUTTON FUNCTIONS ---

  public void ToggleMainMenu()
{
    // If profile panel is open, close it first
    if (profileUI != null && IsProfileOpen())
    {
        profileUI.TogglePanel(false);
        return; // Just close profile, don't open menu
    }
    
    menuIsOpen = !menuIsOpen;
    
    if (menuPanel != null)
    {
        menuPanel.SetActive(menuIsOpen);
    }
}

public void OpenProfilePanel()
{
    if (profileUI == null) return;
    
    // Toggle profile panel (if open, close it)
    bool isCurrentlyOpen = IsProfileOpen();
    profileUI.TogglePanel(!isCurrentlyOpen);
    
    // Close menu panel
    menuIsOpen = false;
    if (menuPanel != null)
    {
        menuPanel.SetActive(false);
    }
}

// Helper to check if profile is open
private bool IsProfileOpen()
{
    // You need to add a way to check this in ProfileUI
    return profileUI != null && profileUI.IsPanelOpen();
}

    public void GoToGroupGarden()
{
    // Check if user has completed quiz
    if (!HasUserProfile())
    {
        // No profile - load quiz scene first
        SceneManager.LoadScene("Quiz"); // Change to your exact scene name!
    }
    else
    {
        // Has profile - go straight to garden
        SceneManager.LoadScene(groupGardenSceneName);
    }
}

bool HasUserProfile()
{
    return PlayerPrefs.GetInt("ProfileCompleted", 0) == 1;
}


void ShowQuiz()
{
    // Load quiz scene or show quiz panel
    SceneManager.LoadScene("Quiz"); // If quiz is separate scene
    
    // OR if quiz is in same scene:
    // QuizManager quiz = FindObjectOfType<QuizManager>();
    // quiz.InitializeQuiz();
}
}