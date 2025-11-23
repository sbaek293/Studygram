using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class GroupManager : MonoBehaviour
{
    public static GroupManager Instance;
    private DatabaseReference db;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        db = FirebaseDatabase.DefaultInstance.RootReference;
        DontDestroyOnLoad(gameObject);
    }

    // Create a new group with the user as first member. Returns new groupId
    public async Task<string> CreateNewGroup(string userId, MatchingProfile userProfile)
    {
        string newGroupId = db.Child("groups").Push().Key;

        var users = new Dictionary<string, object> { { userId, true } };

        var groupData = new Dictionary<string, object>
        {
            { "groupId", newGroupId },
            { "size", 1 },
            { "averageScore", (int)ComputeProfileScore(userProfile) }, // optional aggregate numeric score
            { "points", 0 },
            { "users", users }
        };

        await db.Child("groups").Child(newGroupId).SetValueAsync(groupData);

        // mark user's active group & isGrouped
        await db.Child("users").Child(userId).Child("activeGroup").SetValueAsync(newGroupId);
        await db.Child("users").Child(userId).Child("isGrouped").SetValueAsync(true);

        return newGroupId;
    }

    // Add user to existing group and update size & averageScore
    public async Task AddUserToExistingGroup(string userId, string groupId, MatchingProfile userProfile)
    {
        var groupRef = db.Child("groups").Child(groupId);

        var snap = await groupRef.GetValueAsync();
        if (!snap.Exists)
        {
            Debug.LogError("Group does not exist: " + groupId);
            return;
        }

        int size = snap.Child("size").Exists ? Convert.ToInt32(snap.Child("size").Value) : 0;
        int avg = snap.Child("averageScore").Exists ? Convert.ToInt32(snap.Child("averageScore").Value) : 0;

        int newSize = size + 1;

        // recompute averageScore by including user's profile score (we use a simple numeric proxy)
        int userScoreValue = (int)ComputeProfileScore(userProfile);
        int newAvg = (avg * size + userScoreValue) / Math.Max(1, newSize);

        // write updates
        await groupRef.Child("users").Child(userId).SetValueAsync(true);
        await groupRef.Child("size").SetValueAsync(newSize);
        await groupRef.Child("averageScore").SetValueAsync(newAvg);

        await db.Child("users").Child(userId).Child("activeGroup").SetValueAsync(groupId);
        await db.Child("users").Child(userId).Child("isGrouped").SetValueAsync(true);

    }

    // Optional helper: compute a numeric score for a profile to store in group.averageScore
    // You can tune this mapping; here we simply average the attributes to a 0-100 scale.
    private float ComputeProfileScore(MatchingProfile p)
    {
        float sum = p.morningPerson + p.groupStudy + p.seriousness + p.talkative + p.visual + p.practical + p.theoretical;
        float avg = sum / 7f; // 1-10 scale
        return (avg / 10f) * 100f; // convert to 0-100
    }
}
