using Firebase.Database;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;
    private DatabaseReference db;
    public event Action<List<LeaderboardEntry>> OnLeaderboardUpdated;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        db = FirebaseDatabase.DefaultInstance.RootReference;
    }

    private void Start() => ListenToLeaderboard();

    private void ListenToLeaderboard()
    {
        string groupId = AppContext.CurrentGroupId;
        db.Child("groups").Child(groupId).Child("leaderboard").ValueChanged += (s, e) =>
        {
            List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
            if (e.Snapshot.Exists)
            {
                foreach (var child in e.Snapshot.Children)
                {
                    int score = child.Child("score").Value != null ? Convert.ToInt32(child.Child("score").Value) : 0;
                    string name = child.Child("name").Value?.ToString() ?? "Unknown";
                    entries.Add(new LeaderboardEntry(child.Key, name, score));
                }
                entries.Sort((a, b) => b.score.CompareTo(a.score));
            }
            OnLeaderboardUpdated?.Invoke(entries);
        };
    }

    public void SetScore(int newScore)
    {
        string groupId = AppContext.CurrentGroupId;
        db.Child("groups").Child(groupId).Child("leaderboard")
            .Child(AppContext.UserId)
            .UpdateChildrenAsync(new Dictionary<string, object>
            {
                { "score", newScore },
                { "name", AppContext.UserName }
            });
    }
}


[Serializable]
public class LeaderboardEntry
{
    public string userId;
    public string userName;
    public int score;

    public LeaderboardEntry(string id, string name, int score)
    {
        this.userId = id;
        this.userName = name;
        this.score = score;
    }
}
