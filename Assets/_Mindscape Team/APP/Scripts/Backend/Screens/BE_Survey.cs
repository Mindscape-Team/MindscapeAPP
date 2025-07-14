using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using System.Collections;

public static class BE_Survey
{
    public static IEnumerator SendSurvey(string submitUrl, List<SurveyAnswer> answers, System.Action<SurveyResult> onResult)
    {
        string token = PlayerPrefs.GetString("Token");

        if (string.IsNullOrEmpty(token))
        {
            Debug.LogError("No token found. User needs to log in.");
        }

        var dto = new SubmitSurvey { answers = answers };
        string json = JsonUtility.ToJson(dto);

        UnityWebRequest request = new UnityWebRequest(submitUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + token);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            SurveyResult result = JsonUtility.FromJson<SurveyResult>(request.downloadHandler.text);
            onResult?.Invoke(result);
        }
        else
        {
            Debug.LogError("Survey submission failed: " + request.error);
            onResult?.Invoke(null);
        }
    }
}
