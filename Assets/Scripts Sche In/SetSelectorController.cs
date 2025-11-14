using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;    

public class SetSelectorController : MonoBehaviour
{
    public TMP_Dropdown existingSetsDropdown;
    public TMP_InputField newSetNameField;
    public Button saveButton;

    void Start()
    {
        PopulateDropdown();
        saveButton.onClick.AddListener(OnSave);
    }

    void PopulateDropdown()
    {
        existingSetsDropdown.ClearOptions();
        List<string> names = new List<string>();
        foreach (var set in DataManager.allSets)
            names.Add(set.setName);
        names.Add("New Set");
        existingSetsDropdown.AddOptions(names);
    }

    void OnSave()
    {
        string json = PlayerPrefs.GetString("TempCard");
        Card card = JsonUtility.FromJson<Card>(json);

        string selected = existingSetsDropdown.options[existingSetsDropdown.value].text;
        string finalName = selected == "New Set" ? newSetNameField.text : selected;

        if (string.IsNullOrEmpty(finalName)) return;

        DataManager.AddCardToSet(card, finalName);
        UIManager.Instance.ShowMainMenu();
    }
}
