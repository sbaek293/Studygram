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
    public Image windowBackground;
    public Button backButton; 

    [Header("Error Popup")]
    public GameObject errorPanel;
    public TMP_Text errorText;
    public Button errorCloseButton;

    private string colorHex = "#acc8e5";
    private int previewColorIndex = 1;

    void Start()
    {
        // multipleChoicePanel.SetActive(false);
        typeDropdown.onValueChanged.AddListener(OnTypeChange);
        addChoiceButton.onClick.AddListener(AddChoice);
        colorButton.onClick.AddListener(ChangeColor);
        saveButton.onClick.AddListener(SaveCard);

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(() =>
        {
            // Go back without saving – same place SaveCard goes after save
            UIManager.Instance.ShowMainMenu();
        });

        typeDropdown.value = 1;
        // typeDropdown.options[index].text == "Definition";

        //error panel attempt
        if (errorPanel != null)
            errorPanel.SetActive(false);

        if (errorCloseButton != null)
            errorCloseButton.onClick.AddListener(() =>
            {
                if (errorPanel != null)
                    errorPanel.SetActive(false);
            });

        var answerPlaceholder = answerField.placeholder as TMP_Text;
        if (answerPlaceholder != null)
            answerPlaceholder.text = "Enter the definition";

        Color c;
        ColorUtility.TryParseHtmlString("#E6ACBA", out c);
        colorPreview.color = c;

        ColorUtility.TryParseHtmlString("#519FBE", out c);
        //windowBackground.color = c;

        addChoiceButton.gameObject.SetActive(false);

        foreach (Transform child in choicesParent)                   
        {                                                            
            Toggle t = child.GetComponentInChildren<Toggle>();      
            if (t != null)                                        
            {                                                       
                t.isOn = false;                                     
                t.onValueChanged.AddListener(isOn =>               
                    OnChoiceToggleChanged(t, isOn));               
            }                                                      
        } 

        OnTypeChange(typeDropdown.value);
    }

    void OnTypeChange(int index)
    {
        // multipleChoicePanel.SetActive(typeDropdown.options[index].text == "Multiple Choice");
        bool isMCQ = typeDropdown.options[index].text == "Multiple Choice";
        Debug.Log("Dropdown value = " + typeDropdown.value);

        var questionPlaceholder = questionField.placeholder as TMP_Text;   
        if (questionPlaceholder != null)                                  
        {                                                            
            questionPlaceholder.text = isMCQ                             
                ? "Enter the question"                                
                : "Enter the term";                                 
        } 

        if (isMCQ)
        {
            // Definition → MCQ: copy answerField into option 1
            TMP_InputField firstOption = GetFirstChoiceInput();
            if (firstOption != null)
            {
                firstOption.text = answerField.text;  // keep values in sync
            }

            // Rename placeholders of existing option fields (including option 1)
            UpdateChoicePlaceholders();
        }
        else
        {
            // MCQ → Definition: copy option 1 into answerField
            TMP_InputField firstOption = GetFirstChoiceInput();
            if (firstOption != null)
            {
                answerField.text = firstOption.text;
            }

            // Make sure definition placeholder stays correct
            var answerPlaceholder = answerField.placeholder as TMP_Text;
            if (answerPlaceholder != null)
                answerPlaceholder.text = "Enter the definition";
        }

        multipleChoicePanel.SetActive(isMCQ);
        addChoiceButton.gameObject.SetActive(isMCQ);
        answerField.gameObject.SetActive(!isMCQ);
    }

    void AddChoice()
    {
        // Instantiate and keep a reference to the new choice object
        GameObject newChoice = Instantiate(choicePrefab, choicesParent);

        Toggle toggle = newChoice.GetComponentInChildren<Toggle>();                  
        if (toggle != null)                                                         
        {                                                                           
            toggle.isOn = false;                                                    
            toggle.onValueChanged.AddListener(isOn => OnChoiceToggleChanged(toggle, isOn)); 
        } 

        // Set placeholder text on the new choice's input field
        TMP_InputField input = newChoice.GetComponentInChildren<TMP_InputField>();
        if (input != null)
        {
            var placeholder = input.placeholder as TMP_Text;
            if (placeholder != null)
            {
                // childCount now includes this newly added choice
                int optionNumber = choicesParent.childCount;
                placeholder.text = $"Enter option {optionNumber}";
            }
        }
    }

    void ChangeColor()
    {
        // 1) Save current preview color into colorHex (so it gets stored in the Card)
        Color currentPreview = colorPreview.color;
        colorHex = $"#{ColorUtility.ToHtmlStringRGB(currentPreview)}";

        // 2) Define a small palette of hex colors
        string[] previewPalette = { "#acc8e5", "#E6ACBA", "#cbace6", "#EB9A57", "#F7E1A0", "#58CCB5" };

        // 3) Advance index and wrap around
        previewColorIndex = (previewColorIndex + 1) % previewPalette.Length;

        // 4) Parse the next hex and apply it to the preview
        Color nextPreview;
        ColorUtility.TryParseHtmlString(previewPalette[previewColorIndex], out nextPreview);
        colorPreview.color = nextPreview;
    }

    void SaveCard()
    {
        string selectedType = typeDropdown.options[typeDropdown.value].text.ToLower();

        Card newCard = new Card
        {
            cardID = System.Guid.NewGuid().ToString(),
            type = selectedType,
            question = questionField.text,
            colorHex = colorHex
        };

        string questionText = questionField.text.Trim();                    
        if (string.IsNullOrEmpty(questionText))                             
        {                 
            if (newCard.type == "definition")               
                ShowError("Please enter the term."); 
            else ShowError("Please enter the question.");                  
            return;                                                         
        } 

        if (newCard.type == "definition")
        {
            // newCard.answer = answerField.text;
            string answerText = answerField.text.Trim();                    
            if (string.IsNullOrEmpty(answerText))                           
            {                                                               
                ShowError("Please enter the definition.");                       
                return;                                                     
            }                                                               

            newCard.answer = answerText;
        }
        else // multiple choice
        {
            List<string> choices = new List<string>();
            int correctIndex = -1;
            bool hasEmptyOption = false; 

            foreach (Transform child in choicesParent)
            {
                TMP_InputField inputField = child.GetComponentInChildren<TMP_InputField>();
                Toggle toggle = child.GetComponentInChildren<Toggle>();

                // Only treat objects that have BOTH an input and a toggle as choices
                if (inputField == null || toggle == null)
                    continue;

                string text = inputField.text.Trim();

                if (string.IsNullOrEmpty(text)) {
                    hasEmptyOption = true;
                    continue;
                }

                choices.Add(text);

                if (toggle.isOn)
                {
                    correctIndex = choices.Count - 1;
                }
            }

            if (hasEmptyOption)
            {
                ShowError("Don't leave the choice fields empty.");
                return;
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

    //this is to make different placeholders for MCQ 
    void UpdateChoicePlaceholders()
    {
        int optionNumber = 1;

        foreach (Transform child in choicesParent)
        {
            TMP_InputField input = child.GetComponentInChildren<TMP_InputField>();
            if (input == null) continue;

            var placeholder = input.placeholder as TMP_Text;
            if (placeholder == null) continue;

            placeholder.text = $"Enter option {optionNumber}";
            optionNumber++;
        }
    }

    //for keeping def ansField and MCQ option 1 the same
    TMP_InputField GetFirstChoiceInput()
    {
        // Try to find an existing option 1 under choicesParent
        foreach (Transform child in choicesParent)
        {
            TMP_InputField input = child.GetComponentInChildren<TMP_InputField>();
            if (input != null) {
                // make sure its toggle is part of the "at most one" logic
                Toggle t = child.GetComponentInChildren<Toggle>();
                if (t != null)
                {
                    // optional: force it off initially
                    // t.isOn = false;
                    t.onValueChanged.AddListener(isOn => OnChoiceToggleChanged(t, isOn));
                }

                return input;
            }
        }

        // If none exists but we have a prefab, create option 1
        if (choicePrefab != null && choicesParent != null)
        {
            GameObject newChoice = Instantiate(choicePrefab, choicesParent);
            TMP_InputField input = newChoice.GetComponentInChildren<TMP_InputField>();
            Toggle t = newChoice.GetComponentInChildren<Toggle>();
            if (t != null)
            {
                t.isOn = true;
                t.onValueChanged.AddListener(isOn => OnChoiceToggleChanged(t, isOn));
            }
            return input;
        }

        return null;
    }

    // Ensures at most one toggle is on at any time
    void OnChoiceToggleChanged(Toggle changedToggle, bool isOn)
    {
        // If this toggle was turned ON, turn all others OFF
        if (isOn)
        {
            foreach (Transform child in choicesParent)
            {
                Toggle t = child.GetComponentInChildren<Toggle>();
                if (t != null && t != changedToggle)
                {
                    t.isOn = false;
                }
            }
        }
        // If it was turned OFF, we do nothing:
        // result can be 0 toggles on, which is fine,
        // since SaveCard() already enforces "one must be checked".
    }

    void OnEnable()
    {
        previewColorIndex = 1;
        colorHex = "#acc8e5";
        Color c;
        if (ColorUtility.TryParseHtmlString("#E6ACBA", out c))
            colorPreview.color = c;

        // Clear text fields every time panel opens
        questionField.text = string.Empty;
        answerField.text = string.Empty;

        // Clear MCQ options
        foreach (Transform child in choicesParent)
            Destroy(child.gameObject);

        // Reset dropdown (optional)
        typeDropdown.value = 1;
        typeDropdown.RefreshShownValue();

        // Make sure UI matches type
        OnTypeChange(typeDropdown.value);
    }
}
