using UnityEngine;
using UnityEngine.SceneManagement;

public class GroupSelectionUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject selectionPanel;
    
    [Header("Scene Name")]
    [SerializeField] private string groupGardenSceneName = "Garden";
    
    // Call this to show the panel
    public void ShowGroupSelection()
    {
        if (selectionPanel != null)
        {
            selectionPanel.SetActive(true);
        }
    }
    
    // Call this to hide the panel
    public void HideGroupSelection()
    {
        if (selectionPanel != null)
        {
            selectionPanel.SetActive(false);
        }
    }
    
    // Button functions for each group
    public void JoinSocialComputing()
    {
        PlayerPrefs.SetString("SelectedGroup", "Social Computing");
        SceneManager.LoadScene("Garden"); // Directly load Garden scene
    }
    
    public void JoinGroup2()
    {
        PlayerPrefs.SetString("SelectedGroup", "Group 2");
        LoadGroupGarden();
    }
    
    public void JoinGroup3()
    {
        PlayerPrefs.SetString("SelectedGroup", "Group 3");
        LoadGroupGarden();
    }
    
    public void JoinGroup4()
    {
        PlayerPrefs.SetString("SelectedGroup", "Group 4");
        LoadGroupGarden();
    }
    
    private void LoadGroupGarden()
    {
        SceneManager.LoadScene(groupGardenSceneName);
    }
}