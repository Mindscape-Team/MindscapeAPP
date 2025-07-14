using Newtonsoft.Json;
using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TextToSpeech : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private string apiKey = "sk_de407852dc4970fee634a530ed759a98d6324619dfc7fb7a";
    private string voiceId = "1SM7GgM6IMuvQlz2BwM3";
    
    public IEnumerator GenerateVoice(string textToSpeak, System.Action<bool> onComplete)
    {
        string url = $"https://api.elevenlabs.io/v1/text-to-speech/{voiceId}";

        var payload = new
        {
            text = textToSpeak,
            voice_settings = new
            {
                speed = 1,
                stability = 0.5,
                similarity_boost = 0.75
            }
        };

        string json = JsonConvert.SerializeObject(payload);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("xi-api-key", apiKey);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error: {request.error}");
            Debug.LogError("Response: " + request.downloadHandler.text);
        }
        else
        {
            byte[] audioData = request.downloadHandler.data;
            string tempPath = Application.persistentDataPath + "/tempVoice.mp3";

            System.IO.File.WriteAllBytes(tempPath, audioData);

            using (UnityWebRequest audioRequest = UnityWebRequestMultimedia.GetAudioClip(tempPath, AudioType.MPEG))
            {
                yield return audioRequest.SendWebRequest();

                if (audioRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Audio Load Error: " + audioRequest.error);

                    onComplete?.Invoke(false);
                }
                else
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(audioRequest);
                    audioSource.clip = clip;
                    audioSource.Play();

                    onComplete?.Invoke(true);

                    //Debug.Log("Audio is playing!");
                }
            }

        }
    }
}
