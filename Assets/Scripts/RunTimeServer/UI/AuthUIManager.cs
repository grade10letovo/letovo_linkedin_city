using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Collections;

public class AuthUIManager : MonoBehaviour
{
    public TMP_InputField tokenInput;
    public Button loginButton;
    public TMP_Text statusText;
    public GameObject authPanel;
    public AuthService auth;
    public string sceneName;

    private const string TokenKey = "AuthToken";

    private Coroutine warningResetCoroutine;
    private Color originalInputColor;

    private void Start()
    {
        try
        {
            loginButton.onClick.AddListener(OnLoginClicked);

            if (PlayerPrefs.HasKey(TokenKey))
            {
                tokenInput.SetTextWithoutNotify(PlayerPrefs.GetString(TokenKey));
            }

            EnglishOnlyInput englishOnly = tokenInput.GetComponent<EnglishOnlyInput>();
            if (englishOnly != null)
            {
                englishOnly.OnInvalidInputDetected += HandleInvalidInput;
            }
            else
            {
                Debug.LogWarning("[AuthUIManager] ⚠️ EnglishOnlyInput component not found on tokenInput.");
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Сохраняем оригинальный цвет фона инпута
            if (tokenInput.image != null)
            {
                originalInputColor = tokenInput.image.color;
            }
            else
            {
                Debug.LogWarning("[AuthUIManager] ⚠️ No Image component on tokenInput for color feedback.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AuthUIManager] ❌ Error during Start: {ex}");
        }
    }

    public void OnLoginClicked()
    {
        try
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
        catch (Exception ex)
        {
            Debug.LogError($"[AuthUIManager] ❌ Error during OnLoginClicked: {ex}");
            statusText.SetText("Unexpected error during login.");
        }
    }

    private void OnLoginSuccess(string playerId)
    {
        try
        {
            statusText.SetText($"✅ Success. ID: {playerId}");
            SceneManager.LoadScene(sceneName);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AuthUIManager] ❌ Error during OnLoginSuccess: {ex}");
            statusText.SetText("Unexpected error after login success.");
        }
    }

    private void OnLoginError(string reason)
    {
        try
        {
            statusText.SetText($"❌ Error: {reason}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AuthUIManager] ❌ Error during OnLoginError: {ex}");
            statusText.SetText("Unexpected error after login error.");
        }
    }

    private void HandleInvalidInput(string invalidText)
    {
        Debug.LogWarning($"[AuthUIManager] ⚠️ Invalid input detected: {invalidText}");

        statusText.SetText("Only English letters are allowed.");

        HighlightInputField(Color.red);

        if (warningResetCoroutine != null)
        {
            StopCoroutine(warningResetCoroutine);
        }
        warningResetCoroutine = StartCoroutine(ResetWarningAfterDelay(2f));
    }

    private IEnumerator ResetWarningAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        statusText.SetText(string.Empty);
        HighlightInputField(originalInputColor);
    }

    private void HighlightInputField(Color color)
    {
        if (tokenInput.image != null)
        {
            tokenInput.image.color = color;
        }
    }
}
