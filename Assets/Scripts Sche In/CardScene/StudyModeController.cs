using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
public class StudyModeController : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text questionText;
    public TMP_Text answerText;
    public TMP_Text feedbackText;
    public Button flipButton;
    public Button nextButton;
    public Button prevButton;
    public Transform mcqParent;
    public GameObject mcqOptionPrefab;
    public Button backButton;

    private CardSet currentSet;
    private int currentIndex = 0;

    // Called when the StudyMode panel becomes active
    void OnEnable()
    {
        // Load selected set
        string setName = PlayerPrefs.GetString("CurrentSet", "");
        currentSet = DataManager.GetSet(setName);

        if (currentSet == null || currentSet.cards.Count == 0)
        {
            Debug.LogWarning("No cards found in this set!");
            UIManager.Instance.ShowMainMenu();
            return;
        }

        // Hook up buttons
        flipButton.onClick.RemoveAllListeners();
        nextButton.onClick.RemoveAllListeners();
        prevButton.onClick.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();

        flipButton.onClick.AddListener(FlipCard);
        nextButton.onClick.AddListener(() => ChangeCard(1));
        prevButton.onClick.AddListener(() => ChangeCard(-1));
        backButton.onClick.AddListener(() => UIManager.Instance.ShowMainMenu());

        currentIndex = 0;
        ShowCard();
    }

    // Show the current card
    void ShowCard()
    {
        var card = currentSet.cards[currentIndex];

        // Update question
        questionText.text = card.question;
        answerText.text = "";
        answerText.gameObject.SetActive(false);
        feedbackText.gameObject.SetActive(false);
        questionText.gameObject.SetActive(true);

        // Hide/clear MCQ options
        foreach (Transform child in mcqParent)
            Destroy(child.gameObject);
        mcqParent.gameObject.SetActive(false);
        Debug.Log(card.type);
        // For multiple-choice cards, populate options
        if (card.type == "multiple choice")
        {
            mcqParent.gameObject.SetActive(true);
            for (int i = 0; i < card.choices.Count; i++)
            {
                int index = i; // local copy for closure
                var go = Instantiate(mcqOptionPrefab, mcqParent);
                go.GetComponentInChildren<TMP_Text>().text = card.choices[i];
                go.GetComponent<Button>().onClick.AddListener(() => SelectAnswer(index));
            }
        }

        // Optional: visually set background color
        if (ColorUtility.TryParseHtmlString(card.colorHex, out Color c))
            questionText.color = c; // or apply to background element if desired
    }

    // Flip card (for Definition type)
    void FlipCard()
    {
        var card = currentSet.cards[currentIndex];
        if (card.type != "definition") return;

        answerText.text = card.answer;
        answerText.gameObject.SetActive(!answerText.gameObject.activeSelf);
        questionText.gameObject.SetActive(!questionText.gameObject.activeSelf);
    }

    // Handle multiple-choice selection
    void SelectAnswer(int index)
    {
        var card = currentSet.cards[currentIndex];
        bool correct = (index == card.correctChoiceIndex);
        string correctText = card.choices[card.correctChoiceIndex];

        if (correct)
            feedbackText.text = "Correct!";
        else
            feedbackText.text = $"Incorrect!\nCorrect answer: {correctText}";

        feedbackText.gameObject.SetActive(true);
    }

    // Move between cards
    void ChangeCard(int delta)
    {
        int newIndex = currentIndex + delta;

        // Clamp to range
        if (newIndex < 0) newIndex = 0;
        if (newIndex >= currentSet.cards.Count) newIndex = currentSet.cards.Count - 1;

        if (newIndex != currentIndex)
        {
            currentIndex = newIndex;
            ShowCard();
        }
    }
}
