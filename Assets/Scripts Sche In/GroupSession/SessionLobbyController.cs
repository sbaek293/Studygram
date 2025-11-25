using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Database;
using Firebase.Extensions;
using System;


public class SessionLobbyController : MonoBehaviour
{
    public SessionManager sessionManager;
    public SessionPanelController panelController;

    [Header("UI")]
    public Transform sessionListParent;
    public GameObject sessionItemPrefab;
    public TMP_Text statusText;

    private DatabaseReference db;

    private void Start()
    {
        db = FirebaseDatabase.DefaultInstance.RootReference;
        RefreshSessions();
    }

    public void RefreshSessions()
    {
        statusText.text = "Loading sessions...";

        db.Child("sessions").GetValueAsync().ContinueWithOnMainThread(t =>
        {
            foreach (Transform child in sessionListParent)
                Destroy(child.gameObject);

            if (t.IsFaulted)
            {
                statusText.text = "Failed to load.";
                return;
            }

            if (!t.Result.Exists)
            {
                statusText.text = "No sessions yet.";
                return;
            }

            int shownCount = 0;   // how many sessions we actually show
            int index = 1;

            foreach (var snap in t.Result.Children)
            {
                // Skip ended sessions
                bool ended = snap.Child("ended").Exists && (bool)snap.Child("ended").Value;
                Debug.LogError($"Session key: {snap.Key}, ended: {ended}");
                if (ended)
                    continue;

                // Instantiate UI item
                GameObject item = Instantiate(sessionItemPrefab, sessionListParent);
                SessionItemUI itemUI = item.GetComponent<SessionItemUI>();

                string displayName = $"Session {index}";
                itemUI.Init(snap.Key, displayName);

                index++;
                shownCount++;
            }

            // If we had a /sessions node but all sessions were filtered out
            if (shownCount == 0)
            {
                statusText.text = "No sessions yet.";
            }
            else
            {
                statusText.text = "";
            }
            
        });
    }

    public void CreateSession()
    {
        if (!FirebaseInit.IsReady)
        {
            Debug.LogError("SESSION CREATE FAILED - Firebase not ready yet!");
            return;
        }

        sessionManager.CreateSession();
    }

}
