using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnswerButton : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI answerText;
    public Image answerIcon;
    public Image backgroundImage;
    
    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color selectedColor = new Color(0.4f, 0.8f, 1f); // Light blue
    public Color hoverColor = new Color(0.9f, 0.9f, 0.9f);
    
    private QuizAnswer answer;
    private QuizManager quizManager;
    private Button button;
    private bool isSelected = false;
    
    void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
    }
    
    public void Setup(QuizAnswer answerData, QuizManager manager)
    {
        answer = answerData;
        quizManager = manager;
        
        // Set answer text
        if (answerText != null)
        {
            answerText.text = answerData.answerText;
        }
        
        // Set icon if available
        if (answerIcon != null)
        {
            if (answerData.answerIcon != null)
            {
                answerIcon.sprite = answerData.answerIcon;
                answerIcon.gameObject.SetActive(true);
            }
            else
            {
                answerIcon.gameObject.SetActive(false);
            }
        }
        
        // Set initial color
        if (backgroundImage != null)
        {
            backgroundImage.color = normalColor;
        }
    }
    
    void OnButtonClicked()
    {
        // Deselect all other buttons
        AnswerButton[] allButtons = FindObjectsOfType<AnswerButton>();
        foreach (AnswerButton btn in allButtons)
        {
            btn.Deselect();
        }
        
        // Select this button
        Select();
        
        // Notify quiz manager
        if (quizManager != null && answer != null)
        {
            quizManager.OnAnswerSelected(answer);
        }
    }
    
    public void Select()
    {
        isSelected = true;
        if (backgroundImage != null)
        {
            backgroundImage.color = selectedColor;
        }
        
        // Optional: Add animation or scale effect
        LeanTween.scale(gameObject, Vector3.one * 1.05f, 0.2f).setEaseOutBack();
    }
    
    public void Deselect()
    {
        isSelected = false;
        if (backgroundImage != null)
        {
            backgroundImage.color = normalColor;
        }
        
        LeanTween.scale(gameObject, Vector3.one, 0.2f).setEaseInBack();
    }
    
    // Optional: Add hover effects
    public void OnPointerEnter()
    {
        if (!isSelected && backgroundImage != null)
        {
            backgroundImage.color = hoverColor;
        }
    }
    
    public void OnPointerExit()
    {
        if (!isSelected && backgroundImage != null)
        {
            backgroundImage.color = normalColor;
        }
    }
}
