using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class CardCreateController : MonoBehaviour
{
    public TMP_Dropdown  typeDropdown;
    public TMP_InputField questionField;
    public TMP_InputField answerField;

    [Header("Multiple Choice Section")]
    public GameObject multipleChoicePanel;
    public Transform choicesParent;
    public GameObject choicePrefab; // Input + Toggle
    public Button addChoiceButton;

    [Header("Other UI")]
    public Button colorButton;
    public Image colorPreview;
    public Button saveButton;

    private string colorHex = "#FFFFFF";

    void Start()
    {
        multipleChoicePanel.SetActive(false);
        typeDropdown.onValueChanged.AddListener(OnTypeChange);
        addChoiceButton.onClick.AddListener(AddChoice);
        colorButton.onClick.AddListener(ChangeColor);
        saveButton.onClick.AddListener(SaveCard);
    }

    void OnTypeChange(int index)
    {
        multipleChoicePanel.SetActive(typeDropdown.options[index].text == "Multiple Choice");
    }

    void AddChoice()
    {
        Instantiate(choicePrefab, choicesParent);
    }

    void ChangeColor()
    {
        colorHex = $"#{ColorUtility.ToHtmlStringRGB(Random.ColorHSV())}";
        Color c;
        ColorUtility.TryParseHtmlString(colorHex, out c);
        colorPreview.color = c;
    }

    void SaveCard()
    {
        Card newCard = new Card
        {
            cardID = System.Guid.NewGuid().ToString(),
            type = typeDropdown.options[typeDropdown.value].text.ToLower(),
            question = questionField.text,
            colorHex = colorHex
        };

        if (newCard.type == "definition")
        {
            newCard.answer = answerField.text;
        }
        else
        {
            newCard.choices = new List<string>();
            int index = 0;
            foreach (Transform child in choicesParent)
            {
                var input = child.GetComponentInChildren<TMP_InputField>().text;
                var toggle = child.GetComponentInChildren<Toggle>().isOn;
                newCard.choices.Add(input);
                if (toggle) newCard.correctChoiceIndex = index;
                index++;
            }
        }

        PlayerPrefs.SetString("TempCard", JsonUtility.ToJson(newCard));
        UIManager.Instance.ShowSetSelector();
    }

}
