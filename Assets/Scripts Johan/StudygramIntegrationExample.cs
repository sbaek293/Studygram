using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Example script showing how to integrate the quiz system into your Studygram app
/// This demonstrates the full flow: quiz → save → match → display results
/// </summary>
public class StudygramIntegrationExample : MonoBehaviour
{
    [Header("References")]
    public QuizManager quizManager;
    public GameObject mainMenuPanel;
    public GameObject matchResultsPanel;
    
    [Header("Results UI")]
    public Transform matchesContainer;
    public GameObject matchCardPrefab;
    public TextMeshProUGUI userProfileSummary;
    
    // Simulated database of other students
    private List<StudentMatcher.StudentProfile> allStudents = new List<StudentMatcher.StudentProfile>();
    
    void Start()
    {
        // For demo: Create some fake students
        CreateFakeStudentDatabase();
    }
    
    /// <summary>
    /// Call this when user clicks "Start Quiz" button
    /// </summary>
    public void OnStartQuizClicked()
    {
        mainMenuPanel.SetActive(false);
        quizManager.InitializeQuiz();
    }
    
    /// <summary>
    /// Call this when quiz completes (put this at end of QuizManager.ShowResults())
    /// </summary>
    public void OnQuizComplete()
    {
        // Get the user's profile from the quiz
        MatchingProfile userProfile = quizManager.GetUserProfile();
        
        // Save to your backend
        SaveProfileToBackend(userProfile);
        
        // Find matches
        FindAndDisplayMatches(userProfile);
    }
    
    void SaveProfileToBackend(MatchingProfile profile)
    {
        // TODO: Replace with your actual API call
        Debug.Log("Saving profile to backend...");
        
        // Example: Convert to JSON
        string json = JsonUtility.ToJson(profile);
        Debug.Log($"Profile JSON: {json}");
        
        // Example: Call your API
        // StartCoroutine(PostToAPI("https://yourapi.com/profiles", json));
    }
    
    void FindAndDisplayMatches(MatchingProfile userProfile)
    {
        // In real app, load this from your backend
        // For now, using fake student database
        
        // Find top 5 matches
        List<StudentMatcher.StudentProfile> bestMatches = 
            StudentMatcher.FindBestMatches(userProfile, allStudents, 5);
        
        // Display user's own profile
        DisplayUserProfile(userProfile);
        
        // Display matches
        DisplayMatches(bestMatches, userProfile);
        
        // Show results panel
        if (matchResultsPanel != null)
        {
            matchResultsPanel.SetActive(true);
        }
    }
    
    void DisplayUserProfile(MatchingProfile profile)
    {
        if (userProfileSummary == null) return;
        
        string summary = "Your Study Style:\n\n";
        
        // Morning vs Night
        if (profile.morningPerson >= 7)
            summary += "🌅 Early Bird - You shine in the morning!\n";
        else if (profile.morningPerson <= 3)
            summary += "🌙 Night Owl - Late night is your peak time!\n";
        else
            summary += "☀️ Flexible - You adapt to any schedule\n";
        
        // Group vs Solo
        if (profile.groupStudy >= 7)
            summary += "👥 Group Lover - You thrive with others!\n";
        else if (profile.groupStudy <= 3)
            summary += "🧑 Solo Studier - You focus best alone\n";
        else
            summary += "🤝 Balanced - Mix of group and solo works\n";
        
        // Seriousness
        if (profile.seriousness >= 7)
            summary += "🎯 Highly Focused - Excellence is your goal!\n";
        else if (profile.seriousness <= 3)
            summary += "😊 Casual Learner - Learning should be fun\n";
        else
            summary += "⚖️ Balanced - Serious but not stressed\n";
        
        // Communication
        if (profile.talkative >= 7)
            summary += "💬 Chatty - Discussion helps you learn\n";
        else if (profile.talkative <= 3)
            summary += "🤫 Quiet - You prefer silent focus\n";
        else
            summary += "🗣️ Moderate - Some chat, some silence\n";
        
        userProfileSummary.text = summary;
    }
    
