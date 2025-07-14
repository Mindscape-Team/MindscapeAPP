using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class BE_PasswordManager : MonoBehaviour
{
    private string changePasswordUrl = "https://localhost:44314/api/User/ChangePassword";

    [SerializeField] private TMP_InputField currentPasswordInput;
    [SerializeField] private TMP_InputField newPasswordInput;
    [SerializeField] private TMP_InputField confirmPasswordInput;

    [SerializeField] private GameObject successMessagePanel;

    public void OnChangePasswordClick()
    {
        string token = PlayerPrefs.GetString("Token");

        if (string.IsNullOrEmpty(token))
        {
            Debug.Log("User not logged in!");
            return;
        }

        if (newPasswordInput.text == confirmPasswordInput.text)
        {
            ChangePasswordRequest requestData = new ChangePasswordRequest
            {
                currentPassword = currentPasswordInput.text,
                newPassword = newPasswordInput.text,
                confirmPassword = confirmPasswordInput.text
            };

            string jsonData = JsonUtility.ToJson(requestData);

            StartCoroutine(SendChangePasswordRequest(jsonData, token));
        }
    }

    private IEnumerator SendChangePasswordRequest(string jsonData, string token)
    {
        using (UnityWebRequest request = new UnityWebRequest(changePasswordUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + token);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Password changed successfully!");
                successMessagePanel.SetActive(true);
            }
            else
            {
                Debug.LogError("Password change failed: " + request.downloadHandler.text);
            }
        }
    }
}
