using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LoginUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField usernameField;
    public TMP_Dropdown classDropdown;
    public GameObject errorLabel;

    private bool loading = false;

    private void Start()
    {
        // Hide error label
        errorLabel.SetActive(false);

        //  AUTO LOGIN
        if (PlayerPrefs.HasKey("userName") && PlayerPrefs.HasKey("userClass") && PlayerPrefs.HasKey("userId"))
        {
            Debug.Log("Auto-login with existing user");

            // Load saved values
            string username = PlayerPrefs.GetString("userName");
            string userClass = PlayerPrefs.GetString("userClass");

            // Initialize AppContext with saved values
            AppContext.InitUser(username, userClass);

            // Load user from Firebase
            UserManager.Instance.LoadUserFromFirebase();

            // Skip login screen
            SceneManager.LoadScene("GardenSingleUser");
        }
    }

    public async void OnConfirm()
    {
        if (loading) return;

        string username = usernameField.text.Trim();
        string userClass = classDropdown.options[classDropdown.value].text;

        if (string.IsNullOrEmpty(username))
        {
            errorLabel.SetActive(true);
            errorLabel.GetComponent<TMP_Text>().text = "Please enter a username.";
            return;
        }

        loading = true;

        // Initialize user
        AppContext.InitUser(username, userClass);

        // Check if user already exists in Firebase
        string uid = AppContext.UserId;
        var db = Firebase.Database.FirebaseDatabase.DefaultInstance.RootReference;

        var snap = await db.Child("users").Child(uid).GetValueAsync();

        if (!snap.Exists)
        {
            // First time  create new user entry
            UserManager.Instance.CreateUserInFirebase(username, userClass);
        }

        // Load user into memory
        UserManager.Instance.LoadUserFromFirebase();

        // Continue to game
        SceneManager.LoadScene("GardenSingleUser");
    }
}
