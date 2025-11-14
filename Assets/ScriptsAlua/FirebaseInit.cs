using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;

public class FirebaseInit : MonoBehaviour
{
    public static DatabaseReference DB;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);  // <--- Keep this object forever
    }

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var status = task.Result;
            if (status == DependencyStatus.Available)
            {
                Debug.Log("Firebase is ready!");
                DB = FirebaseDatabase.DefaultInstance.RootReference;
            }
            else
            {
                Debug.LogError($"Could not resolve Firebase dependencies: {status}");
            }
        });
    }
}
