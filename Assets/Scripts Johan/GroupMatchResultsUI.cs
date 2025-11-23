using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.SceneManagement;
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
    Debug.Log($"=== Showing {groups.Count} groups ===");
    
    // Clear previous
    foreach (Transform child in groupListContainer)
    {
        Destroy(child.gameObject);
    }
    
    // Create cards
    foreach (var group in groups)
    {
        GameObject item = Instantiate(groupItemPrefab, groupListContainer);
        Debug.Log($"Spawned: {group.groupName} at Y: {item.transform.localPosition.y}");
        
        // Find components in the prefab
        TextMeshProUGUI nameText = item.transform.Find("GroupNameText")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI sizeText = item.transform.Find("SizeText")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI scoreText = item.transform.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI badgeText = item.transform.Find("BadgeText")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI membersText = item.transform.Find("MembersText")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI reasonsText = item.transform.Find("ReasonsText")?.GetComponent<TextMeshProUGUI>();
        Button joinButton = item.transform.Find("JoinButton")?.GetComponent<Button>();
        
        // Fill data
        if (nameText != null) nameText.text = group.groupName;
        if (sizeText != null) sizeText.text = $"{group.memberCount}/{group.maxMembers} members";
        if (scoreText != null) scoreText.text = $"{group.compatibilityScore:F0}% Match";
        if (badgeText != null) badgeText.text = StudentMatcher.GetCompatibilityDescription(group.compatibilityScore);
        if (membersText != null) membersText.text = "Members: " + string.Join(", ", group.memberNames);
        if (reasonsText != null) reasonsText.text = string.Join("\n", group.matchReasons);
        
        // Connect join button
        if (joinButton != null)
        {
            string groupId = group.groupId;
            joinButton.onClick.AddListener(() => JoinGroup(groupId));
        }
    }
    
    // FORCE LAYOUT REBUILD
    LayoutRebuilder.ForceRebuildLayoutImmediate(groupListContainer.GetComponent<RectTransform>());
    
    if (resultsPanel != null)
    {
        resultsPanel.SetActive(true);
    }
}
    
    async void JoinGroup(string groupId)
{
    Debug.Log($"Joining group: {groupId}");
    
    string userId = "user123"; // TODO: Get real user ID
    
    try
    {
        var db = FirebaseDatabase.DefaultInstance.RootReference;
        
        // Add user to group
        await db.Child("groups").Child(groupId).Child("users").Child(userId).SetValueAsync(true);
        
        // Increment group size
        var sizeSnapshot = await db.Child("groups").Child(groupId).Child("size").GetValueAsync();
        int currentSize = sizeSnapshot.Exists ? Convert.ToInt32(sizeSnapshot.Value) : 0;
        await db.Child("groups").Child(groupId).Child("size").SetValueAsync(currentSize + 1);
        
        // Update user's groupId
        await db.Child("users").Child(userId).Child("groupId").SetValueAsync(groupId);
        
        Debug.Log("Successfully joined group!");
        
        // Save to PlayerPrefs
        PlayerPrefs.SetString("SelectedGroup", groupId);
        PlayerPrefs.Save();
        
        // Load garden scene
        SceneManager.LoadScene("Garden"); // Change to your garden scene name
    }
    catch (Exception e)
    {
        Debug.LogError($"Error joining group: {e.Message}");
    }
}
}