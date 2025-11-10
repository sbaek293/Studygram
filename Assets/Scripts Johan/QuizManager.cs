using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class QuizManager : MonoBehaviour
{
    [Header("Quiz Questions")]
    public List<QuizQuestion> allQuestions = new List<QuizQuestion>();
    
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
            quizQuestions = quizQuestions.OrderBy(x => Random.value).ToList();
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
        
        // Highlight selected answer (AnswerButton will handle its own highlight)
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
        
        // Save profile (you can implement this to save to PlayerPrefs, database, etc.)
        SaveUserProfile();
        
        // Show results panel
        if (quizPanel != null) quizPanel.SetActive(false);
        if (resultsPanel != null) resultsPanel.SetActive(true);
        
        // You can now use userProfile to match with other students
        Debug.Log("Quiz Complete! User Profile:");
        Debug.Log($"Morning Person: {userProfile.morningPerson}/10");
        Debug.Log($"Group Study: {userProfile.groupStudy}/10");
        Debug.Log($"Seriousness: {userProfile.seriousness}/10");
        Debug.Log($"Talkative: {userProfile.talkative}/10");
    }
    
    void SaveUserProfile()
    {
        // Save to PlayerPrefs (or your database)
        PlayerPrefs.SetInt("Profile_MorningPerson", userProfile.morningPerson);
        PlayerPrefs.SetInt("Profile_GroupStudy", userProfile.groupStudy);
        PlayerPrefs.SetInt("Profile_Seriousness", userProfile.seriousness);
        PlayerPrefs.SetInt("Profile_Talkative", userProfile.talkative);
        PlayerPrefs.SetInt("Profile_Visual", userProfile.visual);
        PlayerPrefs.SetInt("Profile_Practical", userProfile.practical);
        PlayerPrefs.SetInt("Profile_Theoretical", userProfile.theoretical);
        PlayerPrefs.Save();
        
        // TODO: Send to your backend/Firebase for matching algorithm
    }
    
    public MatchingProfile GetUserProfile()
    {
        return userProfile;
    }
    
    // Call this to restart the quiz
    public void RestartQuiz()
    {
        InitializeQuiz();
    }
}
