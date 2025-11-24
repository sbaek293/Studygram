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

            statusText.text = "";
            int totalSessions = (int)t.Result.ChildrenCount;
            int index = 1;

            foreach (var snap in t.Result.Children)
            {
                bool ended = snap.Child("ended").Exists && (bool)snap.Child("ended").Value;
                Debug.LogError(snap.Key);
                if (ended)
                    continue;
                GameObject item = Instantiate(sessionItemPrefab, sessionListParent);
                SessionItemUI itemUI = item.GetComponent<SessionItemUI>();
                
                index++;   
                Debug.LogError(ended);  
                // itemUI.Init(snap.Key, snap.Key); // or some display name
                string displayName = $"Session {index}";
                itemUI.Init(snap.Key, displayName); 
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
