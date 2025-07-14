using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SocialPlatforms.Impl;

public class BE_HelperMethods
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

    public static List<T> FromJson<T>(string json)
    {
        Debug.Log("Raw JSON: " + json);
        string wrappedJson = $"{{\"items\":{json}}}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrappedJson);
        return wrapper != null ? wrapper.items : new List<T>();
    }

    [Serializable]
    private class Wrapper<T>
    {
        public List<T> items;
    }
}
