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

    public async Task<string> OnQuizFinished_SaveProfileAndMatch(MatchingProfile profile)
    {
        // ⭐ FIX: WE CHANGED THIS LINE
        // Instead of making a new random ID, we grab the one from the Login screen!
        string userId = AppContext.UserId; 

        // Safety check (optional but good practice)
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("❌ CRITICAL ERROR: No User ID found in AppContext! Did you skip the login scene?");
            return null;
        }
        
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
            return created; // RETURN THE NEW GROUP ID
        }
        else
        {
            await groupManager.AddUserToExistingGroup(userId, bestGroupId, profile);
            Debug.Log("Added user to existing group: " + bestGroupId);
            return bestGroupId; // RETURN THE EXISTING GROUP ID
        }
    }

    // ⭐ FIX: I DELETED the 'GetOrCreateLocalUserId' method completely.
    // We don't want it anymore because it creates duplicate users.
}