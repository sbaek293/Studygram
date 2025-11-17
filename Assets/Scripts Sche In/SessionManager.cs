using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance;

    [Header("Session Settings")]
    public string groupId = "groupA"; // set per your app/group
    public string currentUserId;      // set on start (or use Firebase Auth UID)

    [Header("Internal State")]
    public string currentSessionId;
    public bool isHost = false;

    // Timer state
    private double elapsedSeconds = 0.0;
    private bool paused = false;
    private bool active = false;

    // Firebase reference
    private DatabaseReference dbRoot;

    // Events for UI
    public event Action<double> OnTimerUpdated;
    public event Action<bool> OnPausedChanged;
    public event Action<bool> OnActiveChanged;
    public event Action<bool> OnReadyToStartChanged;
    public event Action<Dictionary<string, object>> OnSessionDataChanged;
    public event Action<List<string>> OnParticipantsChanged;

    // Timer throttling
    private float hostUpdateAccumulator = 0f;
    private const float hostUpdateInterval = 0.5f; // seconds

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (string.IsNullOrEmpty(currentUserId))
            currentUserId = "user_" + UnityEngine.Random.Range(1000, 9999);

        dbRoot = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // ================== Create Session (host) ==================
    public void CreateSession()
    {
        currentSessionId = "session_" + Guid.NewGuid().ToString("N").Substring(0, 6);
        isHost = true;
        elapsedSeconds = 0.0;
        paused = false;
        active = false; // timer doesn’t start automatically

        var sessionData = new Dictionary<string, object>()
        {
            { "groupId", groupId },
            { "hostId", currentUserId },
            { "active", false },
            { "paused", false },
            { "readyToStart", false },
            { "elapsedSeconds", 0.0 },
            { "createdAt", ServerValue.Timestamp },
        };

        var updates = new Dictionary<string, object>();
        updates[$"sessions/{currentSessionId}"] = sessionData;
        updates[$"sessions/{currentSessionId}/participants/{currentUserId}"] = true;

        dbRoot.UpdateChildrenAsync(updates).ContinueWithOnMainThread(t =>
        {
            if (t.IsFaulted)
            {
                foreach (var ex in t.Exception.Flatten().InnerExceptions)
                    Debug.LogWarning("Firebase write warning: " + ex.Message);
            }
            else
            {
                Debug.Log("Session created: " + currentSessionId);
            }

            try { StartListeningToSession(); }
            catch (Exception e) { Debug.LogWarning("StartListeningToSession warning: " + e.Message); }
        });
    }

    // ================== Join Existing Session ==================
    public void JoinSession(string sessionId)
    {
        if (string.IsNullOrEmpty(sessionId)) return;

        currentSessionId = sessionId;
        isHost = false;

        dbRoot.Child("sessions").Child(currentSessionId).Child("participants")
            .Child(currentUserId).SetValueAsync(true).ContinueWithOnMainThread(t =>
            {
                if (t.IsFaulted)
                    Debug.LogError("Failed to join session: " + t.Exception);
                else
                    Debug.Log("Joined session: " + currentSessionId);

                try { StartListeningToSession(); }
                catch (Exception e) { Debug.LogWarning("StartListeningToSession warning: " + e.Message); }
            });
    }

    // ================== Host Control Functions ==================
    public void SetReadyToStart()
    {
        if (!isHost || string.IsNullOrEmpty(currentSessionId)) return;

        dbRoot.Child("sessions").Child(currentSessionId).Child("readyToStart")
            .SetValueAsync(true).ContinueWithOnMainThread(t =>
            {
                if (t.IsFaulted) Debug.LogError("Failed to set readyToStart: " + t.Exception);
                else Debug.Log("Session ready to start.");
            });
    }

    public void StartTimer()
    {
        if (!isHost || string.IsNullOrEmpty(currentSessionId)) return;

        dbRoot.Child("sessions").Child(currentSessionId).Child("active")
            .SetValueAsync(true).ContinueWithOnMainThread(t =>
            {
                if (t.IsFaulted) Debug.LogError("Failed to start session: " + t.Exception);
                else Debug.Log("Session timer started.");
            });
    }

    public void PauseSession()
    {
        if (!isHost || string.IsNullOrEmpty(currentSessionId)) return;
        dbRoot.Child("sessions").Child(currentSessionId).Child("paused")
            .SetValueAsync(true);
    }

    public void ResumeSession()
    {
        if (!isHost || string.IsNullOrEmpty(currentSessionId)) return;
        dbRoot.Child("sessions").Child(currentSessionId).Child("paused")
            .SetValueAsync(false);
    }

    public void EndSession()
    {
        if (!isHost || string.IsNullOrEmpty(currentSessionId)) return;
        dbRoot.Child("sessions").Child(currentSessionId).Child("active")
            .SetValueAsync(false);
    }

    // ================== Listen to Session Changes ==================
    private void StartListeningToSession()
    {
        var sessionRef = dbRoot.Child("sessions").Child(currentSessionId);

        sessionRef.ValueChanged += (s, e) =>
        {
            if (e.DatabaseError != null) { Debug.LogError(e.DatabaseError.Message); return; }
            if (!e.Snapshot.Exists) return;

            var dict = SnapshotToDict(e.Snapshot);
            OnSessionDataChanged?.Invoke(dict);

            if (e.Snapshot.HasChild("elapsedSeconds"))
            {
                elapsedSeconds = ConvertToDouble(e.Snapshot.Child("elapsedSeconds").Value);
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

            if (e.Snapshot.HasChild("readyToStart"))
            {
                bool ready = Convert.ToBoolean(e.Snapshot.Child("readyToStart").Value);
                OnReadyToStartChanged?.Invoke(ready);
            }

            if (e.Snapshot.HasChild("participants"))
            {
                var participantsSnap = e.Snapshot.Child("participants");
                List<string> participants = new List<string>();
                foreach (var child in participantsSnap.Children)
                    participants.Add(child.Key);

                OnParticipantsChanged?.Invoke(participants);
            }
        };
    }

    // ================== Timer Updates (Host Only) ==================
    void Update()
    {
        if (isHost && active && !paused && !string.IsNullOrEmpty(currentSessionId))
        {
            elapsedSeconds += Time.deltaTime;
            hostUpdateAccumulator += Time.deltaTime;

            if (hostUpdateAccumulator >= hostUpdateInterval)
            {
                hostUpdateAccumulator = 0f;
                dbRoot.Child("sessions").Child(currentSessionId).Child("elapsedSeconds")
                    .SetValueAsync(elapsedSeconds);
            }

            OnTimerUpdated?.Invoke(elapsedSeconds);
        }
    }

    // ================== Utility ==================
    private static Dictionary<string, object> SnapshotToDict(DataSnapshot snap)
    {
        var d = new Dictionary<string, object>();
        foreach (var child in snap.Children)
            d[child.Key] = child.Value;
        return d;
    }

    private static double ConvertToDouble(object o)
    {
        if (o == null) return 0.0;
        try { return Convert.ToDouble(o); } catch { return 0.0; }
    }
}
