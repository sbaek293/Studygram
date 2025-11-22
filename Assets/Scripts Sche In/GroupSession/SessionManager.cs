using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance;
    public SessionPanelController panelController;

    [Header("Session State")]
    //public string currentUserId;
    public string currentSessionId;
    //public string groupId = "groupA";

    public bool isHost = false;
    public bool active = false;
    public bool paused = false;

    private double elapsedSeconds = 0;
    private float hostUpdateCounter = 0f;
    private const float hostUpdateInterval = 0.5f;

    private DatabaseReference dbRoot;

    // EVENTS for UI
    public event Action<double> OnTimerUpdated;
    public event Action<bool> OnActiveChanged;
    public event Action<bool> OnPausedChanged;
    public event Action<Dictionary<string, bool>> OnParticipantsChanged;
    public event Action<double> OnSessionEnded;
    
    private EventHandler<ValueChangedEventArgs> sessionListener;

    private IEnumerator Start()
    {
        // Wait for FirebaseInit to finish
        while (!FirebaseInit.IsReady)
            yield return null;

        dbRoot = FirebaseInit.DB;
        Debug.Log("SessionManager connected to Firebase");
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        dbRoot = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // ---------------------------------------------------------
    // CREATE SESSION (HOST)
    // ---------------------------------------------------------
    public void CreateSession()
    {
        currentSessionId = "session_" + Guid.NewGuid().ToString("N").Substring(0, 6);
        isHost = true;
        active = false;   // waiting lobby
        paused = false;
        elapsedSeconds = 0;

        var sessionData = new Dictionary<string, object>()
        {
            { "groupId", AppContext.CurrentGroupId },
            { "hostId", AppContext.UserId },
            { "active", false },
            { "paused", false },
            { "elapsedSeconds", 0.0 },
            { "createdAt", ServerValue.Timestamp },
        };

        var updates = new Dictionary<string, object>();
        updates[$"sessions/{currentSessionId}"] = sessionData;
        updates[$"sessions/{currentSessionId}/participants/{AppContext.UserId}"] = true;

        dbRoot.Child("sessions").Child(currentSessionId)
      .SetValueAsync(sessionData).ContinueWithOnMainThread(t =>
      {
          if (t.IsFaulted) Debug.LogError("Session creation failed: " + t.Exception);
          else
          {
              // Add host participant separately
              dbRoot.Child("sessions").Child(currentSessionId).Child("participants")
                    .Child(AppContext.UserId).SetValueAsync(true);

              Debug.Log("Session created: " + currentSessionId);
              StartListening();
              panelController.ShowSession();
          }
      });

    }

    // ---------------------------------------------------------
    // JOIN SESSION (GUEST)
    // ---------------------------------------------------------
    public void JoinSession(string sessionId)
    {
        currentSessionId = sessionId;
        isHost = false;

        dbRoot.Child("sessions").Child(currentSessionId)
            .Child("participants").Child(AppContext.UserId)
            .SetValueAsync(true)
            .ContinueWithOnMainThread(t =>
            {
                if (t.IsFaulted)
                {
                    Debug.LogError("JOIN FAILED: " + t.Exception);
                    return;
                }

                Debug.Log("Joined session: " + currentSessionId);
                StartListening();
                panelController.ShowSession();
            });
    }

    // ---------------------------------------------------------
    // LISTEN TO SESSION CHANGES
    // ---------------------------------------------------------
    private void StartListening()
    {
        var sessionRef = dbRoot.Child("sessions").Child(currentSessionId);

        // Keep a reference so we can detach later
        sessionListener = (s, e) =>
        {
            if (e.DatabaseError != null) return;

            if (!e.Snapshot.Exists) return;

            // Participants
            if (e.Snapshot.HasChild("participants"))
            {
                var dict = new Dictionary<string, bool>();
                foreach (var child in e.Snapshot.Child("participants").Children)
                    dict[child.Key] = true;

                OnParticipantsChanged?.Invoke(dict);
            }

            // Timer
            if (e.Snapshot.HasChild("elapsedSeconds"))
            {
                elapsedSeconds = Convert.ToDouble(e.Snapshot.Child("elapsedSeconds").Value);
                OnTimerUpdated?.Invoke(elapsedSeconds);
            }

            // Active
            if (e.Snapshot.HasChild("active"))
            {
                active = Convert.ToBoolean(e.Snapshot.Child("active").Value);
                OnActiveChanged?.Invoke(active);
            }

            // Paused
            if (e.Snapshot.HasChild("paused"))
            {
                paused = Convert.ToBoolean(e.Snapshot.Child("paused").Value);
                OnPausedChanged?.Invoke(paused);
            }

            if (e.Snapshot.HasChild("ended"))
            {
                bool ended = Convert.ToBoolean(e.Snapshot.Child("ended").Value);
                if (ended)
                {
                    double finalTime = Convert.ToDouble(e.Snapshot.Child("finalTime").Value);

                    int exp = Convert.ToInt32(e.Snapshot.Child("rewards").Child("exp").Value);
                    int coins = Convert.ToInt32(e.Snapshot.Child("rewards").Child("coins").Value);
                    int score = e.Snapshot.Child("rewards").Child("score").Exists ? Convert.ToInt32(e.Snapshot.Child("rewards").Child("score").Value) : 0;

                    UserManager.Instance.AddXP(exp);
                    UserManager.Instance.AddCoins(coins);
                    // Show popup
                    EndPopUpUI.Instance.Show(finalTime, exp, coins);

                    LeaderboardManager.Instance.SetScore(score);

                    return; // stop processing anything else
                }
            }

        };

        // Subscribe
        sessionRef.ValueChanged += sessionListener;
    }

    private void StopListening()
    {
        if (sessionListener != null)
        {
            var sessionRef = dbRoot.Child("sessions").Child(currentSessionId);
            sessionRef.ValueChanged -= sessionListener;
            sessionListener = null;
        }
    }


    // ---------------------------------------------------------
    // HOST TIMER UPDATE
    // ---------------------------------------------------------
    private void Update()
    {
        if (!isHost) return;
        if (!active) return;
        if (paused) return;

        elapsedSeconds += Time.deltaTime;

        hostUpdateCounter += Time.deltaTime;
        if (hostUpdateCounter >= hostUpdateInterval)
        {
            hostUpdateCounter = 0;
            dbRoot.Child("sessions").Child(currentSessionId)
                .Child("elapsedSeconds")
                .SetValueAsync(elapsedSeconds);
        }

        OnTimerUpdated?.Invoke(elapsedSeconds);
    }

    // ---------------------------------------------------------
    // HOST CONTROLS
    // ---------------------------------------------------------
    public void StartSession()
    {
        if (!isHost) return;

        dbRoot.Child("sessions").Child(currentSessionId)
            .Child("active").SetValueAsync(true);
    }

    public void PauseSession()
    {
        if (!isHost) return;

        dbRoot.Child("sessions").Child(currentSessionId)
            .Child("paused").SetValueAsync(true);
    }

    public void ResumeSession()
    {
        if (!isHost) return;

        dbRoot.Child("sessions").Child(currentSessionId)
            .Child("paused").SetValueAsync(false);
    }

    public void EndSession()
    {
        if (!isHost) return;

        StopListening(); // detach listener locally but clients still have theirs

        double finalTime = elapsedSeconds;

        var updates = new Dictionary<string, object>()
    {
        { "active", false },
        { "ended", true },
        { "finalTime", finalTime },
        { "rewards/exp", 50 },
        { "rewards/coins", 10 },

    };

        var sessionRef = dbRoot.Child("sessions").Child(currentSessionId);

        // Step 1: Write end summary
        sessionRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(t =>
        {
            if (t.IsFaulted)
            {
                Debug.LogError("Failed to send session end data.");
                return;
            }

            Debug.Log("Session summary sent.");

            // Step 2: Wait for clients to process the popup
            StartCoroutine(DeleteSessionAfterDelay());
        });
    }

    private IEnumerator DeleteSessionAfterDelay()
    {
        yield return new WaitForSeconds(3f); // give clients time

        dbRoot.Child("sessions").Child(currentSessionId).RemoveValueAsync()
            .ContinueWithOnMainThread(t =>
            {
                Debug.Log("Session deleted.");

                
            });
    }



}
