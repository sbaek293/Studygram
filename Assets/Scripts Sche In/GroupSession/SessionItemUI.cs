using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionItemUI : MonoBehaviour
{
    public TMP_Text sessionNameText;
    private string sessionId;

    public void Init(string sessionId, string displayName)
    {
        this.sessionId = sessionId;
        sessionNameText.text = displayName;
    }

    public void OnClick()
    {
        if (SessionManager.Instance != null)
        {
            SessionManager.Instance.JoinSession(sessionId);
        }
    }
}
