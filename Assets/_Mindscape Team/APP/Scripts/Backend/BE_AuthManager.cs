using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class BE_AuthManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField[] registerScreenTxtFields;
    [SerializeField] private TMP_InputField[] loginScreenTxtFields;
    //[SerializeField] private GameObject homeScreen;
    //[SerializeField] private GameObject navbar;

    private string apiUrl = "https://localhost:44314/api";

    public void Register()
    {
        var newUser = new UserRegister
        {
            fullName = registerScreenTxtFields[0].text.Trim(),
            email = registerScreenTxtFields[1].text.Trim(),
            password = registerScreenTxtFields[2].text.Trim(),
            confirmPassword = registerScreenTxtFields[3].text.Trim()
        };

        string registerUrl = $"{apiUrl}/Auth/register";
        string jsonContent = JsonUtility.ToJson(newUser);

        Request(registerUrl, jsonContent);
    }

    public void Login()
    {
        var user = new UserLogin
        {
            email = loginScreenTxtFields[0].text.Trim(),
            password = loginScreenTxtFields[1].text.Trim(),
        };

        string loginUrl = $"{apiUrl}/Auth/login";
        string jsonContent = JsonUtility.ToJson(user);

        Request(loginUrl, jsonContent);
    }

    public void Request(string url, string jsonContent)
    {
        UnityWebRequest request = new UnityWebRequest(url, "POST")
        {
            uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonContent)),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");

        StartCoroutine(SendRequest(request));
    }

    private IEnumerator SendRequest(UnityWebRequest request)
    {
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            AuthResponse authResponse = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);
            SaveUserSession(authResponse);

            //homeScreen.gameObject.SetActive(true);
            //navbar.gameObject.SetActive(true);

            //string selectedDate = System.DateTime.UtcNow.ToString("yyyy-MM-dd");
            //StartCoroutine(BEHelperMethods.GetMedicinesByDate(selectedDate, HandleGetAllMedicinesResponse));
        }
        else
        {
            Debug.LogError($"Process Failed! Status: {request.responseCode} - {request.error}");
        }
    }

    private void SaveUserSession(AuthResponse response)
    {
        PlayerPrefs.SetString("Token", response.token);
        PlayerPrefs.SetString("Expires On", response.expireOn);
        PlayerPrefs.SetString("User Name", response.userName);
        PlayerPrefs.SetString("User Email", response.email);
        PlayerPrefs.SetString("User Full Name", response.fullName);
        PlayerPrefs.SetString("User Profile Picture", string.Empty);    

        if (response.hasAnxietyResponse)
            PlayerPrefs.SetString("User Anxiety Survey", "1");
        else
            PlayerPrefs.SetString("User Anxiety Survey", "0");

        if (response.hasDepressionResponse)
            PlayerPrefs.SetString("User Depression Survey", "1");
        else
            PlayerPrefs.SetString("User Depression Survey", "0");

        PlayerPrefs.Save();
    }

    //private void HandleGetAllMedicinesResponse(bool success, List<Medicine> medicines, string response)
    //{
    //    if (success && medicines != null && medicines.Count > 0)
    //    {
    //        Debug.Log($"{medicines.Count} medicines loaded!");
    //    }
    //    else
    //    {
    //        Debug.Log(" No medicines found.");
    //    }
    //}
}
