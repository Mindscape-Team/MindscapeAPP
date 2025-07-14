using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;
using UnityEngine.Windows;

public class SpeechToText : MonoBehaviour
{
    private string apiKey = "sk_de407852dc4970fee634a530ed759a98d6324619dfc7fb7a";

    public IEnumerator GenerateSpeech(byte[] audioData, System.Action<string> onComplete)
    {
        string url = "https://api.elevenlabs.io/v1/speech-to-text";
        var formData = new List<IMultipartFormSection>
    {
        new MultipartFormFileSection("file", audioData, "audio.wav", "audio/wav"),
        new MultipartFormDataSection("model_id", "scribe_v1")
    };

        var request = UnityWebRequest.Post(url, formData);
        request.SetRequestHeader("xi-api-key", apiKey);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"STT Error: {request.error}");
            Debug.LogError("Response: " + request.downloadHandler.text);
            onComplete?.Invoke(null);
        }
        else
        {
            try
            {
                string jsonResponse = request.downloadHandler.text;
                var sttResult = JsonConvert.DeserializeObject<STTResponse>(jsonResponse);
                string cleanedText = CleanText(sttResult.text);

                //Debug.Log("Transcribed Text: " + cleanedText);
                onComplete?.Invoke(cleanedText);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to parse STT response: " + e.Message);
                onComplete?.Invoke(null);
            }
        }
    }


    string CleanText(string input)
    {
        string cleaned = Regex.Replace(input, @"\s*\([^)]*\)\s*", " ");

        return Regex.Replace(cleaned, @"\s{2,}", " ").Trim();
    }
}
