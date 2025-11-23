using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;

public class QuizEnd : MonoBehaviour
{
    public StudentMatcher studentMatcher; // drag in Inspector
    public GroupManager groupManager;     // drag in Inspector

    private DatabaseReference db;

    private void Start()
    {
        db = FirebaseDatabase.DefaultInstance.RootReference;

        if (studentMatcher == null) studentMatcher = GetComponent<StudentMatcher>();
        if (groupManager == null) groupManager = GroupManager.Instance;
    }

    // Call this when user finishes quiz
    public async Task OnQuizFinished_SaveProfileAndMatch(MatchingProfile profile)
    {
        var user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null)
        {
            Debug.LogError("No authenticated user.");
            return;
        }

        string userId = user.UserId;

        // 1) Save profile to /users/{uid}/profile (JSON)
        await db.Child("users").Child(userId).Child("profile").SetRawJsonValueAsync(JsonUtility.ToJson(profile));

        // 2) (Optional) update the user's numeric score field in users/score if you use it
        int numericScore = (int)((profile.morningPerson + profile.groupStudy + profile.seriousness +
                                   profile.talkative + profile.visual + profile.practical + profile.theoretical) / 7f);
        await db.Child("users").Child(userId).Child("score").SetValueAsync(numericScore);

        // 3) Ask StudentMatcher for the best existing groupId
        string bestGroupId = await studentMatcher.FindBestGroupForStudent(userId, profile, maxGroupSize: 4);

        // 4) If no group found, create a new group; otherwise add user to existing group
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

        // 5) Optionally load the group UI or navigate to "My Group" scene now
    }
}
