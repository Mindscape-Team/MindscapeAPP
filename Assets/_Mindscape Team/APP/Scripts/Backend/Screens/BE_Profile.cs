using System.Collections;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class BE_Profile
{
    public static IEnumerator FetchUserProfile()
    {
        string fetchDataApiUrl = "https://localhost:44314/api/User/RetrieveProfile";

        string token = PlayerPrefs.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            Debug.LogError("No token found! User needs to log in.");
            yield break;
        }

        using (UnityWebRequest request = UnityWebRequest.Get(fetchDataApiUrl))
        {
            request.SetRequestHeader("Authorization", "Bearer " + token);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;

                UserProfile userProfile = JsonUtility.FromJson<UserProfile>(jsonResponse);

                if (userProfile != null)
                {
                    PlayerPrefs.SetString("Full Name", userProfile.fullName);
                    //PlayerPrefs.SetString("Email", userProfile.email);
                    PlayerPrefs.SetString("Phone Number", userProfile.phoneNumber);
                    PlayerPrefs.SetString("Address", userProfile.address);
                    PlayerPrefs.SetString("Profile Picture", userProfile.profilePicture);

                    PlayerPrefs.Save();
                }
                else
                {
                    Debug.LogError("Deserialization Failed!");
                }
            }
            else
            {
                Debug.LogError("API Request Failed: " + request.error);
            }
        }
    }

    public static async Task<bool> UpdateProfile(TMP_InputField[] inputFields)
    {
        string updateApiUrl = "https://localhost:44314/api/User/UpdateProfile";

        string token = PlayerPrefs.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            Debug.LogError("No token found. User needs to log in.");
            return false;
        }

        UserProfile updatedUser = new UserProfile
        {
            fullName = !string.IsNullOrEmpty(inputFields[0].text) ? inputFields[0].text : null,
            phoneNumber = !string.IsNullOrEmpty(inputFields[1].text) ? inputFields[1].text : null,
            address = !string.IsNullOrEmpty(inputFields[2].text) ? inputFields[2].text : null,
        };

        string jsonData = JsonUtility.ToJson(updatedUser);
        Debug.Log("Request JSON: " + jsonData);

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(updateApiUrl, "PUT"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + token);

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("User profile updated successfully!");
                return true;
            }
            else
            {
                Debug.LogError($"Failed to update profile: {request.error} (HTTP {request.responseCode})");
                Debug.LogError("Response: " + request.downloadHandler.text);
                return false;
            }
        }
    }
}
