using TMPro;
using UnityEngine;

public class EndPopUpUI : MonoBehaviour
{
    public static EndPopUpUI Instance;

    public GameObject popup;
    public TMP_Text summaryText;

    private void Awake() => Instance = this;

    public void Show(double time, int exp, int coins)
    {
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);

        summaryText.text = $"Session ended!\nTime: {minutes:D2}:{seconds:D2}\nEXP: {exp}\nCoins: {coins}";
        popup.SetActive(true);
    }

    public void OnOK()
    {
        popup.SetActive(false);
        SessionManager.Instance.panelController.ShowLobby();
    }
}