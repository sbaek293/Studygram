using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;

    private DatabaseReference db;

    public string groupId = "groupA";

    public event Action<List<LeaderboardEntry>> OnLeaderboardUpdated;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        db = FirebaseDatabase.DefaultInstance.RootReference;
        ListenToLeaderboard();
    }

    // -------------------------------
    // PUBLIC: Update a player's score
    // -------------------------------
    public void SetScore(string userId, int newScore, string userName)
    {
        var data = new Dictionary<string, object>()
        {
            { "score", newScore },
            { "name", userName }
        };

        db.Child("groups").Child(groupId)
          .Child("leaderboard").Child(userId)
          .UpdateChildrenAsync(data);
    }

    // -------------------------------
    // LISTENER
    // -------------------------------
    private void ListenToLeaderboard()
    {
        db.Child("groups").Child(groupId).Child("leaderboard")
          .ValueChanged += (s, e) =>
          {
              if (e.Snapshot == null || !e.Snapshot.Exists)
              {
                  OnLeaderboardUpdated?.Invoke(new List<LeaderboardEntry>());
                  return;
              }

              List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

              foreach (var child in e.Snapshot.Children)
              {
                  int score = 0;
                  string name = "Unknown";

                  if (child.Child("score").Value != null)
                      score = Convert.ToInt32(child.Child("score").Value);

                  if (child.Child("name").Value != null)
                      name = child.Child("name").Value.ToString();

                  entries.Add(new LeaderboardEntry(child.Key, name, score));
              }

              // SORT DESCENDING (highest score first)
              entries.Sort((a, b) => b.score.CompareTo(a.score));

              OnLeaderboardUpdated?.Invoke(entries);
          };
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
