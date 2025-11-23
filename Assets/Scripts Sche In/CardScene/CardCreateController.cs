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

    [Header("Error Popup")]
    public GameObject errorPanel;
    public TMP_Text errorText;
    public Button errorCloseButton;

    private string colorHex = "#000000";

    void Start()
    {
        multipleChoicePanel.SetActive(false);
        typeDropdown.onValueChanged.AddListener(OnTypeChange);
        addChoiceButton.onClick.AddListener(AddChoice);
        colorButton.onClick.AddListener(ChangeColor);
        saveButton.onClick.AddListener(SaveCard);

        //error panel attempt
        if (errorPanel != null)
            errorPanel.SetActive(false);

        if (errorCloseButton != null)
            errorCloseButton.onClick.AddListener(() =>
            {
                if (errorPanel != null)
                    errorPanel.SetActive(false);
            });

        var questionPlaceholder = questionField.placeholder as TMP_Text;
        if (questionPlaceholder != null)
            questionPlaceholder.text = "Enter question";

        var answerPlaceholder = answerField.placeholder as TMP_Text;
        if (answerPlaceholder != null)
            answerPlaceholder.text = "Enter an answer";
    }

    void OnTypeChange(int index)
    {
        multipleChoicePanel.SetActive(typeDropdown.options[index].text == "Multiple Choice");
    }

    void AddChoice()
    {
        Instantiate(choicePrefab, choicesParent);

        // // Set placeholder text on the new choice's input field
        // TMP_InputField input = newChoice.GetComponentInChildren<TMP_InputField>();
        // if (input != null)
        // {
        //     var placeholder = input.placeholder as TMP_Text;
        //     if (placeholder != null)
        //     {
        //         int optionNumber = choicesParent.childCount;
        //         placeholder.text = $"Enter option {optionNumber}";
        //     }
        // }
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
        // Card newCard = new Card
        // {
        //     cardID = System.Guid.NewGuid().ToString(),
        //     type = typeDropdown.options[typeDropdown.value].text.ToLower(),
        //     question = questionField.text,
        //     colorHex = colorHex
        // };

        // if (newCard.type == "definition")
        // {
        //     newCard.answer = answerField.text;
        // }
        // else
        // {
        //     newCard.choices = new List<string>();
        //     int index = 0;
        //     foreach (Transform child in choicesParent)
        //     {
        //         var input = child.GetComponentInChildren<TMP_InputField>().text;
        //         var toggle = child.GetComponentInChildren<Toggle>().isOn;
        //         newCard.choices.Add(input);
        //         if (toggle) newCard.correctChoiceIndex = index;
        //         index++;
        //     }
        // }

        // PlayerPrefs.SetString("TempCard", JsonUtility.ToJson(newCard));
        // UIManager.Instance.ShowSetSelector();


        //above was working
        string selectedType = typeDropdown.options[typeDropdown.value].text.ToLower();

        Card newCard = new Card
        {
            cardID = System.Guid.NewGuid().ToString(),
            type = selectedType,
            question = questionField.text,
            colorHex = colorHex
        };

        if (newCard.type == "definition")
        {
            newCard.answer = answerField.text;
        }
        else // multiple choice
        {
            List<string> choices = new List<string>();
            int correctIndex = -1;

            foreach (Transform child in choicesParent)
            {
                TMP_InputField inputField = child.GetComponentInChildren<TMP_InputField>();
                Toggle toggle = child.GetComponentInChildren<Toggle>();

                // Only treat objects that have BOTH an input and a toggle as choices
                if (inputField == null || toggle == null)
                    continue;

                string text = inputField.text.Trim();

                if (string.IsNullOrEmpty(text))
                    continue;

                choices.Add(text);

                if (toggle.isOn)
                {
                    correctIndex = choices.Count - 1;
                }
            }


            // Require at least 2 non-empty choices
            if (choices.Count < 2)
            {
                ShowError("Please enter at least two answer choices for multiple choice cards.");
                return;
            }

            // Require one marked correct answer
            if (correctIndex == -1)
            {
                ShowError("Please mark one of the choices as the correct answer.");
                return;
            }

            newCard.choices = choices;
            newCard.correctChoiceIndex = correctIndex;
        }

        PlayerPrefs.SetString("TempCard", JsonUtility.ToJson(newCard));
        UIManager.Instance.ShowSetSelector();
    }

    //error popup during card creation (when count of MCQ options <=2 for instance)
    void ShowError(string message)
    {
        if (errorText != null)
            errorText.text = message;

        if (errorPanel != null)
            errorPanel.SetActive(true);
        else
            Debug.LogError(message);
    }

}
