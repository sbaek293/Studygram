using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class SessionUI : MonoBehaviour
{
    public TMP_InputField sessionIdInput;
    public Button createButton;
    public Button joinButton;
    public TMP_Text sessionIdText;
    public TMP_Text timerText;
    public Button pauseButton;
    public Button resumeButton;
    public Button endButton;
    public TMP_Text participantsText; // optional list display

    void Start()
    {
        createButton.onClick.AddListener(OnCreateClicked);
        joinButton.onClick.AddListener(OnJoinClicked);
        pauseButton.onClick.AddListener(OnPauseClicked);
        resumeButton.onClick.AddListener(OnResumeClicked);
        endButton.onClick.AddListener(OnEndClicked);

        SessionManager.Instance.OnTimerUpdated += UpdateTimer;
        SessionManager.Instance.OnPausedChanged += OnPausedChanged;
        SessionManager.Instance.OnActiveChanged += OnActiveChanged;
        SessionManager.Instance.OnSessionDataChanged += OnSessionDataChanged;
    }

    void OnCreateClicked()
    {
        SessionManager.Instance.CreateSession();
        sessionIdText.text = $"Session ID: {SessionManager.Instance.currentSessionId}";
    }

    void OnJoinClicked()
    {
        var id = sessionIdInput.text.Trim();
        if (string.IsNullOrEmpty(id)) return;
        SessionManager.Instance.JoinSession(id);
        sessionIdText.text = $"Session ID: {SessionManager.Instance.currentSessionId}";
    }

    void OnPauseClicked() => SessionManager.Instance.PauseSession();
    void OnResumeClicked() => SessionManager.Instance.ResumeSession();
    void OnEndClicked() => SessionManager.Instance.EndSession();

    void UpdateTimer(double seconds)
    {
        timerText.text = FormatTime(seconds);
    }

    void OnPausedChanged(bool paused)
    {
        // reflect paused state on UI
        pauseButton.gameObject.SetActive(!paused);
        resumeButton.gameObject.SetActive(paused);
    }

    void OnActiveChanged(bool active)
    {
        if (!active)
        {
            // session ended - update UI
            timerText.text += " (ENDED)";
        }
    }

    void OnSessionDataChanged(System.Collections.Generic.Dictionary<string, object> dict)
    {
        // Optionally show participants
        if (dict != null && dict.ContainsKey("participants"))
        {
            // it's easier to fetch participants separately in a real app; here's a simple render
            participantsText.text = "Participants updated";
        }
    }

    string FormatTime(double t)
    {
        int minutes = Mathf.FloorToInt((float)t / 60f);
        int seconds = Mathf.FloorToInt((float)t % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
}
