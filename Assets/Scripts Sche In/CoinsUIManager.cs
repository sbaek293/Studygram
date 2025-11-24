using UnityEngine;
using TMPro;

public class CoinsUIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TMP_Text coinsText;
    void Start()
    {
        UserManager.Instance.OnUserDataLoaded += updateUI;
        Debug.Log("update Coins UI");
        updateUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateUI()
    {
        Debug.Log("you should have" + UserManager.Instance.coins);
        coinsText.text = UserManager.Instance.coins + "";
    } 
}
