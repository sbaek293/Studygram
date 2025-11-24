using UnityEngine;
using Firebase.Database;
using System.Collections.Generic;
using System.Threading.Tasks;

public class RandomGroupAssigner : MonoBehaviour
{
    public static RandomGroupAssigner Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    // REMOVED: Start() method that initializes DB. 
    // We will use FirebaseInit.DB or fetch it when needed, ensuring dependencies are ready.

    /// <summary>
    /// Call this button to test random assignment
    /// </summary>
    public void AssignMe()
    {
        if (!FirebaseInit.IsReady)
        {
            Debug.LogError("Firebase not ready yet! Wait for FirebaseInit.");
            return;
        }

        // CORRECTED: Get the ID from PlayerPrefs, matching QuizEnd.cs logic
        string userId = PlayerPrefs.GetString("LocalUserId", "");
        
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("No LocalUserId found. Run the Quiz first!");
            return;
        }

        // Note: You need to decide where "className" comes from. 
        // For testing, we can hardcode it or fetch it if you saved it.
        string className = "Social Computing"; 

        AssignUserToRandomGroup(userId, className);
    }

    public async void AssignUserToRandomGroup(string userId, string className)
    {
        var db = FirebaseDatabase.DefaultInstance.RootReference;

        // prevent double placement
        var existing = await db.Child("userGroups").Child(userId).GetValueAsync();
        if (existing.Exists)
        {
            Debug.Log("User already in group: " + existing.Value.ToString());
            return;
        }

        // get all groups
        var snapshot = await db.Child("groups").GetValueAsync();

        List<(string groupId, int size)> candidateGroups = new List<(string, int)>();

        if (snapshot.Exists)
        {
            foreach (var group in snapshot.Children)
            {
                // Safety check for null values
                string gClass = group.Child("className").Value?.ToString();
                string sizeStr = group.Child("size").Value?.ToString();
                int size = int.Parse(string.IsNullOrEmpty(sizeStr) ? "0" : sizeStr);

                if (gClass == className && size < 4)
                    candidateGroups.Add((group.Key, size));
            }
        }

        string selectedGroupId = "";

        if (candidateGroups.Count > 0)
        {
            // pick a random existing group
            var picked = candidateGroups[Random.Range(0, candidateGroups.Count)];
            selectedGroupId = picked.groupId;

            int newSize = picked.size + 1;

            // Update Group
            await db.Child("groups").Child(selectedGroupId).Child("users").Child(userId).SetValueAsync(true);
            await db.Child("groups").Child(selectedGroupId).Child("size").SetValueAsync(newSize);
        }
        else
        {
            // create new group
            selectedGroupId = db.Child("groups").Push().Key;

            await db.Child("groups").Child(selectedGroupId).Child("className").SetValueAsync(className);
            await db.Child("groups").Child(selectedGroupId).Child("size").SetValueAsync(1);
            await db.Child("groups").Child(selectedGroupId).Child("users").Child(userId).SetValueAsync(true);
        }

        // store reverse lookup
        await db.Child("userGroups").Child(userId).SetValueAsync(selectedGroupId);
        
        // Update user profile to know they have a group (Matches GroupManager logic)
        await db.Child("users").Child(userId).Child("activeGroup").SetValueAsync(selectedGroupId);

        Debug.Log("User assigned to random group: " + selectedGroupId);
    }
}