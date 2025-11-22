using System;
using UnityEngine;

public static class AppContext
{
    public static string UserId { get; private set; }
    public static string UserName { get; private set; }
    public static string UserClass { get; private set; }
    public static string CurrentGroupId { get; private set; }

    public static void InitUser(string username, string userClass)
    {
        if (string.IsNullOrEmpty(username)) username = "guest";

        if (!PlayerPrefs.HasKey("userId"))
        {
            // First-time user → create local ID
            UserId = "user_" + Guid.NewGuid().ToString("N").Substring(0, 6);
            PlayerPrefs.SetString("userId", UserId);
        }
        else
        {
            // Existing user
            UserId = PlayerPrefs.GetString("userId");
        }

        UserName = username;
        UserClass = userClass;

        PlayerPrefs.SetString("userName", username);
        PlayerPrefs.SetString("userClass", userClass);
    }

    public static void SetCurrentGroup(string groupId)
    {
        CurrentGroupId = groupId;
        PlayerPrefs.SetString("currentGroupId", groupId);
    }

    public static void LoadPersistedGroup()
    {
        if (PlayerPrefs.HasKey("currentGroupId"))
            CurrentGroupId = PlayerPrefs.GetString("currentGroupId");
    }
}
