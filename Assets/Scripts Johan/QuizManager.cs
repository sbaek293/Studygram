using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Firebase.Database;
using Firebase.Extensions;

public class QuizManager : MonoBehaviour
{
    [Header("Quiz Questions")]
    public List<QuizQuestion> allQuestions = new List<QuizQuestion>();

    [Header("Group Matching")]
    public GroupMatchResultsUI groupResultsUI;
    
    [Header("UI References")]
    public TextMeshProUGUI questionText;
    public Transform answersContainer;
    public GameObject answerButtonPrefab;
    public Button nextButton;
    public GameObject quizPanel;
    public GameObject resultsPanel;
    
    [Header("Progress")]
    public Slider progressBar;
    public TextMeshProUGUI progressText;
    
    [Header("Settings")]
    public bool shuffleQuestions = true;
    public float animationDelay = 0.3f;
    
    private int currentQuestionIndex = 0;
    private List<QuizQuestion> quizQuestions;
    private MatchingProfile userProfile;
    private int totalWeight = 0;
    private QuizAnswer selectedAnswer;
    
    void Start()
    {
        InitializeQuiz();
    }
    
    public void InitializeQuiz()
    {
        // Initialize user profile
        userProfile = new MatchingProfile();
        
        // Setup questions
        quizQuestions = new List<QuizQuestion>(allQuestions);
        if (shuffleQuestions && quizQuestions.Count > 0)
        {
            quizQuestions = quizQuestions.OrderBy(x => UnityEngine.Random.value).ToList();
        }
        
        // Reset
        currentQuestionIndex = 0;
        totalWeight = 0;
        
        // Setup UI
        if (resultsPanel != null) resultsPanel.SetActive(false);
        if (quizPanel != null) quizPanel.SetActive(true);
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(false);
            nextButton.onClick.AddListener(OnNextButtonClicked);
        }
        
