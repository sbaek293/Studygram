using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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
        // Clear previous
        foreach (Transform child in groupListContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create UI for each group
        foreach (var group in groups)
        {
            GameObject item = Instantiate(groupItemPrefab, groupListContainer);
            
            // Find components
            TextMeshProUGUI nameText = item.transform.Find("GroupNameText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI sizeText = item.transform.Find("SizeText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI scoreText = item.transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI badgeText = item.transform.Find("BadgeText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI membersText = item.transform.Find("MembersText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI reasonsText = item.transform.Find("ReasonsText").GetComponent<TextMeshProUGUI>();
            Button joinButton = item.transform.Find("JoinButton").GetComponent<Button>();
            
            // Fill data
            nameText.text = group.groupName;
            sizeText.text = $"{group.memberCount}/{group.maxMembers} members";
            scoreText.text = $"{group.compatibilityScore:F0}% Match";
            badgeText.text = StudentMatcher.GetCompatibilityDescription(group.compatibilityScore);
            membersText.text = "Members: " + string.Join(", ", group.memberNames);
            reasonsText.text = string.Join("\n", group.matchReasons);
            
            // Connect join button
            string groupId = group.groupId;
            joinButton.onClick.AddListener(() => JoinGroup(groupId));
        }
        
        resultsPanel.SetActive(true);
    }
    
    private void JoinGroup(string groupId)
    {
        Debug.Log($"Joining group: {groupId}");
        // TODO: Firebase - add user to group
        // TODO: Load group garden scene
    }
}