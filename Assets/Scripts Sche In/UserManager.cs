using System;
using System.Collections.Generic;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class UserManager : MonoBehaviour
{
    public static UserManager Instance;
    private DatabaseReference db;

    public int coins = 0;
    public int xp = 0;
    public int score = 0;

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

    private void Start()
    {
        AppContext.LoadPersistedGroup();
        if (!string.IsNullOrEmpty(AppContext.UserId))
            LoadUserFromFirebase();
    }

    // ⭐ NEW — creates the Firebase user (used after login)
    public void CreateUserInFirebase(string username, string userClass)
    {
        string uid = AppContext.UserId;

        var data = new Dictionary<string, object>
        {
            { "username", username },
            { "class", userClass },
            { "coins", 0 },
            { "xp", 0 },
            { "score", 0 },
            { "activeGroup", "" }
        };

        db.Child("users").Child(uid).SetValueAsync(data);
    }

    public void LoadUserFromFirebase()
    {
        string uid = AppContext.UserId;
        if (string.IsNullOrEmpty(uid)) return;

        db.Child("users").Child(uid).GetValueAsync().ContinueWithOnMainThread(t =>
        {
            if (t.IsFaulted || t.Result == null || !t.Result.Exists) return;

            var snap = t.Result;

            coins = snap.Child("coins").Exists ? Convert.ToInt32(snap.Child("coins").Value) : 0;
            xp = snap.Child("xp").Exists ? Convert.ToInt32(snap.Child("xp").Value) : 0;
            score = snap.Child("score").Exists ? Convert.ToInt32(snap.Child("score").Value) : 0;
        });
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        db.Child("users").Child(AppContext.UserId).Child("coins").SetValueAsync(coins);
    }

    public void AddXP(int amount)
    {
        xp += amount;
        db.Child("users").Child(AppContext.UserId).Child("xp").SetValueAsync(xp);
    }

    public void UpdateScore(int newScore)
    {
        score = newScore;
        db.Child("users").Child(AppContext.UserId).Child("score").SetValueAsync(score);
    }
}
