using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Database;
using Firebase.Extensions;

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

            foreach (var snap in t.Result.Children)
            {
                GameObject item = Instantiate(sessionItemPrefab, sessionListParent);
                SessionItemUI itemUI = item.GetComponent<SessionItemUI>();
                itemUI.Init(snap.Key, snap.Key); // or some display name
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
