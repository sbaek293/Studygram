using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ParticipantItem : MonoBehaviour
{
    public TMP_Text nameText;

    public void SetName(string name)
    {
        nameText.text = name;
    }
}