        DisplayCurrentQuestion();
    }
    
    void DisplayCurrentQuestion()
    {
        if (currentQuestionIndex >= quizQuestions.Count)
        {
            ShowResults();
            return;
        }
        
        QuizQuestion question = quizQuestions[currentQuestionIndex];
        
        // Update question text
        if (questionText != null)
        {
            questionText.text = question.questionText;
        }
        
        // Update progress
        UpdateProgress();
        
        // Clear previous answers
        foreach (Transform child in answersContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create answer buttons
        foreach (QuizAnswer answer in question.answers)
        {
            GameObject buttonObj = Instantiate(answerButtonPrefab, answersContainer);
            AnswerButton answerBtn = buttonObj.GetComponent<AnswerButton>();
            
            if (answerBtn != null)
            {
                answerBtn.Setup(answer, this);
            }
        }
        
        // Hide next button until answer is selected
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(false);
        }
        
        selectedAnswer = null;
    }
    
    public void OnAnswerSelected(QuizAnswer answer)
    {
        selectedAnswer = answer;
        
        // Show next button
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(true);
        }
    }
    
    void OnNextButtonClicked()
    {
        if (selectedAnswer == null) return;
        
        // Add this answer's profile to user's total profile
        QuizQuestion currentQuestion = quizQuestions[currentQuestionIndex];
        AddToProfile(selectedAnswer.profile, currentQuestion.matchingWeight);
        
        // Move to next question
        currentQuestionIndex++;
        DisplayCurrentQuestion();
    }
    
    void AddToProfile(MatchingProfile answerProfile, int weight)
    {
        totalWeight += weight;
        
        userProfile.morningPerson += answerProfile.morningPerson * weight;
        userProfile.groupStudy += answerProfile.groupStudy * weight;
        userProfile.seriousness += answerProfile.seriousness * weight;
        userProfile.talkative += answerProfile.talkative * weight;
        userProfile.visual += answerProfile.visual * weight;
        userProfile.practical += answerProfile.practical * weight;
        userProfile.theoretical += answerProfile.theoretical * weight;
    }
    
    void UpdateProgress()
    {
        float progress = (float)currentQuestionIndex / quizQuestions.Count;
        
        if (progressBar != null)
        {
            progressBar.value = progress;
        }
        
        if (progressText != null)
        {
            progressText.text = $"Question {currentQuestionIndex + 1} of {quizQuestions.Count}";
        }
    }
    
    void ShowResults()
    {
        // Normalize the profile by dividing by total weight
        if (totalWeight > 0)
        {
            userProfile.morningPerson = Mathf.RoundToInt((float)userProfile.morningPerson / totalWeight);
            userProfile.groupStudy = Mathf.RoundToInt((float)userProfile.groupStudy / totalWeight);
            userProfile.seriousness = Mathf.RoundToInt((float)userProfile.seriousness / totalWeight);
            userProfile.talkative = Mathf.RoundToInt((float)userProfile.talkative / totalWeight);
            userProfile.visual = Mathf.RoundToInt((float)userProfile.visual / totalWeight);
            userProfile.practical = Mathf.RoundToInt((float)userProfile.practical / totalWeight);
            userProfile.theoretical = Mathf.RoundToInt((float)userProfile.theoretical / totalWeight);
        }
        
        SaveUserProfile();
        
        // Find and display best groups
        FindBestGroup();

        if (quizPanel != null) quizPanel.SetActive(false);
        if (resultsPanel != null) resultsPanel.SetActive(true);
    }
    
    async void FindBestGroup()
    {
        QuizEnd quizEnd = FindObjectOfType<QuizEnd>();
        
        if (quizEnd == null)
        {
            Debug.LogError("QuizEnd not found! Add it to the scene.");
            ShowMockDataFallback();
            return;
        }
        
        try
        {
            // Get the group ID directly!
            string groupId = await quizEnd.OnQuizFinished_SaveProfileAndMatch(userProfile);
            
            if (string.IsNullOrEmpty(groupId))
            {
                Debug.LogError("Failed to create/join group!");
                ShowMockDataFallback();
                return;
            }
            
            Debug.Log($"Quiz completed! Group: {groupId}");
            
            // Display the group using the ID we already have
            await DisplayUserGroup(groupId);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error: {e.Message}\n{e.StackTrace}");
            ShowMockDataFallback();
        }
    }

    async Task DisplayUserGroup(string groupId)
    {
        Debug.Log($"=== DisplayUserGroup called with groupId: {groupId} ===");
        
        string userId = AppContext.UserId;
        
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("No local user ID found");
            ShowMockDataFallback();
            return;
        }
        
        Debug.Log($"UserId: {userId}");
        
        var db = FirebaseDatabase.DefaultInstance.RootReference;
        
        try
        {
            Debug.Log($"Fetching group from Firebase: {groupId}");
            var groupSnapshot = await db.Child("groups").Child(groupId).GetValueAsync();
            
            Debug.Log($"Group exists: {groupSnapshot.Exists}");
            
            if (!groupSnapshot.Exists)
            {
                Debug.LogError($"Group {groupId} not found in Firebase!");
                ShowMockDataFallback();
                return;
            }
            
            int size = groupSnapshot.Child("size").Exists ? System.Convert.ToInt32(groupSnapshot.Child("size").Value) : 1;
            int avgScore = groupSnapshot.Child("averageScore").Exists ? System.Convert.ToInt32(groupSnapshot.Child("averageScore").Value) : 50;
            
            Debug.Log($"Group size: {size}, avgScore: {avgScore}");
            
            List<string> memberNames = new List<string>();
            var usersNode = groupSnapshot.Child("users");
            
            Debug.Log($"Users node exists: {usersNode.Exists}, Children count: {usersNode.ChildrenCount}");
            
            foreach (var memberSnapshot in usersNode.Children)
            {
                string memberId = memberSnapshot.Key;
                Debug.Log($"Processing member: {memberId}");
                
                var memberDataSnapshot = await db.Child("users").Child(memberId).GetValueAsync();
                
                string memberName = memberDataSnapshot.Child("username").Exists ? 
        memberDataSnapshot.Child("username").Value.ToString() : memberId;
    
    Debug.Log($"Member name: {memberName}");
    memberNames.Add(memberName);
            }
            
            string displayName = size == 1 ? "Your New Group" : "Your Matched Group";
            Debug.Log($"Display name: {displayName}");
            Debug.Log($"Member names: {string.Join(", ", memberNames)}");

            // ---------------------------------------------------------
            // FIX START: Construct the 'groups' list needed for the UI
            // ---------------------------------------------------------
            
            // Create the single group data object from the Firebase results
            var matchData = new GroupMatchResultsUI.GroupMatchData
            {
                groupId = groupId,
                groupName = displayName,
                memberCount = size,
                maxMembers = 4, // Default max size
                compatibilityScore = (float)avgScore, 
                memberNames = memberNames,
                matchReasons = new List<string> { "Matched based on study compatibility" } // Generic reason
            };

            // Create the list expected by ShowGroupMatches
            List<GroupMatchResultsUI.GroupMatchData> groups = new List<GroupMatchResultsUI.GroupMatchData>();
            groups.Add(matchData);

            // ---------------------------------------------------------
            // FIX END
            // ---------------------------------------------------------
            
            Debug.Log($"About to show UI. groupResultsUI null? {groupResultsUI == null}");

            if (groupResultsUI != null)
            {
                Debug.Log("Calling ShowGroupMatches...");
                // Now 'groups' exists and is populated
                groupResultsUI.ShowGroupMatches(groups); 
                Debug.Log("ShowGroupMatches completed");
            }
            else
            {
                Debug.LogError("❌ groupResultsUI is NULL! Cannot display results!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error displaying group: {e.Message}\n{e.StackTrace}");
            ShowMockDataFallback();
        }
    }

    // Fallback to mock data if Firebase fails
    void ShowMockDataFallback()
    {
        Debug.Log("Showing mock data as fallback");
        
        List<GroupMatchResultsUI.GroupMatchData> mockGroups = new List<GroupMatchResultsUI.GroupMatchData>
        {
            new GroupMatchResultsUI.GroupMatchData
            {
                groupId = "group1",
                groupName = "Social Computing",
                memberCount = 3,
                maxMembers = 4,
                compatibilityScore = 87f,
                memberNames = new List<string> { "Alice", "Bob", "Carol" },
                matchReasons = new List<string> { "- Similar study schedules", "- Compatible goals", "- Morning study preference" }
            },
            new GroupMatchResultsUI.GroupMatchData
            {
                groupId = "group2",
                groupName = "CS Study Squad",
                memberCount = 2,
                maxMembers = 4,
                compatibilityScore = 72f,
                memberNames = new List<string> { "David", "Emma" },
                matchReasons = new List<string> { "- Compatible communication style", "- Similar intensity" }
            }
        };
        
        if (groupResultsUI != null)
        {
            groupResultsUI.ShowGroupMatches(mockGroups);
        }
    }

    string GenerateGroupName(MatchingProfile profile)
    {
        string[] prefixes = { "Study", "Focus", "Learn", "Academic" };
        string[] suffixes = { "Squad", "Crew", "Team", "Group", "Circle" };
        
        // Add time preference to name based on morningPerson score
        string timePreference = "";
        if (profile.morningPerson >= 7)
            timePreference = "Morning ";
        else if (profile.morningPerson <= 4)
            timePreference = "Night ";
        
        string prefix = prefixes[UnityEngine.Random.Range(0, prefixes.Length)];
        string suffix = suffixes[UnityEngine.Random.Range(0, suffixes.Length)];
        
        return $"{timePreference}{prefix} {suffix}";
    }

    void SaveUserProfile()
    {
        PlayerPrefs.SetInt("Profile_MorningPerson", userProfile.morningPerson);
        PlayerPrefs.SetInt("Profile_GroupStudy", userProfile.groupStudy);
        PlayerPrefs.SetInt("Profile_Seriousness", userProfile.seriousness);
        PlayerPrefs.SetInt("Profile_Talkative", userProfile.talkative);
        PlayerPrefs.SetInt("Profile_Visual", userProfile.visual);
        PlayerPrefs.SetInt("Profile_Practical", userProfile.practical);
        PlayerPrefs.SetInt("Profile_Theoretical", userProfile.theoretical);
        
        PlayerPrefs.SetInt("ProfileCompleted", 1);
        
        PlayerPrefs.Save();
    }
    
    public MatchingProfile GetUserProfile()
    {
        return userProfile;
    }
    
    public void RestartQuiz()
    {
        InitializeQuiz();
    }

    public void GoToGarden()
    {
        SceneManager.LoadScene("Garden"); 
    }
}

[System.Serializable]
public class GroupData
{
    public string id;
    public string name;
    public int size;
    public List<string> memberIds = new List<string>();
    public List<string> memberNames = new List<string>();
    public List<MatchingProfile> memberProfiles = new List<MatchingProfile>();
}