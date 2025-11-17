using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance;

    [Header("Session settings")]
    public string groupId = "groupA"; // set per your app/group
    public string currentUserId; // set on start (or use Firebase Auth UID)

    [Header("Internal state")]
    public string currentSessionId;
    public bool isHost = false;

    // timer state
    private double elapsedSeconds = 0.0;
    private bool paused = false;
    private bool active = false;

    // Firebase reference
    private DatabaseReference dbRoot;

    // events
    public event Action<double> OnTimerUpdated;
    public event Action<bool> OnPausedChanged;
    public event Action<bool> OnActiveChanged;
    public event Action<Dictionary<string, object>> OnSessionDataChanged;

    // throttle updates
    private float hostUpdateAccumulator = 0f;
    private const float hostUpdateInterval = 0.5f; // seconds

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // simple temporary user id if you don't use auth yet
        if (string.IsNullOrEmpty(currentUserId))
            currentUserId = "user_" + UnityEngine.Random.Range(1000, 9999);

        dbRoot = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // ========== Create session (host) ==========
    public void CreateSession()
    {
        currentSessionId = "session_" + Guid.NewGuid().ToString("N").Substring(0, 6);
        isHost = true;
        elapsedSeconds = 0.0;
        paused = false;
        active = true;

        var sessionData = new Dictionary<string, object>()
        {
            { "groupId", groupId },
            { "hostId", currentUserId },
            { "active", true },
            { "paused", false },
            { "elapsedSeconds", 0.0 },
            { "createdAt", ServerValue.Timestamp },
        };

        // set session node with initial values and add participant
        var updates = new Dictionary<string, object>();
        updates[$"sessions/{currentSessionId}"] = sessionData;
        updates[$"sessions/{currentSessionId}/participants/{currentUserId}"] = true;

        dbRoot.UpdateChildrenAsync(updates).ContinueWith(t =>
        {
            if (t.IsFaulted) Debug.LogError("Failed to create session: " + t.Exception);
            else
            {
                Debug.Log("Session created: " + currentSessionId);
                StartListeningToSession();
            }
        });
    }

    // ========== Join existing session ==========
    public void JoinSession(string sessionId)
    {
        if (string.IsNullOrEmpty(sessionId)) return;

        currentSessionId = sessionId;
        isHost = false;

        // add participant
        dbRoot.Child("sessions").Child(currentSessionId).Child("participants")
            .Child(currentUserId).SetValueAsync(true).ContinueWith(t =>
            {
                if (t.IsFaulted) Debug.LogError("Failed to join session: " + t.Exception);
                else
                {
                    Debug.Log("Joined session: " + currentSessionId);
                    StartListeningToSession();
                }
            });
    }

    // ========== Listen for changes ==========
    private void StartListeningToSession()
    {
        var sessionRef = dbRoot.Child("sessions").Child(currentSessionId);

        // listen for the root session object (to pick up many changes)
        sessionRef.ValueChanged += (s, e) =>
        {
            if (e.DatabaseError != null) { Debug.LogError(e.DatabaseError.Message); return; }
            if (e.Snapshot.Exists)
            {
                var dict = SnapshotToDict(e.Snapshot);
                // update local values if present
                if (e.Snapshot.HasChild("elapsedSeconds"))
                {
                    double newTime = ConvertToDouble(e.Snapshot.Child("elapsedSeconds").Value);
                    elapsedSeconds = newTime;
                    OnTimerUpdated?.Invoke(elapsedSeconds);
                }
                if (e.Snapshot.HasChild("paused"))
                {
                    paused = Convert.ToBoolean(e.Snapshot.Child("paused").Value);
                    OnPausedChanged?.Invoke(paused);
                }
                if (e.Snapshot.HasChild("active"))
                {
                    active = Convert.ToBoolean(e.Snapshot.Child("active").Value);
                    OnActiveChanged?.Invoke(active);
                }

                OnSessionDataChanged?.Invoke(dict);
            }
            else
            {
                Debug.Log("Session snapshot does not exist.");
            }
        };
    }

    // ========== Host timer update (send periodically) ==========
    void Update()
    {
        if (isHost && active && !paused && !string.IsNullOrEmpty(currentSessionId))
        {
            elapsedSeconds += Time.deltaTime;
            hostUpdateAccumulator += Time.deltaTime;

            // throttle writes to Firebase to e.g. 0.5s
            if (hostUpdateAccumulator >= hostUpdateInterval)
            {
                hostUpdateAccumulator = 0f;
                dbRoot.Child("sessions").Child(currentSessionId).Child("elapsedSeconds")
                    .SetValueAsync(elapsedSeconds);
            }

            OnTimerUpdated?.Invoke(elapsedSeconds);
        }
    }

    // ========== Host controls ==========
    public void PauseSession()
    {
        if (!isHost || string.IsNullOrEmpty(currentSessionId)) return;
        dbRoot.Child("sessions").Child(currentSessionId).Child("paused").SetValueAsync(true);
    }

    public void ResumeSession()
    {
        if (!isHost || string.IsNullOrEmpty(currentSessionId)) return;
        dbRoot.Child("sessions").Child(currentSessionId).Child("paused").SetValueAsync(false);
    }

    public void EndSession()
    {
        if (!isHost || string.IsNullOrEmpty(currentSessionId)) return;
        dbRoot.Child("sessions").Child(currentSessionId).Child("active").SetValueAsync(false);
    }

    // ========== Utility ==========
    private static Dictionary<string, object> SnapshotToDict(DataSnapshot snap)
    {
        var d = new Dictionary<string, object>();
        foreach (var child in snap.Children)
        {
            d[child.Key] = child.Value;
        }
        return d;
    }

    private static double ConvertToDouble(object o)
    {
        if (o == null) return 0.0;
        try { return Convert.ToDouble(o); } catch { return 0.0; }
    }
}
