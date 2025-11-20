using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;

public class FirebaseInit : MonoBehaviour
{
    public static DatabaseReference DB;

    public static bool IsReady = false;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var status = task.Result;
            if (status == DependencyStatus.Available)
            {
                Debug.Log("Firebase READY");

                DB = FirebaseDatabase.DefaultInstance.RootReference;
                IsReady = true;
            }
            else
            {
                Debug.LogError("Firebase FAILURE: " + status);
            }
        });
    }

}
