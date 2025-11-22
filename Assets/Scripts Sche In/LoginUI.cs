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
        errorLabel.SetActive(false);
    }

    public async void OnConfirm()
    {
        if (loading) return;

        string username = usernameField.text.Trim();
        string userClass = classDropdown.options[classDropdown.value].text;

        // --- VALIDATION ---
        if (string.IsNullOrEmpty(username))
        {
            errorLabel.SetActive(true);
            errorLabel.GetComponent<TMP_Text>().text = "Please enter a username.";
            return;
        }

        loading = true;

        // --- 1. Initialize LOCAL user ---
        AppContext.InitUser(username, userClass);

        // --- 2. Create user entry in Firebase ---
        UserManager.Instance.CreateUserInFirebase(username, userClass);

        // --- 3. Load user data ---
        UserManager.Instance.LoadUserFromFirebase();

        // --- 4. Go to quiz screen ---
        SceneManager.LoadScene("GardenSingleUser");
    }
}
