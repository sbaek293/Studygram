using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "QuizQuestion", menuName = "Studygram/Quiz Question")]
public class QuizQuestion : ScriptableObject
{
    public string questionText;
    public QuestionType questionType;
    public List<QuizAnswer> answers = new List<QuizAnswer>();
    
    [Header("Matching Settings")]
    [Tooltip("How important is this question for matching? (1-10)")]
    [Range(1, 10)]
    public int matchingWeight = 5;
}

[System.Serializable]
public class QuizAnswer
{
    public string answerText;
    public Sprite answerIcon; // Optional icon for visual answers
    public MatchingProfile profile;
}

[System.Serializable]
public class MatchingProfile
{
    [Header("Study Preferences")]
    [Range(0, 10)] public int morningPerson = 5;  // 0 = night owl, 10 = morning person
    [Range(0, 10)] public int groupStudy = 5;     // 0 = solo, 10 = group
    [Range(0, 10)] public int seriousness = 5;    // 0 = casual, 10 = serious
    [Range(0, 10)] public int talkative = 5;      // 0 = quiet, 10 = chatty
    
    [Header("Study Style")]
    [Range(0, 10)] public int visual = 5;         // Visual learner
    [Range(0, 10)] public int practical = 5;      // Hands-on learner
    [Range(0, 10)] public int theoretical = 5;    // Conceptual learner
}

public enum QuestionType
{
    StudyTime,
    StudyStyle,
    Communication,
    Goals,
    Personality
}
