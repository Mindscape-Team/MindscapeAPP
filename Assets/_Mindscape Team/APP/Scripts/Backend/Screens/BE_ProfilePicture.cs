using System.Text;
using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BE_ProfilePicture : MonoBehaviour
{
    private string updateApiUrl = "https://localhost:44314/api/User/UpdateProfilePicture";

    public void SaveProfilePic()
    {
        byte[] profilePicture = UI_ProfilePicture.imageData;

        string token = PlayerPrefs.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            Debug.LogError("No token found. User needs to log in.");
        }

        string base64String = Convert.ToBase64String(profilePicture);

        ImageDataWrapper wrapper = new ImageDataWrapper { profilePicture = base64String };
        string jsonData = JsonUtility.ToJson(wrapper);

        Request(updateApiUrl, token, jsonData);
    }

    public void Request(string url, string token, string jsonContent)
    {
        UnityWebRequest request = new UnityWebRequest(url, "PUT")
        {
            uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonContent)),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + token);

        StartCoroutine(SendRequest(request));
    }

    private IEnumerator SendRequest(UnityWebRequest request)
    {
        yield return request.SendWebRequest();

        var data = request.downloadHandler.data;

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("User profile picture updated successfully!");
        }
        else
        {
            Debug.LogError($"Failed to update profile: {request.error} (HTTP {request.responseCode})");
            Debug.LogError("Response: " + request.downloadHandler.text);
        }
    }

    [Serializable]
    public class ImageDataWrapper
    {
        public string profilePicture;
    }
}
