using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardMenuController : MonoBehaviour
{
    public Transform gridParent;
    public GameObject setButtonPrefab;
    public Button newSetButton;

    void OnEnable()
    {
        PopulateSets();
        newSetButton.onClick.RemoveAllListeners();
        newSetButton.onClick.AddListener(() => UIManager.Instance.ShowCardCreator());
    }

    void PopulateSets()
    {
        foreach (Transform child in gridParent) Destroy(child.gameObject);
        foreach (var set in DataManager.allSets)
        {
            var go = Instantiate(setButtonPrefab, gridParent);
            go.GetComponentInChildren<TMP_Text>().text = $"{set.setName} ({set.cards.Count})";
            go.GetComponent<Button>().onClick.AddListener(() => OpenSet(set.setName));
        }
    }

    void OpenSet(string setName)
    {
        PlayerPrefs.SetString("CurrentSet", setName);
        UIManager.Instance.ShowStudyMode();
    }

    public void ClearAllData()
    {
        DataManager.allSets.Clear();
        DataManager.SaveData();
        Debug.Log("All flashcards cleared.");
    }
}
