using System;
using System.Collections.Generic;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class UserManager : MonoBehaviour
{
    public static UserManager Instance;
    private DatabaseReference db;
    public event Action OnUserDataLoaded;

    // --- 1. GAME STATS (The Gamer) ---
    public int coins = 0;
    public int xp = 0;
    public int score = 0;

    // --- 2. IDENTITY (The Student) ---
    // ⭐ This is the fix! The profile is now part of the main user.
    public MatchingProfile matchingProfile; 

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

    // ⭐ Creates the User in Firebase if they don't exist
    public void CreateUserInFirebase(string username, string userClass)
    {
        string uid = AppContext.UserId;

        db.Child("users").Child(uid).GetValueAsync().ContinueWithOnMainThread(t =>
        {
            if (t.IsFaulted) return;

            if (!t.Result.Exists)
            {
                var data = new Dictionary<string, object>
                {
                    { "username", username },
                    { "class", userClass },
                    { "coins", 0 },
                    { "xp", 0 },
                    { "score", 0 },
                    { "isGrouped", false },
                    { "activeGroup", "" },
                    { "avatarId", "" }
                };

                // Note: We don't save a profile here yet because they haven't taken the quiz
                db.Child("users").Child(uid).SetValueAsync(data);
            }
        });
    }

    // ⭐ Loads BOTH Stats and Profile at the same time
    public void LoadUserFromFirebase()
    {
        string uid = AppContext.UserId;
        Debug.Log("Loading User: " + uid);
        
        if (string.IsNullOrEmpty(uid)) return;

        db.Child("users").Child(uid).GetValueAsync().ContinueWithOnMainThread(t =>
        {
            if (t.IsFaulted || t.Result == null || !t.Result.Exists) return;

            var snap = t.Result;

            // 1. Load Game Stats
            coins = snap.Child("coins").Exists ? Convert.ToInt32(snap.Child("coins").Value) : 0;
            xp = snap.Child("xp").Exists ? Convert.ToInt32(snap.Child("xp").Value) : 0;
            score = snap.Child("score").Exists ? Convert.ToInt32(snap.Child("score").Value) : 0;

            // 2. Load Student Profile (The "Clone" is now merged here)
            if (snap.Child("profile").Exists)
            {
                string json = snap.Child("profile").GetRawJsonValue();
                matchingProfile = JsonUtility.FromJson<MatchingProfile>(json);
            }
            else
            {
                matchingProfile = new MatchingProfile(); // Default empty profile
            }

            Debug.Log($"User Loaded. Coins: {coins}, Profile Morning Score: {matchingProfile.morningPerson}");
            OnUserDataLoaded?.Invoke();
        });
    }

    // --- Helper Methods ---

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
}