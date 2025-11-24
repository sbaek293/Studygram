using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using Firebase.Database;

public class GroupMatchResultsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject resultsPanel;
    [SerializeField] private Transform groupListContainer;
    [SerializeField] private GameObject groupItemPrefab;
    
    [System.Serializable]
    public class GroupMatchData
    {
        public string groupId;
        public string groupName;
        public int memberCount;
        public int maxMembers;
        public float compatibilityScore;
        public List<string> memberNames;
        public List<string> matchReasons;
    }
    
    public void ShowGroupMatches(List<GroupMatchData> groups)
    {
        Debug.Log($"=== ShowGroupMatches called with {groups.Count} groups ===");
        
        // Clear previous items
        foreach (Transform child in groupListContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create cards
        foreach (var group in groups)
        {
            GameObject item = Instantiate(groupItemPrefab, groupListContainer);
            Debug.Log($"Spawned Card for: {group.groupName}");
            
            // --- FIX START: Use Recursive Search to find components ---
            // This finds the object even if it is nested inside other Panels/Layouts
            
            var nameText = FindComponentDeep<TextMeshProUGUI>(item.transform, "GroupNameText");
            var sizeText = FindComponentDeep<TextMeshProUGUI>(item.transform, "SizeText");
            var scoreText = FindComponentDeep<TextMeshProUGUI>(item.transform, "ScoreText");
            var badgeText = FindComponentDeep<TextMeshProUGUI>(item.transform, "BadgeText");
            var membersText = FindComponentDeep<TextMeshProUGUI>(item.transform, "MembersText");
            var reasonsText = FindComponentDeep<TextMeshProUGUI>(item.transform, "ReasonsText");
            var joinButton = FindComponentDeep<Button>(item.transform, "JoinButton");
            
            // Debugging to help you identify naming mismatches
            if (nameText == null) Debug.LogError("❌ Could not find 'GroupNameText' in prefab!");
            if (scoreText == null) Debug.LogError("❌ Could not find 'ScoreText' in prefab!");
            if (joinButton == null) Debug.LogError("❌ Could not find 'JoinButton' in prefab!");

            // Fill data
            if (nameText != null) nameText.text = group.groupName;
            if (sizeText != null) sizeText.text = $"{group.memberCount}/{group.maxMembers} members";
            if (scoreText != null) scoreText.text = $"{group.compatibilityScore:F0}% Match";
            
            // Handle static helper check safely
            string badgeDesc = "Good Match";
            // Check if StudentMatcher exists, otherwise fallback
            // (Assumes StudentMatcher is in the project)
            try { badgeDesc = StudentMatcher.GetCompatibilityDescription(group.compatibilityScore); } catch { }
            if (badgeText != null) badgeText.text = badgeDesc;

            if (membersText != null && group.memberNames != null) 
                membersText.text = "Members: " + string.Join(", ", group.memberNames);
            
            if (reasonsText != null && group.matchReasons != null) 
                reasonsText.text = string.Join("\n", group.matchReasons);
            
            // Connect join button
            if (joinButton != null)
            {
                // Remove existing listeners to be safe
                joinButton.onClick.RemoveAllListeners();
                string groupId = group.groupId;
                joinButton.onClick.AddListener(() => JoinGroup(groupId));
            }
            // --- FIX END ---
        }
        
        // FORCE LAYOUT REBUILD to fix spacing issues
        LayoutRebuilder.ForceRebuildLayoutImmediate(groupListContainer.GetComponent<RectTransform>());
        
        if (resultsPanel != null)
        {
            resultsPanel.SetActive(true);
        }
    }
    
    // Helper function to find components in nested children
    private T FindComponentDeep<T>(Transform parent, string name) where T : Component
    {
        // 1. Check direct children first
        Transform result = parent.Find(name);
        if (result != null) return result.GetComponent<T>();

        // 2. Check all children recursively
        foreach (Transform child in parent)
        {
            T found = FindComponentDeep<T>(child, name);
            if (found != null) return found;
        }

        return null;
    }

    async void JoinGroup(string groupId)
    {
        Debug.Log($"Joining group: {groupId}");
        
        string userId = PlayerPrefs.GetString("LocalUserId", "");
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("No LocalUserId found! Cannot join group.");
            return;
        }

        try
        {
            var db = FirebaseDatabase.DefaultInstance.RootReference;
            
            // Add user to group list
            await db.Child("groups").Child(groupId).Child("users").Child(userId).SetValueAsync(true);
            
            // Update group size logic
            var sizeSnapshot = await db.Child("groups").Child(groupId).Child("size").GetValueAsync();
            int currentSize = sizeSnapshot.Exists ? Convert.ToInt32(sizeSnapshot.Value) : 0;
            await db.Child("groups").Child(groupId).Child("size").SetValueAsync(currentSize + 1);
            
            // Save as active group for the user
            await db.Child("users").Child(userId).Child("activeGroup").SetValueAsync(groupId);
            await db.Child("users").Child(userId).Child("isGrouped").SetValueAsync(true); 
            
            Debug.Log("Successfully joined group!");
            
            // Save to PlayerPrefs
            PlayerPrefs.SetString("SelectedGroup", groupId);
            PlayerPrefs.Save();
            
            // Load garden scene
            SceneManager.LoadScene("Garden"); 
        }
        catch (Exception e)
        {
            Debug.LogError($"Error joining group: {e.Message}");
        }
    }
}