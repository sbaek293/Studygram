using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
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

    DatabaseReference db;

   private async void Start()
{
    db = FirebaseDatabase.DefaultInstance.RootReference;

    // Ensure user is signed in
    if (FirebaseAuth.DefaultInstance.CurrentUser == null)
    {
        await FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync();
        Debug.Log("Signed in anonymously as: " + FirebaseAuth.DefaultInstance.CurrentUser.UserId);
    }
}


    /// <summary>
    /// Call this after quiz finishes.
    /// </summary>
    public async void AssignUserToRandomGroup(string userId, string className)
    {
        // prevent double placement
        var existing = await db.Child("userGroups").Child(userId).GetValueAsync();
        if (existing.Exists)
        {
            Debug.Log("User already in group: " + existing.Value.ToString());
            return;
        }

        // get all groups of same class
        var snapshot = await db.Child("groups").GetValueAsync();

        List<(string groupId, int size)> candidateGroups = new List<(string, int)>();

        if (snapshot.Exists)
        {
            foreach (var group in snapshot.Children)
            {
                string gClass = group.Child("className").Value?.ToString();
                int size = int.Parse(group.Child("size").Value?.ToString() ?? "0");

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

        Debug.Log("User assigned to group: " + selectedGroupId);
    }

public void AssignMe()
{
    string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

    // Get className from Firebase
    FirebaseDatabase.DefaultInstance
        .GetReference("Users")
        .Child(userId)
        .Child("className")
        .GetValueAsync()
        .ContinueWith(task =>
        {
            if (task.IsFaulted || !task.Result.Exists)
            {
                Debug.LogError("Failed to load className from Firebase.");
                return;
            }

            string className = task.Result.Value.ToString();

            // Call your already-written random group assigner
            AssignUserToRandomGroup(userId, className);
        });
}
}