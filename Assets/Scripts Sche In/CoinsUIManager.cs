using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class CoinsUIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TMP_Text coinsText;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updateUI();
    }

    public void updateUI()
    {
        coinsText.text = UserManager.Instance.coins + "";
    } 
}
