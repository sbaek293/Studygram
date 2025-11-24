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
        existingSetsDropdown.onValueChanged.AddListener(OnDropdownChanged);

        //Set initial visibility based on current selection
        OnDropdownChanged(existingSetsDropdown.value);

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

        // Only need to check duplicates when user is creating a NEW set
        if (selected == "New Set")
        {
            // Does a set with this name already exist?
            if (DataManager.GetSet(finalName) != null)
            {
                // You can replace this with a proper error popup if you want
                Debug.LogWarning("A set with this name already exists: " + finalName);
                return;
            }
        }

        DataManager.AddCardToSet(card, finalName);
        UIManager.Instance.ShowMainMenu();
    }

    void OnDropdownChanged(int index)
    {
        string selected = existingSetsDropdown.options[index].text;
        bool isNewSet = selected == "New Set";

        newSetNameField.gameObject.SetActive(isNewSet);

        // optional: clear the field when not creating a new set
        if (!isNewSet)
            newSetNameField.text = string.Empty;
    }
}
