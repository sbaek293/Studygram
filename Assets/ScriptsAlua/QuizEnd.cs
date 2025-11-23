using System.Threading.Tasks;
using Firebase.Database;
using UnityEngine;

public class QuizEnd : MonoBehaviour
{
    public StudentMatcher studentMatcher;
    public GroupManager groupManager;

    private DatabaseReference db;

    private void Start()
    {
        db = FirebaseDatabase.DefaultInstance.RootReference;

        if (studentMatcher == null) studentMatcher = GetComponent<StudentMatcher>();
        if (groupManager == null) groupManager = GroupManager.Instance;
    }

    public async Task OnQuizFinished_SaveProfileAndMatch(MatchingProfile profile)
    {
        // Use local user ID instead of Firebase Auth
        string userId = GetOrCreateLocalUserId();
        
        Debug.Log($"Processing quiz for user: {userId}");

        // Save profile
        await db.Child("users").Child(userId).Child("profile").SetRawJsonValueAsync(JsonUtility.ToJson(profile));

        // Save score
        int numericScore = (int)((profile.morningPerson + profile.groupStudy + profile.seriousness +
                                   profile.talkative + profile.visual + profile.practical + profile.theoretical) / 7f);
        await db.Child("users").Child(userId).Child("score").SetValueAsync(numericScore);

        // Find or create group
        string bestGroupId = await studentMatcher.FindBestGroupForStudent(userId, profile, maxGroupSize: 4);

        if (string.IsNullOrEmpty(bestGroupId))
        {
            string created = await groupManager.CreateNewGroup(userId, profile);
            Debug.Log("Created new group: " + created);
        }
        else
        {
            await groupManager.AddUserToExistingGroup(userId, bestGroupId, profile);
            Debug.Log("Added user to existing group: " + bestGroupId);
        }
    }
    
    private string GetOrCreateLocalUserId()
    {
        string userId = PlayerPrefs.GetString("LocalUserId", "");
        
        if (string.IsNullOrEmpty(userId))
        {
            userId = "user_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            PlayerPrefs.SetString("LocalUserId", userId);
            PlayerPrefs.Save();
            Debug.Log($"Created new local user ID: {userId}");
        }
        
        return userId;
    }
}