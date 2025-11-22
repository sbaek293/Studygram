using UnityEngine;
using Firebase.Database;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class GroupManager : MonoBehaviour
{
    private DatabaseReference db;

    private void Start()
    {
        db = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public async Task AssignUserToGroup(string userId, string userClass, int score)
    {
        // Step 1: Check if user is already assigned
        var userGroupSnap = await db.Child("userGroups").Child(userId).GetValueAsync();
        if (userGroupSnap.Exists)
        {
            Debug.Log("User already in group: " + userGroupSnap.Value);
            return;
        }

        // Step 2: Get all groups of same class with less than 4 people
        var groupsSnap = await db.Child("groups").GetValueAsync();
        List<GroupData> possibleGroups = new();

        if (groupsSnap.Exists)
        {
            foreach (var g in groupsSnap.Children)
            {
                string c = g.Child("class").Value.ToString();
                int size = int.Parse(g.Child("size").Value.ToString());

                if (c == userClass && size < 4)
                {
                    int avgScore = int.Parse(g.Child("averageScore").Value.ToString());

                    possibleGroups.Add(new GroupData
                    {
                        groupId = g.Key,
                        averageScore = avgScore,
                        size = size
                    });
                }
            }
        }

        string selectedGroupId;

        // Step 3: If no group → create new one
        if (possibleGroups.Count == 0)
        {
            selectedGroupId = db.Child("groups").Push().Key;

            await db.Child("groups").Child(selectedGroupId).SetValueAsync(new Dictionary<string, object>
            {
                {"class", userClass},
                {"size", 1},
                {"averageScore", score},
                {"users/" + userId, true}
            });
        }
        else
        {
            // Step 4: Pick most compatible group (closest score)
            var bestGroup = possibleGroups
                .OrderBy(g => Mathf.Abs(g.averageScore - score))
                .First();

            selectedGroupId = bestGroup.groupId;

            // Update group info
            int newSize = bestGroup.size + 1;
            int newAvg = (bestGroup.averageScore * bestGroup.size + score) / newSize;

            await db.Child("groups").Child(selectedGroupId).Child("users").Child(userId).SetValueAsync(true);
            await db.Child("groups").Child(selectedGroupId).Child("size").SetValueAsync(newSize);
            await db.Child("groups").Child(selectedGroupId).Child("averageScore").SetValueAsync(newAvg);
        }

        // Step 5: Mark user as grouped
        await db.Child("users").Child(userId).Child("isGrouped").SetValueAsync(true);

        // Step 6: Add reverse lookup
        await db.Child("userGroups").Child(userId).SetValueAsync(selectedGroupId);

        Debug.Log("User assigned to group: " + selectedGroupId);
    }

    private class GroupData
    {
        public string groupId;
        public int averageScore;
        public int size;
    }
}
