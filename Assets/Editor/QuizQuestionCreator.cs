using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Helper script to quickly create sample quiz questions
/// Use this in the Unity Editor to generate example questions
/// </summary>
public class QuizQuestionCreator : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Studygram/Create Sample Quiz Questions")]
    public static void CreateSampleQuestions()
    {
        // Create folder if it doesn't exist
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");
        if (!AssetDatabase.IsValidFolder("Assets/Resources/QuizQuestions"))
            AssetDatabase.CreateFolder("Assets/Resources", "QuizQuestions");
        
        CreateStudyTimeQuestion();
        CreateGroupPreferenceQuestion();
        CreateStudyStyleQuestion();
        CreateGoalIntensityQuestion();
        CreateCommunicationQuestion();
        CreateLearningStyleQuestion();
        CreateEnvironmentQuestion();
        CreateBreakStyleQuestion();
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("Sample quiz questions created in Assets/Resources/QuizQuestions/");
    }
    
    static void CreateStudyTimeQuestion()
    {
        QuizQuestion question = ScriptableObject.CreateInstance<QuizQuestion>();
        question.questionText = "When do you feel most productive when studying?";
        question.questionType = QuestionType.StudyTime;
        question.matchingWeight = 8;
        
        question.answers = new List<QuizAnswer>
        {
            new QuizAnswer
            {
                answerText = "Early morning (6-9 AM) ",
                profile = new MatchingProfile { morningPerson = 10, seriousness = 7 }
            },
            new QuizAnswer
            {
                answerText = "Mid-morning to afternoon (9 AM-5 PM) ",
                profile = new MatchingProfile { morningPerson = 7, seriousness = 6 }
            },
            new QuizAnswer
            {
                answerText = "Evening (5-10 PM) ",
                profile = new MatchingProfile { morningPerson = 4, seriousness = 6 }
            },
            new QuizAnswer
            {
                answerText = "Late night (10 PM-2 AM) ",
                profile = new MatchingProfile { morningPerson = 1, seriousness = 5 }
            }
        };
        
        AssetDatabase.CreateAsset(question, "Assets/Resources/QuizQuestions/Q1_StudyTime.asset");
    }
    
    static void CreateGroupPreferenceQuestion()
    {
        QuizQuestion question = ScriptableObject.CreateInstance<QuizQuestion>();
        question.questionText = "How do you prefer to study?";
        question.questionType = QuestionType.StudyStyle;
        question.matchingWeight = 10;
        
        question.answers = new List<QuizAnswer>
        {
            new QuizAnswer
            {
                answerText = "Always with a study group 👥",
                profile = new MatchingProfile { groupStudy = 10, talkative = 8 }
            },
            new QuizAnswer
            {
                answerText = "Sometimes with others, sometimes alone ",
                profile = new MatchingProfile { groupStudy = 6, talkative = 6 }
            },
            new QuizAnswer
            {
                answerText = "Mostly alone, occasional partner ",
                profile = new MatchingProfile { groupStudy = 3, talkative = 4 }
            },
            new QuizAnswer
            {
                answerText = "Always alone, I need silence ",
                profile = new MatchingProfile { groupStudy = 0, talkative = 2 }
            }
        };
        
        AssetDatabase.CreateAsset(question, "Assets/Resources/QuizQuestions/Q2_GroupPreference.asset");
    }
    
    static void CreateStudyStyleQuestion()
    {
        QuizQuestion question = ScriptableObject.CreateInstance<QuizQuestion>();
        question.questionText = "What's your approach during study sessions?";
        question.questionType = QuestionType.Personality;
        question.matchingWeight = 7;
        
        question.answers = new List<QuizAnswer>
        {
            new QuizAnswer
            {
                answerText = "Let's chat and learn together! ",
                profile = new MatchingProfile { talkative = 9, groupStudy = 8 }
            },
            new QuizAnswer
            {
                answerText = "Mix of discussion and quiet time ",
                profile = new MatchingProfile { talkative = 6, groupStudy = 6 }
            },
            new QuizAnswer
            {
                answerText = "Mostly quiet with occasional questions ",
                profile = new MatchingProfile { talkative = 3, groupStudy = 5 }
            },
            new QuizAnswer
            {
                answerText = "Complete silence, just focused work ",
                profile = new MatchingProfile { talkative = 1, groupStudy = 2 }
            }
        };
        
        AssetDatabase.CreateAsset(question, "Assets/Resources/QuizQuestions/Q3_StudyStyle.asset");
    }
    
    static void CreateGoalIntensityQuestion()
    {
        QuizQuestion question = ScriptableObject.CreateInstance<QuizQuestion>();
        question.questionText = "What's your goal for this class?";
        question.questionType = QuestionType.Goals;
        question.matchingWeight = 9;
        
        question.answers = new List<QuizAnswer>
        {
            new QuizAnswer
            {
                answerText = "A+ or bust! Going for excellence ",
                profile = new MatchingProfile { seriousness = 10, practical = 7 }
            },
            new QuizAnswer
            {
                answerText = "Solid grade and actually understand the material ",
                profile = new MatchingProfile { seriousness = 7, theoretical = 7 }
            },
            new QuizAnswer
            {
                answerText = "Just pass and learn something useful ",
                profile = new MatchingProfile { seriousness = 5, practical = 8 }
            },
            new QuizAnswer
            {
                answerText = "Pass the class, that's all I need ",
                profile = new MatchingProfile { seriousness = 3, practical = 6 }
            }
        };
        
        AssetDatabase.CreateAsset(question, "Assets/Resources/QuizQuestions/Q4_Goals.asset");
    }
    
    static void CreateCommunicationQuestion()
    {
        QuizQuestion question = ScriptableObject.CreateInstance<QuizQuestion>();
        question.questionText = "How do you like to communicate with study partners?";
        question.questionType = QuestionType.Communication;
        question.matchingWeight = 6;
        
        question.answers = new List<QuizAnswer>
        {
            new QuizAnswer
            {
                answerText = "Video calls all the way! ",
                profile = new MatchingProfile { talkative = 9, groupStudy = 8 }
            },
            new QuizAnswer
            {
                answerText = "Voice chat works great ",
                profile = new MatchingProfile { talkative = 7, groupStudy = 7 }
            },
            new QuizAnswer
            {
                answerText = "Text/chat is perfect for me ",
                profile = new MatchingProfile { talkative = 5, groupStudy = 6 }
            },
            new QuizAnswer
            {
                answerText = "In-person only ",
                profile = new MatchingProfile { talkative = 6, groupStudy = 9 }
            }
        };
        
        AssetDatabase.CreateAsset(question, "Assets/Resources/QuizQuestions/Q5_Communication.asset");
    }
    
    static void CreateLearningStyleQuestion()
    {
        QuizQuestion question = ScriptableObject.CreateInstance<QuizQuestion>();
        question.questionText = "How do you learn best?";
        question.questionType = QuestionType.StudyStyle;
        question.matchingWeight = 7;
        
        question.answers = new List<QuizAnswer>
        {
            new QuizAnswer
            {
                answerText = "Visual aids, diagrams, and videos ",
                profile = new MatchingProfile { visual = 10, theoretical = 6 }
            },
            new QuizAnswer
            {
                answerText = "Hands-on practice and examples ",
                profile = new MatchingProfile { practical = 10, visual = 5 }
            },
            new QuizAnswer
            {
                answerText = "Reading and understanding concepts ",
                profile = new MatchingProfile { theoretical = 10, visual = 4 }
            },
            new QuizAnswer
            {
                answerText = "Discussion and explaining to others ",
                profile = new MatchingProfile { talkative = 9, groupStudy = 9, theoretical = 6 }
            }
        };
        
        AssetDatabase.CreateAsset(question, "Assets/Resources/QuizQuestions/Q6_LearningStyle.asset");
    }
    
    static void CreateEnvironmentQuestion()
    {
        QuizQuestion question = ScriptableObject.CreateInstance<QuizQuestion>();
        question.questionText = "What's your ideal study environment?";
        question.questionType = QuestionType.Personality;
        question.matchingWeight = 6;
        
        question.answers = new List<QuizAnswer>
        {
            new QuizAnswer
            {
                answerText = "Coffee shop with some background noise ",
                profile = new MatchingProfile { talkative = 6, groupStudy = 7 }
            },
            new QuizAnswer
            {
                answerText = "Library - quiet but not isolated ",
                profile = new MatchingProfile { talkative = 4, groupStudy = 5, seriousness = 7 }
            },
            new QuizAnswer
            {
                answerText = "My room with music ",
                profile = new MatchingProfile { talkative = 3, groupStudy = 3 }
            },
            new QuizAnswer
            {
                answerText = "Silent study room, zero distractions ",
                profile = new MatchingProfile { talkative = 1, groupStudy = 2, seriousness = 9 }
            }
        };
        
        AssetDatabase.CreateAsset(question, "Assets/Resources/QuizQuestions/Q7_Environment.asset");
    }
    
    static void CreateBreakStyleQuestion()
    {
        QuizQuestion question = ScriptableObject.CreateInstance<QuizQuestion>();
        question.questionText = "How do you handle study breaks?";
        question.questionType = QuestionType.StudyStyle;
        question.matchingWeight = 5;
        
        question.answers = new List<QuizAnswer>
        {
            new QuizAnswer
            {
                answerText = "Power through! Minimal breaks ",
                profile = new MatchingProfile { seriousness = 9, practical = 7 }
            },
            new QuizAnswer
            {
                answerText = "Pomodoro technique (25 min work, 5 min break) ⏱",
                profile = new MatchingProfile { seriousness = 7, practical = 8 }
            },
            new QuizAnswer
            {
                answerText = "Flexible breaks when I need them ",
                profile = new MatchingProfile { seriousness = 5 }
            },
            new QuizAnswer
            {
                answerText = "Frequent breaks to stay fresh ",
                profile = new MatchingProfile { seriousness = 4, talkative = 6 }
            }
        };
        
        AssetDatabase.CreateAsset(question, "Assets/Resources/QuizQuestions/Q8_Breaks.asset");
    }
#endif
}
