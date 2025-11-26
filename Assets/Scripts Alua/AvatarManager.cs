using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public struct AvatarItem
{
    public string id;       // e.g., "owl", "bunny"
    public Sprite sprite;   // The actual image file
}

public class AvatarManager : MonoBehaviour
{
    public static AvatarManager Instance; // Singleton for easy access

    public List<AvatarItem> avatarList;   // Drag your sprites here in Inspector

    private void Awake()
    {
        // standard singleton setup
        if (Instance == null) 
        {
            Instance = this; 
            DontDestroyOnLoad(gameObject); // Keep this alive between scenes
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    // Helper to get a sprite by its name
    public Sprite GetSpriteById(string id)
    {
        // Find the item in the list where the ID matches
        var item = avatarList.FirstOrDefault(x => x.id == id);
        return item.sprite; // Returns null if not found
    }

    // Helper to get a random avatar ID
    public string GetRandomAvatarId()
    {
        int randomIndex = Random.Range(0, avatarList.Count);
        return avatarList[randomIndex].id;
    }
}