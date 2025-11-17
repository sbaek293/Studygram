using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using UnityEngine.UI;
using TMPro;

public class SessionLobbyController : MonoBehaviour
{
    public Transform listParent;
    public GameObject sessionItemPrefab; // button with text
    private DatabaseReference dbRef;

    void Start()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        ListenForActiveSessions();
    }

    void ListenForActiveSessions()
    {
        var sessionsRef = dbRef.Child("sessions");
        sessionsRef.OrderByChild("groupId").EqualTo(SessionManager.Instance.groupId)
            .ValueChanged += (s, e) =>
            {
                if (e.DatabaseError != null) { Debug.LogError(e.DatabaseError.Message); return; }
                // clear current list
                foreach (Transform child in listParent) Destroy(child.gameObject);
                if (!e.Snapshot.Exists) return;
                foreach (var child in e.Snapshot.Children)
                {
                    if (child.HasChild("active") && child.Child("active").Value is bool active && active)
                    {
                        string id = child.Key;
                        var go = Instantiate(sessionItemPrefab, listParent);
                        go.GetComponentInChildren<TMP_Text>().text = id;
                        go.GetComponent<Button>().onClick.AddListener(() => {
                            SessionManager.Instance.JoinSession(id);
                        });
                    }
                }
            };
    }
}
