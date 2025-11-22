using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

public class OnlineCardManager : MonoBehaviour
{
    public static OnlineCardManager Instance;
    private DatabaseReference db;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        db = FirebaseDatabase.DefaultInstance.RootReference;

        // Automatisches Uploaden
        DataManager.OnSetChanged += UploadOrUpdateSet;
    }

    // ------------------------
    // Upload a new card set
    // ------------------------
    private void UploadOrUpdateSet(CardSet set)
    {
        string setId = PlayerPrefs.GetString("setId_" + set.setName, "");

        if (string.IsNullOrEmpty(setId))
        {
            // Neues Set → generiere ID
            setId = "set_" + Guid.NewGuid().ToString("N").Substring(0, 6);
            PlayerPrefs.SetString("setId_" + set.setName, setId);
        }

        UploadSet(set, 0, setId); // price = 0, bereits gekauft vom User selbst
    }

    public void UploadSet(CardSet set, int price, string setId)
    {
        var cardsDict = new Dictionary<string, object>();
        foreach (var c in set.cards)
        {
            cardsDict[c.cardID] = new Dictionary<string, object>
        {
            { "type", c.type },
            { "question", c.question },
            { "answer", c.answer },
            { "choices", c.choices ?? new List<string>() },
            { "correctChoiceIndex", c.correctChoiceIndex },
            { "colorHex", c.colorHex }
        };
        }

        var data = new Dictionary<string, object>
    {
        { "setName", set.setName },
        { "authorId", AppContext.UserId },
        { "price", price },
        { "cards", cardsDict }
    };

        db.Child("cardSets").Child(setId).SetValueAsync(data);
    }

    // ------------------------
    // Buy a card set
    // ------------------------
    public void BuySet(string setId)
    {
        string userId = AppContext.UserId;

        db.Child("cardSets").Child(setId).GetValueAsync().ContinueWithOnMainThread(t =>
        {
            if (t.IsFaulted || !t.Result.Exists) return;

            int price = Convert.ToInt32(t.Result.Child("price").Value);
            if (UserManager.Instance.coins >= price)
            {
                UserManager.Instance.AddCoins(-price);

                // Add to user's purchased sets
                db.Child("users").Child(userId).Child("purchasedSets").Child(setId).SetValueAsync(true);

                // Download to local immediately
                DownloadSet(setId);
            }
        });
    }

    // ------------------------
    // Download a single set and merge with DataManager
    // ------------------------
    public void DownloadSet(string setId)
    {
        db.Child("cardSets").Child(setId).GetValueAsync().ContinueWithOnMainThread(t =>
        {
            if (t.IsFaulted || !t.Result.Exists) return;

            var snap = t.Result;
            CardSet set = new CardSet { setName = snap.Child("setName").Value.ToString() };

            foreach (var cardSnap in snap.Child("cards").Children)
            {
                Card c = new Card
                {
                    cardID = cardSnap.Key,
                    type = cardSnap.Child("type").Value.ToString(),
                    question = cardSnap.Child("question").Value.ToString(),
                    answer = cardSnap.Child("answer").Value.ToString(),
                    correctChoiceIndex = int.Parse(cardSnap.Child("correctChoiceIndex").Value.ToString()),
                    colorHex = cardSnap.Child("colorHex").Value.ToString(),
                    choices = new List<string>()
                };

                if (cardSnap.Child("choices").Exists)
                {
                    foreach (var ch in cardSnap.Child("choices").Children)
                        c.choices.Add(ch.Value.ToString());
                }

                set.cards.Add(c);
            }

            // Merge with local DataManager if not exists
            if (DataManager.GetSet(set.setName) == null)
            {
                DataManager.allSets.Add(set);
                DataManager.SaveData();
            }
        });
    }

    // ------------------------
    // Download all sets owned by current user
    // ------------------------
    public void DownloadAllUserSets(Action callback = null)
    {
        string userId = AppContext.UserId;

        db.Child("users").Child(userId).Child("purchasedSets")
          .GetValueAsync().ContinueWithOnMainThread(t =>
          {
              if (t.IsFaulted)
              {
                  Debug.LogError("Firebase Fehler: " + t.Exception);
                  callback?.Invoke();
                  return;
              }

              if (!t.Result.Exists || t.Result.ChildrenCount == 0)
              {
                  callback?.Invoke();
                  return;
              }

              int setsCount = (int)t.Result.ChildrenCount;
              int downloaded = 0;

              foreach (var setSnap in t.Result.Children)
              {
                  string setId = setSnap.Key;

                  // Direkt inlined statt DownloadSet, um Task korrekt zu überwachen
                  db.Child("cardSets").Child(setId)
                .GetValueAsync().ContinueWithOnMainThread(s =>
                  {
                      if (!s.IsFaulted && s.Result.Exists)
                      {
                          var snap = s.Result;
                          CardSet set = new CardSet { setName = snap.Child("setName").Value.ToString() };

                          foreach (var cardSnap in snap.Child("cards").Children)
                          {
                              Card c = new Card
                              {
                                  cardID = cardSnap.Key,
                                  type = cardSnap.Child("type").Value.ToString(),
                                  question = cardSnap.Child("question").Value.ToString(),
                                  answer = cardSnap.Child("answer").Value.ToString(),
                                  correctChoiceIndex = int.Parse(cardSnap.Child("correctChoiceIndex").Value.ToString()),
                                  colorHex = cardSnap.Child("colorHex").Value.ToString(),
                                  choices = new List<string>()
                              };

                              if (cardSnap.Child("choices").Exists)
                              {
                                  foreach (var ch in cardSnap.Child("choices").Children)
                                      c.choices.Add(ch.Value.ToString());
                              }

                              set.cards.Add(c);
                          }

                          // Merge in DataManager
                          if (DataManager.GetSet(set.setName) == null)
                          {
                              DataManager.allSets.Add(set);
                              DataManager.SaveData();
                          }
                      }

                      downloaded++;
                      if (downloaded >= setsCount)
                          callback?.Invoke();
                  });
              }
          });
    }

}