    void DisplayMatches(List<StudentMatcher.StudentProfile> matches, MatchingProfile userProfile)
    {
        // Clear previous matches
        foreach (Transform child in matchesContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create a card for each match
        foreach (StudentMatcher.StudentProfile match in matches)
        {
            GameObject card = Instantiate(matchCardPrefab, matchesContainer);
            
            // Setup the card (assumes your prefab has these components)
            TextMeshProUGUI nameText = card.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI scoreText = card.transform.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI reasonsText = card.transform.Find("ReasonsText")?.GetComponent<TextMeshProUGUI>();
            Button matchButton = card.GetComponent<Button>();
            
            if (nameText != null)
                nameText.text = match.studentName;
            
            if (scoreText != null)
            {
                scoreText.text = $"{match.compatibilityScore:F0}% Match";
                scoreText.text += "\n" + StudentMatcher.GetCompatibilityDescription(match.compatibilityScore);
            }
            
            if (reasonsText != null)
            {
                List<string> reasons = StudentMatcher.GetMatchingReasons(userProfile, match.profile);
                reasonsText.text = string.Join("\n", reasons);
            }
            
            if (matchButton != null)
            {
                matchButton.onClick.AddListener(() => OnMatchCardClicked(match));
            }
        }
    }
    
    void OnMatchCardClicked(StudentMatcher.StudentProfile student)
    {
        Debug.Log($"User clicked on match: {student.studentName}");
        
        // TODO: Show student profile details
        // TODO: Add "Send Study Request" button
        // TODO: Start chat with student
        // TODO: Add to study group
    }
    
    /// <summary>
    /// Creates fake students for testing
    /// In real app, load this from your backend/database
    /// </summary>
    void CreateFakeStudentDatabase()
    {
        // Student 1: Morning person, loves groups, very serious
        allStudents.Add(new StudentMatcher.StudentProfile
        {
            studentId = "student001",
            studentName = "Emma",
            profile = new MatchingProfile
            {
                morningPerson = 9,
                groupStudy = 8,
                seriousness = 9,
                talkative = 7,
                visual = 6,
                practical = 5,
                theoretical = 8
            }
        });
        
        // Student 2: Night owl, prefers solo, moderate
        allStudents.Add(new StudentMatcher.StudentProfile
        {
            studentId = "student002",
            studentName = "Alex",
            profile = new MatchingProfile
            {
                morningPerson = 2,
                groupStudy = 3,
                seriousness = 6,
                talkative = 4,
                visual = 8,
                practical = 7,
                theoretical = 5
            }
        });
        
        // Student 3: Flexible, loves discussion, practical learner
        allStudents.Add(new StudentMatcher.StudentProfile
        {
            studentId = "student003",
            studentName = "Sofia",
            profile = new MatchingProfile
            {
                morningPerson = 5,
                groupStudy = 7,
                seriousness = 7,
                talkative = 9,
                visual = 5,
                practical = 9,
                theoretical = 4
            }
        });
        
        // Student 4: Early bird, balanced, theoretical
        allStudents.Add(new StudentMatcher.StudentProfile
        {
            studentId = "student004",
            studentName = "Marcus",
            profile = new MatchingProfile
            {
                morningPerson = 8,
                groupStudy = 5,
                seriousness = 8,
                talkative = 5,
                visual = 6,
                practical = 4,
                theoretical = 9
            }
        });
        
        // Student 5: Night owl, loves groups, casual
        allStudents.Add(new StudentMatcher.StudentProfile
        {
            studentId = "student005",
            studentName = "Lily",
            profile = new MatchingProfile
            {
                morningPerson = 3,
                groupStudy = 9,
                seriousness = 4,
                talkative = 8,
                visual = 7,
                practical = 6,
                theoretical = 5
            }
        });
        
        // Add more students as needed...
    }
    
    /// <summary>
    /// Example: Load students from your backend
    /// </summary>
    void LoadStudentsFromBackend()
    {
        // TODO: Replace with your actual API call
        // StartCoroutine(GetFromAPI("https://yourapi.com/students"));
        
        // When data arrives, parse it:
        // foreach (var studentData in responseData)
        // {
        //     allStudents.Add(new StudentMatcher.StudentProfile
        //     {
        //         studentId = studentData.id,
        //         studentName = studentData.name,
        //         profile = JsonUtility.FromJson<MatchingProfile>(studentData.profileJson)
        //     });
        // }
    }
    
    /// <summary>
    /// Example: Retake quiz button
    /// </summary>
    public void OnRetakeQuizClicked()
    {
        matchResultsPanel.SetActive(false);
        quizManager.RestartQuiz();
    }
}
