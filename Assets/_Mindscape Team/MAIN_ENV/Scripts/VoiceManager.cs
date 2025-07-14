using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class VoiceManager : MonoBehaviour
{
    [SerializeField] private SessionManager sessionManager;

    public IEnumerator SendAudio(byte[] audioData)
    {
        //Debug.Log("Sending audio to audio emotion API...");

        WWWForm form = new WWWForm();
        form.AddBinaryData("audio", audioData, "audio.wav", "audio/wav");

        using (UnityWebRequest request = UnityWebRequest.Post("https://nada013-audio-emotion-recognition.hf.space/predict", form))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("API error: " + request.error);
            }
            else
            {
                AudioDetectionResponse result = JsonConvert.DeserializeObject<AudioDetectionResponse>(request.downloadHandler.text);

                if (result.predicted_emotion != string.Empty)
                {
                    string emotion = result.predicted_emotion;
                    double confidence = Math.Round(result.confidence, 2);

                    string finalScore = $"{emotion} {confidence * 100}%";
                    Debug.Log($"AUDIO Emotion Detection: {finalScore}");

                    sessionManager.AddAudioEmotionToDict(emotion, confidence);
                }
                else
                {
                    Debug.Log("No emotion detected.");

                    sessionManager.AddAudioEmotionToDict("No Emotion", 0);
                }
            }
        }
    }
}
