using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Database;
using Firebase.Extensions;

public class SessionLobbyController : MonoBehaviour
{
    public SessionManager sessionManager;

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
                item.transform.Find("Name").GetComponent<TMP_Text>().text = snap.Key;

                Button joinBtn = item.transform.Find("JoinButton").GetComponent<Button>();
                string id = snap.Key;
                joinBtn.onClick.AddListener(() =>
                {
                    sessionManager.JoinSession(id);
                });
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
