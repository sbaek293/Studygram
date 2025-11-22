using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class GroupDataManager : MonoBehaviour
{
    public static GroupDataManager Instance;

    private DatabaseReference db;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        db = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // ============================================================
    //  AUTO MATCHMAKING + FULL SOCIAL GROUP FEATURES
    // ============================================================
    public async Task AssignUserToGroup(string username, string userClass, int score)
    {
        string userId = AppContext.UserId;

        // -----------------------------------------------------------------
        // Step 1 — If user already has group, load it into AppContext
        // -----------------------------------------------------------------
        var userGroupSnap = await db.Child("userGroups").Child(userId).GetValueAsync();
        if (userGroupSnap.Exists)
        {
            string existingGroup = userGroupSnap.Value.ToString();
            AppContext.SetCurrentGroup(existingGroup);
            Debug.Log("User already belongs to: " + existingGroup);
            return;
        }

        // -----------------------------------------------------------------
        // Step 2 — Search all compatible groups of same class
        // -----------------------------------------------------------------
        var groupsSnap = await db.Child("groups").GetValueAsync();
        List<AutoGroupData> candidates = new();

        if (groupsSnap.Exists)
        {
            foreach (var g in groupsSnap.Children)
            {
                if (!g.HasChild("class")) continue;

                string c = g.Child("class").Value.ToString();
                if (c != userClass) continue;

                int size = g.Child("size").Exists ? int.Parse(g.Child("size").Value.ToString()) : 0;
                if (size >= 4) continue;

                int avgScore = g.Child("averageScore").Exists ? int.Parse(g.Child("averageScore").Value.ToString()) : 0;

                candidates.Add(new AutoGroupData
                {
                    groupId = g.Key,
                    averageScore = avgScore,
                    size = size
                });
            }
        }

        string chosenGroupId;

        // -----------------------------------------------------------------
        // Step 3 — Create new group if no candidates
        // -----------------------------------------------------------------
        if (candidates.Count == 0)
        {
            chosenGroupId = "group_" + System.Guid.NewGuid().ToString("N").Substring(0, 6);

            await CreateNewGroup(chosenGroupId, username, userClass, score, userId);
        }
        else
        {
            // -----------------------------------------------------------------
            // Step 4 — Choose best match (closest score difference)
            // -----------------------------------------------------------------
            var best = candidates.OrderBy(g => Mathf.Abs(g.averageScore - score)).First();
            chosenGroupId = best.groupId;

            await AddUserToExistingGroup(best, chosenGroupId, username, score, userId);
        }

        // -----------------------------------------------------------------
        // Step 5 — Save in AppContext + reverse lookup
        // -----------------------------------------------------------------
        AppContext.SetCurrentGroup(chosenGroupId);

        await db.Child("userGroups").Child(userId).SetValueAsync(chosenGroupId);
        await db.Child("users").Child(userId).Child("activeGroup").SetValueAsync(chosenGroupId);

        Debug.Log("Assigned to group: " + chosenGroupId);
    }

    // ============================================================
    // CREATE NEW GROUP (FULL SOCIAL STRUCTURE INCLUDED)
    // ============================================================
    private async Task CreateNewGroup(string groupId, string username, string userClass, int score, string userId)
    {
        var groupData = new Dictionary<string, object>
        {
            { "class", userClass },
            { "size", 1 },
            { "averageScore", score },
            { "name", $"Auto-Group {userClass} #{Random.Range(100,999)}" }
        };

        await db.Child("groups").Child(groupId).SetValueAsync(groupData);

        // Add member entry
        await db.Child("groups").Child(groupId).Child("members").Child(userId).SetValueAsync(new Dictionary<string, object>
        {
            { "username", username },
            { "streak", 0 },
            { "envLevel", 1 }
        });

        // Create streak container
        await db.Child("groups").Child(groupId).Child("streak").SetValueAsync(new Dictionary<string, object>
        {
            { "currentStreak", 1 },
            { "lastUpdateDate", System.DateTime.UtcNow.ToString("yyyy-MM-dd") }
        });

        // Create leaderboard entry
        await db.Child("groups").Child(groupId).Child("leaderboard").Child(userId).SetValueAsync(new Dictionary<string, object>
        {
            { "score", score },
            { "name", username }
        });
    }

    // ============================================================
    // ADD USER TO EXISTING GROUP + UPDATE STATS
    // ============================================================
    private async Task AddUserToExistingGroup(AutoGroupData group, string groupId, string username, int newScore, string userId)
    {
        int newSize = group.size + 1;
        int newAvg = (group.averageScore * group.size + newScore) / newSize;

        // Update group stats
        await db.Child("groups").Child(groupId).Child("size").SetValueAsync(newSize);
        await db.Child("groups").Child(groupId).Child("averageScore").SetValueAsync(newAvg);

        // Add member
        await db.Child("groups").Child(groupId).Child("members").Child(userId).SetValueAsync(new Dictionary<string, object>
        {
            { "username", username },
            { "streak", 0 },
            { "envLevel", 1 }
        });

        // Add to leaderboard
        await db.Child("groups").Child(groupId).Child("leaderboard").Child(userId).SetValueAsync(new Dictionary<string, object>
        {
            { "score", newScore },
            { "name", username }
        });
    }

    private class AutoGroupData
    {
        public string groupId;
        public int averageScore;
        public int size;
    }
}
