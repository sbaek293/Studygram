using System;
using System.Collections.Generic;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class GroupStreakManager : MonoBehaviour
{
    public static GroupStreakManager Instance;

    public string groupId = "groupA";

    private DatabaseReference db;

    public int currentStreak = 0;
    public DateTime lastUpdate;

    public event Action<int> OnStreakChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        db = FirebaseDatabase.DefaultInstance.RootReference;
    }

    private void Start()
    {
        ListenToStreak();        // syncs changes
        AutoCheckStreak();       // extends automatically on app open
    }

    // ---------------------------------------------------------
    // LISTEN for streak updates (sync all members)
    // ---------------------------------------------------------
    private void ListenToStreak()
    {
        db.Child("groups").Child(groupId).Child("streak")
          .ValueChanged += (s, e) =>
          {
              if (e.DatabaseError != null || e.Snapshot == null || !e.Snapshot.Exists) return;

              currentStreak = e.Snapshot.Child("currentStreak").Exists
                    ? Convert.ToInt32(e.Snapshot.Child("currentStreak").Value)
                    : 0;

              if (e.Snapshot.Child("lastUpdateDate").Exists)
              {
                  string raw = e.Snapshot.Child("lastUpdateDate").Value.ToString();
                  lastUpdate = DateTime.Parse(raw);
              }

              OnStreakChanged?.Invoke(currentStreak);
          };
    }

    // ---------------------------------------------------------
    // AUTO — called on app start
    // ---------------------------------------------------------
    public void AutoCheckStreak()
    {
        ExtendStreakIfNeeded();
    }

    // ---------------------------------------------------------
    // UNIVERSAL STREAK UPDATE — Call this whenever needed
    // (app open, session end, daily quest complete etc.)
    // ---------------------------------------------------------
    public void ExtendStreakIfNeeded()
    {
        db.Child("groups").Child(groupId).Child("streak")
          .GetValueAsync().ContinueWithOnMainThread(t =>
          {
              if (t.IsFaulted)
              {
                  Debug.LogError("Could not load streak.");
                  return;
              }

              DataSnapshot snap = t.Result;

              // If streak does not exist → create new one
              if (!snap.Exists)
              {
                  SaveStreak(1, DateTime.UtcNow.Date);
                  return;
              }

              int oldStreak = snap.Child("currentStreak").Exists
                                ? Convert.ToInt32(snap.Child("currentStreak").Value)
                                : 0;

              string lastDateString = snap.Child("lastUpdateDate").Exists
                                ? snap.Child("lastUpdateDate").Value.ToString()
                                : DateTime.UtcNow.AddDays(-2).ToString("yyyy-MM-dd");

              DateTime last = DateTime.Parse(lastDateString).Date;
              DateTime today = DateTime.UtcNow.Date;

              // Already counted today → do nothing
              if (today == last)
              {
                  Debug.Log("Streak already counted today.");
                  return;
              }

              double daysBetween = (today - last).TotalDays;

              // Missed a day → RESET
              if (daysBetween > 1)
              {
                  SaveStreak(1, today);
              }
              else
              {
                  // Normal extension
                  SaveStreak(oldStreak + 1, today);
              }
          });
    }

    // ---------------------------------------------------------
    // Save streak to Firebase
    // ---------------------------------------------------------
    private void SaveStreak(int value, DateTime date)
    {
        var data = new Dictionary<string, object>()
        {
            { "currentStreak", value },
            { "lastUpdateDate", date.ToString("yyyy-MM-dd") }
        };

        db.Child("groups").Child(groupId).Child("streak")
          .SetValueAsync(data)
          .ContinueWithOnMainThread(t =>
          {
              Debug.Log("Streak updated to: " + value);
          });
    }
}
