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
        // ---------------------------
        // USER ID (persistent)
        // ---------------------------
        if (!PlayerPrefs.HasKey("userId"))
        {
            UserId = "user_" + Guid.NewGuid().ToString("N").Substring(0, 6);
            PlayerPrefs.SetString("userId", UserId);
        }
        else
        {
            UserId = PlayerPrefs.GetString("userId");
        }

        // ---------------------------
        // USERNAME + CLASS (only overwrite if first time)
        // ---------------------------
        if (!PlayerPrefs.HasKey("userName"))
            PlayerPrefs.SetString("userName", username);

        if (!PlayerPrefs.HasKey("userClass"))
            PlayerPrefs.SetString("userClass", userClass);

        UserName = PlayerPrefs.GetString("userName");
        UserClass = PlayerPrefs.GetString("userClass");
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
