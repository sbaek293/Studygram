using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SessionUI : MonoBehaviour
{
    public SessionManager manager;

    public TMP_Text timerText;
    public TMP_Text statusText;

    public Transform participantParent;
    public GameObject participantItemPrefab;

    // Host buttons
    public Button startButton;
    public Button pauseButton;
    public Button resumeButton;
    public Button endButton;

    private void Start()
    {
        if (manager == null) manager = SessionManager.Instance;

        manager.OnTimerUpdated += UpdateTimer;
        manager.OnActiveChanged += UpdateActiveState;
        manager.OnPausedChanged += UpdatePausedState;
        manager.OnParticipantsChanged += UpdateParticipants;

        InitUI();
    }

    private void InitUI()
    {
        timerText.text = "00:00";
        statusText.text = "Waiting...";

        startButton.gameObject.SetActive(manager.isHost);
        pauseButton.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        endButton.gameObject.SetActive(false);
    }

    // ----------- EVENTS ------------
    private void UpdateTimer(double s)
    {
        TimeSpan t = TimeSpan.FromSeconds(s);
        timerText.text = $"{t.Minutes:D2}:{t.Seconds:D2}";
    }

    private void UpdateActiveState(bool active)
    {
        if (manager.isHost)
        {
            startButton.gameObject.SetActive(!active);
            pauseButton.gameObject.SetActive(active && !manager.paused);
            resumeButton.gameObject.SetActive(active && manager.paused);
            endButton.gameObject.SetActive(active);
        }

        statusText.text = active ? "Running" : "Waiting";
    }

    private void UpdatePausedState(bool paused)
    {
        if (!manager.isHost) return;

        pauseButton.gameObject.SetActive(!paused);
        resumeButton.gameObject.SetActive(paused);
    }

    private void UpdateParticipants(Dictionary<string, bool> p)
    {
        foreach (Transform t in participantParent)
            Destroy(t.gameObject);

        foreach (var kv in p)
        {
            var obj = Instantiate(participantItemPrefab, participantParent);
            obj.GetComponentInChildren<TMP_Text>().text = kv.Key;
        }
    }

    // -------- BUTTONS --------
    public void OnStart() => manager.StartSession();
    public void OnPause() => manager.PauseSession();
    public void OnResume() => manager.ResumeSession();
    public void OnEnd() => manager.EndSession();
}
