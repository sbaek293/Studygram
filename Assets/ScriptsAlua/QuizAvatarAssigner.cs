using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions; // Needed for ContinueWith

public class QuizAvatarAssigner : MonoBehaviour
{
    public Image avatarDisplayImage; // Drag the UI Image here
    DatabaseReference dbReference;

    void Start()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        
        // 1. Assign a random avatar
        AssignAndSaveAvatar();
    }

    void AssignAndSaveAvatar()
    {
        // Get a random ID from our Manager (e.g., "leopard")
        string randomId = AvatarManager.Instance.GetRandomAvatarId();

        // 2. Display it immediately on the result page
        Sprite selectedSprite = AvatarManager.Instance.GetSpriteById(randomId);
        if (selectedSprite != null)
        {
            avatarDisplayImage.sprite = selectedSprite;
        }

        // 3. Save to Firebase
        // Assuming you have the UserID stored somewhere. 
        // If using Firebase Auth, use FirebaseAuth.DefaultInstance.CurrentUser.UserId
        string userId = AppContext.UserId; 

        dbReference.Child("users").Child(userId).Child("avatarId").SetValueAsync(randomId)
            .ContinueWithOnMainThread(task => 
        {
            if (task.IsCompleted) {
                Debug.Log("Avatar saved as: " + randomId);
            }
        });
    }
}