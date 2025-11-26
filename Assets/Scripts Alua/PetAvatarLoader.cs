using UnityEngine;
using Firebase.Database;
using Firebase.Extensions; // Required for ContinueWithOnMainThread

public class PetAvatarLoader : MonoBehaviour
{
    private SpriteRenderer myRenderer;
    public static string avatarName;
    void Start()
    {
        // 1. Get the SpriteRenderer component attached to this specific object (PetSprite)
        myRenderer = GetComponent<SpriteRenderer>();
       
        // 2. Start loading the avatar
        LoadAvatar();
    }
    

    void LoadAvatar()
    {
        // REPLACE THIS with your actual User ID logic (e.g., FirebaseAuth.DefaultInstance.CurrentUser.UserId)
        // For testing, you can hardcode a user ID that you know exists in your database
        string currentUserId = AppContext.UserId; 

        DatabaseReference dbRef = FirebaseDatabase.DefaultInstance.RootReference;

        // 3. Ask Firebase: "What avatar ID does this user have?"
        dbRef.Child("users").Child(currentUserId).Child("avatarId").GetValueAsync()
            .ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to load pet avatar.");
                return;
            }

            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists && snapshot.Value != null)
                {
                    avatarName = snapshot.Value.ToString(); // e.g., "owl" or "leopard"
                    
                    // 4. Ask your AvatarManager for the actual picture
                    // (Make sure AvatarManager is in the scene!)
                    if (AvatarManager.Instance != null)
                    {
                        Sprite loadedSprite = AvatarManager.Instance.GetSpriteById(avatarName);

                        // 5. Put the image into the 'None (Sprite)' slot
                        if (loadedSprite != null)
                        {
                            myRenderer.sprite = loadedSprite;
                        }
                        else
                        {
                            Debug.LogWarning($"Avatar ID '{avatarName}' found in database, but not found in AvatarManager list.");
                        }
                    }
                    else
                    {
                        Debug.LogError("AvatarManager Instance is null! Make sure AvatarManager is in the scene.");
                    }
                }
            }
        });
    }
}