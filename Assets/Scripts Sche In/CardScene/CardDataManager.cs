using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class Card
{
    public string cardID;
    public string type; // "definition" or "multiple_choice"
    public string question;
    public string answer;
    public List<string> choices;
    public int correctChoiceIndex;
    public string colorHex;
}

[System.Serializable]
public class CardSet
{
    public string setName;
    public List<Card> cards = new List<Card>();
}

public static class DataManager
{
    public static List<CardSet> allSets = new List<CardSet>();
    private static string savePath => Path.Combine(Application.persistentDataPath, "flashcard_data.json");

    // -----------------------------
    // Load data from disk
    // -----------------------------
    public static void LoadData()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("No saved data found, starting new.");
            allSets = new List<CardSet>();
            return;
        }

        string json = File.ReadAllText(savePath);
        SaveWrapper wrapper = JsonUtility.FromJson<SaveWrapper>(json);
        allSets = wrapper.sets ?? new List<CardSet>();

        Debug.Log($"Loaded {allSets.Count} sets from disk.");
    }

    // -----------------------------
    // Save data to disk
    // -----------------------------
    public static void SaveData()
    {
        SaveWrapper wrapper = new SaveWrapper { sets = allSets };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"Data saved to {savePath}");
    }

    // -----------------------------
    // Utility methods
    // -----------------------------
    public static CardSet GetSet(string name)
    {
        return allSets.Find(s => s.setName == name);
    }

    public static void AddCardToSet(Card newCard, string setName)
    {
        CardSet target = GetSet(setName);
        if (target == null)
        {
            target = new CardSet { setName = setName };
            allSets.Add(target);
        }
        target.cards.Add(newCard);
        SaveData(); // auto-save after adding card
    }

    public static void DeleteSet(string name)
    {
        var set = GetSet(name);
        if (set != null)
        {
            allSets.Remove(set);
            SaveData();
        }
    }

    [System.Serializable]
    private class SaveWrapper
    {
        public List<CardSet> sets;
    }
}
