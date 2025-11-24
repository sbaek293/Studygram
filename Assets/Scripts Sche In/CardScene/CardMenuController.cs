using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardMenuController : MonoBehaviour
{
    public Transform gridParent;
    public GameObject setButtonPrefab;
    public Button newSetButton;
    public Button refreshButton;    

    void OnEnable()
    {
        PopulateSets();
        // RefreshSets();
        newSetButton.onClick.RemoveAllListeners();
        newSetButton.onClick.AddListener(() => UIManager.Instance.ShowCardCreator());

        refreshButton.onClick.RemoveAllListeners();
        refreshButton.onClick.AddListener(RefreshSets);  // NEW
    }

    public void PopulateSets()
    {
        foreach (Transform child in gridParent) Destroy(child.gameObject);
        foreach (var set in DataManager.allSets)
        {
            var go = Instantiate(setButtonPrefab, gridParent);
            go.GetComponentInChildren<TMP_Text>().text = $"{set.setName} ({set.cards.Count})";
            // Color the button based on the first card in the set
            var img = go.GetComponent<Image>();  

            bool isOwned = true;   // default: assume owned
            string setId = set.setId;
            // If the set has a valid online id, check purchasedSetIds
            if (!string.IsNullOrEmpty(setId) && OnlineCardManager.Instance != null)
            {
                isOwned = OnlineCardManager.Instance.purchasedSetIds.Contains(setId);
            }
            if (!isOwned)
            {
                // Unbought = grey
                if (img != null)
                    img.color = Color.gray;
            }
            else
            {
                // Owned = color of first card
                if (img != null && set.cards != null && set.cards.Count > 0)
                {
                    string hex = set.cards[0].colorHex;
                    if (!string.IsNullOrEmpty(hex) && ColorUtility.TryParseHtmlString(hex, out Color c))
                    {
                        img.color = c;
                    }
                }
            }
            go.GetComponent<Button>().onClick.AddListener(() => OpenSet(set.setName));
        }

    }

    void OpenSet(string setName)
    {
        // Find set in DataManager
        CardSet set = DataManager.GetSet(setName);
        if (set == null)
        {
            Debug.LogError("Set not found: " + setName);
            return;
        }

        string setId = set.setId;

        // If user does NOT own the set → show buy UI
        if (!OnlineCardManager.Instance.purchasedSetIds.Contains(setId))
        {
            UIManager.Instance.ShowBuyPopup(setName, setId);
            return;
        }

        // User owns the set → open normally
        PlayerPrefs.SetString("CurrentSet", setName);
        UIManager.Instance.ShowStudyMode();
    }

    void RefreshSets()
    {
        Debug.Log("Refreshing sets...");

        // Fetch from Firebase
        OnlineCardManager.Instance.DownloadAllUserSets(() =>
        {
            Debug.Log("Refresh complete.");
            PopulateSets();
        });
    }

    public void ClearAllData()
    {
        DataManager.allSets.Clear();
        DataManager.SaveData();
        Debug.Log("All flashcards cleared.");
    }
}
