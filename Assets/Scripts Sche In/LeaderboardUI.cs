using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LeaderboardUI : MonoBehaviour
{
    public GameObject leaderboardItemPrefab;
    public Transform contentParent;

    private void OnEnable()
    {
        LeaderboardManager.Instance.OnLeaderboardUpdated += UpdateUI;
    }

    private void OnDisable()
    {
        LeaderboardManager.Instance.OnLeaderboardUpdated -= UpdateUI;
    }

    private void UpdateUI(List<LeaderboardEntry> entries)
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        int rank = 1;
        foreach (var entry in entries)
        {
            GameObject item = Instantiate(leaderboardItemPrefab, contentParent);

            item.transform.Find("Rank").GetComponent<TMP_Text>().text = rank.ToString();
            item.transform.Find("Name").GetComponent<TMP_Text>().text = entry.userName;
            item.transform.Find("Score").GetComponent<TMP_Text>().text = entry.score.ToString();

            rank++;
        }
    }
}
