using System;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class GroupStreakManager : MonoBehaviour
{
    public static GroupStreakManager Instance;
    private DatabaseReference db;

    public int currentStreak = 0;
    public DateTime lastUpdate;

    public event Action<int> OnStreakChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this; else Destroy(gameObject);
        db = FirebaseDatabase.DefaultInstance.RootReference;
    }

    private void Start()
    {
        // start listening if group already set
        if (!string.IsNullOrEmpty(AppContext.CurrentGroupId))
            StartListening();
    }

    public void StartListening()
    {
        string gId = AppContext.CurrentGroupId;
        if (string.IsNullOrEmpty(gId)) return;

        db.Child("groups").Child(gId).Child("streak").ValueChanged += (s, e) =>
        {
            if (e.DatabaseError != null || e.Snapshot == null || !e.Snapshot.Exists) return;

            currentStreak = e.Snapshot.Child("currentStreak").Exists ? Convert.ToInt32(e.Snapshot.Child("currentStreak").Value) : 0;
            if (e.Snapshot.Child("lastUpdateDate").Exists)
                lastUpdate = DateTime.Parse(e.Snapshot.Child("lastUpdateDate").Value.ToString());

            OnStreakChanged?.Invoke(currentStreak);
        };
    }

    public void ExtendStreakIfNeeded()
    {
        string gId = AppContext.CurrentGroupId;
        if (string.IsNullOrEmpty(gId)) return;

        db.Child("groups").Child(gId).Child("streak").GetValueAsync().ContinueWithOnMainThread(t =>
        {
            if (t.IsFaulted || !t.Result.Exists)
            {
                SaveStreak(1, DateTime.UtcNow.Date);
                return;
            }

            var snap = t.Result;
            int old = snap.Child("currentStreak").Exists ? Convert.ToInt32(snap.Child("currentStreak").Value) : 0;
            DateTime last = snap.Child("lastUpdateDate").Exists ? DateTime.Parse(snap.Child("lastUpdateDate").Value.ToString()).Date : DateTime.UtcNow.AddDays(-2).Date;
            DateTime today = DateTime.UtcNow.Date;

            if (today == last) return;
            double diff = (today - last).TotalDays;
            SaveStreak(diff > 1 ? 1 : old + 1, today);
        });
    }

    private void SaveStreak(int value, DateTime date)
    {
        string gId = AppContext.CurrentGroupId;
        if (string.IsNullOrEmpty(gId)) return;

        var data = new System.Collections.Generic.Dictionary<string, object>
        {
            { "currentStreak", value },
            { "lastUpdateDate", date.ToString("yyyy-MM-dd") }
        };

        db.Child("groups").Child(gId).Child("streak").SetValueAsync(data);
    }
}
