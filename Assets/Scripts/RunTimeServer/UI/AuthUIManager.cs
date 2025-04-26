using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AuthUIManager : MonoBehaviour
{
    public TMP_InputField tokenInput;
    public Button loginButton;
    public TMP_Text statusText;
    public GameObject authPanel;
    public AuthService auth;
    public string sceneName;

    private const string TokenKey = "AuthToken";

    private void Start()
    {
        loginButton.onClick.AddListener(OnLoginClicked);

        // 🔁 Auto-fill saved token if exists
        if (PlayerPrefs.HasKey(TokenKey))
        {
            tokenInput.SetTextWithoutNotify(PlayerPrefs.GetString(TokenKey));
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log($"statusText: {statusText}");
        Debug.Log($"FontAsset: {statusText.font}");
        Debug.Log($"Font Material: {statusText.font.material}");
    }

    public void OnLoginClicked()
    {
        string token = tokenInput.text.Trim();
        if (string.IsNullOrEmpty(token))
        {
            statusText.text = "Token cannot be empty.";
            return;
        }

        statusText.SetText("Authorizing...");
        PlayerPrefs.SetString(TokenKey, token);
        PlayerPrefs.Save();

        auth.OnLoginSuccess += OnLoginSuccess;
        auth.OnLoginError += OnLoginError;
        auth.Authenticate();
    }

    private void OnLoginSuccess(string playerId)
    {
        statusText.SetText($"✅ Success. ID: {playerId}");

        // 🚀 Load main scene
        SceneManager.LoadScene(sceneName);
    }

    private void OnLoginError(string reason)
    {
        statusText.SetText($"❌ Error: {reason}");
    }
}
