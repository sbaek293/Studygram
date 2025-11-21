using UnityEngine;

public class SessionPanelController : MonoBehaviour
{
    public GameObject lobbyPanel;
    public GameObject sessionPanel;

    private void Start()
    {
        ShowLobby();
    }

    public void ShowLobby()
    {
        lobbyPanel.SetActive(true);
        sessionPanel.SetActive(false);
    }

    public void ShowSession()
    {
        lobbyPanel.SetActive(false);
        sessionPanel.SetActive(true);
    }
}
