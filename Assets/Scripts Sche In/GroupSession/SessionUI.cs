using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Extensions;


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
    // public Button backButton;

    public GameObject endPopup;
    public TMP_Text endTimeText;
    public TMP_Text rewardText;
    private void Start()
    {
        if (manager == null) manager = SessionManager.Instance;

        manager.OnTimerUpdated += UpdateTimer;
        manager.OnActiveChanged += UpdateActiveState;
        manager.OnPausedChanged += UpdatePausedState;
        manager.OnParticipantsChanged += UpdateParticipants;

        InitUI();

        // if (backButton != null)
        // {
        //     backButton.onClick.RemoveAllListeners();
        //     backButton.onClick.AddListener(() =>
        //     {
        //         manager.LeaveSession();   
        //     });
        // }
    }

    private void InitUI()
    {
        timerText.text = "00:00";
        statusText.text = "Waiting...";

        startButton.gameObject.SetActive(manager.isHost);
        pauseButton.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        endButton.gameObject.SetActive(false);

        if (backButton != null)
            backButton.gameObject.SetActive(true);   // waiting room
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

        // Back button visible only before session starts
        // if (backButton != null)
        //     backButton.gameObject.SetActive(!active);

        // statusText.text = active ? "Running" : "Waiting";
        if (!active)
            statusText.text = "Waiting";
        else if (manager.paused)
            statusText.text = "Paused";
        else
            statusText.text = "Running";
    }

    private void UpdatePausedState(bool paused)
    {
        if (!manager.isHost) return;

        pauseButton.gameObject.SetActive(!paused);
        resumeButton.gameObject.SetActive(paused);

        if (!manager.active)
            statusText.text = "Waiting";
        else if (paused)
            statusText.text = "Paused";
        else
            statusText.text = "Running";
    }

    private void UpdateParticipants(Dictionary<string, bool> p)
    {
        foreach (Transform t in participantParent)
            Destroy(t.gameObject);

        DatabaseReference db = FirebaseDatabase.DefaultInstance.RootReference;

        foreach (var kv in p)
        {
            // var obj = Instantiate(participantItemPrefab, participantParent);
            // obj.GetComponentInChildren<TMP_Text>().text = kv.Key;

            string userId = kv.Key;

            var obj = Instantiate(participantItemPrefab, participantParent);
            TMP_Text label = obj.GetComponentInChildren<TMP_Text>();

            // default: show userId while we load (or if username missing)
            label.text = userId;

            // async load username from /users/{userId}/username
            db.Child("users").Child(userId).Child("username")
            .GetValueAsync().ContinueWithOnMainThread(t =>
            {
                if (t.IsFaulted || !t.Result.Exists)
                {
                    // keep userId if we can't get a username
                    return;
                }

                string username = t.Result.Value.ToString();
                label.text = username;
            });
        }
    }

    // -------- BUTTONS --------
    public void OnStart() => manager.StartSession();
    public void OnPause() => manager.PauseSession();
    public void OnResume() => manager.ResumeSession();
    public void OnEnd() => manager.EndSession();
}
