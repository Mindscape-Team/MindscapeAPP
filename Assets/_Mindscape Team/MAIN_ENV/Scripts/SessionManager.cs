using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SessionManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Button startRecordingBtn;
    [SerializeField] private Button stopRecordingBtn;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject finalReportScreen;
    [SerializeField] private UI_FinalReport finalReportUI;

    [Header("Managers Components")]
    [SerializeField] private TextToSpeech ttsManager;
    [SerializeField] private SpeechToText sttManager;
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private VoiceManager voiceManager;

    private string cpuModel = "https://nada013-conversational-chat.hf.space";
    private string gpuModel = "https://nada013-chat-gpu.hf.space";

    private string userId;
    private string sessionId;

    private AudioClip recordedClip;
    private string micDevice;
    private int sampleRate = 44100;
    private bool isRecording = false;

    private List<KeyValuePair<string, double>> faceEmotionScores;
    private List<KeyValuePair<string, double>> audioEmotionsScores;

    private string dateTime;
    private int count = 0;

    private void Start()
    {
        sessionId = string.Empty;
        dateTime = string.Empty;

        stopRecordingBtn.gameObject.SetActive(false);
        startRecordingBtn.interactable = false;

        //startRecordingBtn.gameObject.SetActive(false);

        StartCoroutine(StartSession((success) =>
        {
            if (success)
            {
                startRecordingBtn.interactable = true;
            }
        }));
    }

    public void OnStartRecording()
    {
        dateTime = string.Empty;

        startRecordingBtn.gameObject.SetActive(false);
        stopRecordingBtn.gameObject.SetActive(true);

        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone detected on device.");
            return;
        }

        micDevice = Microphone.devices[0];
        //Debug.Log("Using microphone: " + micDevice);

        isRecording = true;
        recordedClip = Microphone.Start(micDevice, true, 300, sampleRate);
        //Debug.Log("Recording started...");
    }

    public void OnStopRecording()
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        dateTime = timestamp;

        startRecordingBtn.gameObject.SetActive(true);
        stopRecordingBtn.gameObject.SetActive(false);

        startRecordingBtn.interactable = false;

        if (!isRecording || recordedClip == null)
        {
            Debug.LogWarning("Recording was not started.");
            return;
        }

        int position = Microphone.GetPosition(micDevice);
        Microphone.End(micDevice);
        isRecording = false;

        //Debug.Log("Recording stopped at sample position: " + position);

        float[] samples = new float[position * recordedClip.channels];
        recordedClip.GetData(samples, 0);

        AudioClip trimmedClip = AudioClip.Create("TrimmedClip", position, recordedClip.channels, recordedClip.frequency, false);
        trimmedClip.SetData(samples, 0);

        byte[] audioData = WavUtility.ConvertAudioClipToWav(trimmedClip);

        //Debug.Log("Audio byte length: " + audioData.Length);
        //Debug.Log("First few bytes: " + System.BitConverter.ToString(audioData, 0, Mathf.Min(20, audioData.Length)));

        System.IO.File.WriteAllBytes(Application.persistentDataPath + "/test.wav", audioData);
        //Debug.Log("Saved audio to: " + Application.persistentDataPath + "/test.wav");

        StartCoroutine(voiceManager.SendAudio(audioData));
        cameraManager.CaptureFrame();

        StartCoroutine(sttManager.GenerateSpeech(audioData, (text) =>
        {
            if (text != null)
            {
                //Debug.Log("sending...");
                StartCoroutine(SendMessage(text, (success) =>
                {
                    startRecordingBtn.interactable = true;
                }));
            }
        }));
    }

    public void OnEndingSession()
    {
        StartCoroutine(HandleEndSessionFlow());
    }

    private IEnumerator HandleEndSessionFlow()
    {
        loadingScreen.SetActive(true);

        bool isDone = false;
        SessionSummaryResponse result = null;

        yield return StartCoroutine(EndSession((response) =>
        {
            result = response;
            isDone = true;
        }));

        yield return new WaitUntil(() => isDone);

        loadingScreen.SetActive(false);

        if (result != null)
        {
            finalReportScreen.SetActive(true);
            finalReportUI.ChangeUI(result, faceEmotionScores, audioEmotionsScores);
        }
        else
        {
            Debug.LogError("EndSession failed. Show error UI.");
        }
    }

    private IEnumerator StartSession(System.Action<bool> onComplete)
    {
        faceEmotionScores = new List<KeyValuePair<string, double>>();
        audioEmotionsScores = new List<KeyValuePair<string, double>>();

        string userId = PlayerPrefs.GetString("Token").Substring(0, 10);
        PlayerPrefs.SetString("User Id", userId);
        //Debug.Log(userId);

        string startSessionUrl = $"{cpuModel}/start_session?user_id={userId}";
        //string startSessionUrl = $"{gpuModel}/start_session?user_id={userId}";

        var payload = new
        {
            user_id = userId,
        };

        string json = JsonConvert.SerializeObject(payload);

        UnityWebRequest request = new UnityWebRequest(startSessionUrl, "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error: {request.error}");
            Debug.LogError("Response: " + request.downloadHandler.text);

            onComplete?.Invoke(false);
        }
        else
        {
            ChatSessionResponse sessionResponse = JsonUtility.FromJson<ChatSessionResponse>(request.downloadHandler.text);
            string text = sessionResponse.response + $"{PlayerPrefs.GetString("User Full Name")}?";

            PlayerPrefs.SetString("Session Id", sessionResponse.session_id);

            Debug.Log("Chatbot: " + sessionResponse.response);
            //Debug.Log("Session ID: " + sessionResponse.session_id);

            StartCoroutine(ttsManager.GenerateVoice("Hello! I'm here to support you today. How have you been feeling lately?", (success) =>
            {
                if (success)
                    onComplete?.Invoke(true);
                else
                    onComplete?.Invoke(false);
            }));
        }
    }

    private IEnumerator SendMessage(string message, System.Action<bool> onComplete)
    {
        string userId = PlayerPrefs.GetString("Token").Substring(0, 10);
        //Debug.Log(userId);

        Debug.Log($"Message Recieved. Processing Message: '{message}'");

        string sendMessageUrl = $"{cpuModel}/send_message";
        //string sendMessageUrl = $"{gpuModel}/send_message";

        var payload = new
        {
            user_id = userId,
            message = message
        };

        string json = JsonConvert.SerializeObject(payload);

        UnityWebRequest request = new UnityWebRequest(sendMessageUrl, "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error: {request.error}");
            Debug.LogError("Response: " + request.downloadHandler.text);

            onComplete?.Invoke(false);
        }
        else
        {
            ChatSessionResponse sessionResponse = JsonUtility.FromJson<ChatSessionResponse>(request.downloadHandler.text);
            string text = sessionResponse.response;

            Debug.Log("Chatbot: " + sessionResponse.response);
            //Debug.Log("Session ID: " + sessionResponse.session_id);

            StartCoroutine(ttsManager.GenerateVoice(text, (success) =>
            {
                if (success)
                    onComplete?.Invoke(true);
                else
                    onComplete?.Invoke(false);
            }));
        }

    }

    private IEnumerator EndSession(System.Action<SessionSummaryResponse> onComplete)
    {
        string userId = PlayerPrefs.GetString("Token").Substring(0, 10);
        //Debug.Log(userId);

        string endSessionUrl = $"{cpuModel}/end_session?user_id={userId}";
        //string endSessionUrl = $"{gpuModel}/end_session?user_id={userId}";

        var payload = new
        {
            user_id = userId,
        };

        string json = JsonConvert.SerializeObject(payload);

        UnityWebRequest request = new UnityWebRequest(endSessionUrl, "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error: {request.error}");
            Debug.LogError("Response: " + request.downloadHandler.text);

            onComplete?.Invoke(null);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);

            SessionSummaryResponse sessionResponse = JsonConvert.DeserializeObject<SessionSummaryResponse>(request.downloadHandler.text);

            onComplete?.Invoke(sessionResponse);
        }
    }

    //private IEnumerator GetUserReplies()
    //{
    //    string userId = PlayerPrefs.GetString("Token").Substring(0, 10);
    //    Debug.Log(userId);

    //    string userRepliesUrl = $"https://nada013-chat-gpu.hf.space/user_replies/{userId}";

    //    UnityWebRequest request = new UnityWebRequest(userRepliesUrl, "GET");
    //}

    public void AddFaceEmotionToDict(string emotion, double emotionScore)
    {
        var keyValuePair = new KeyValuePair<string, double>(emotion, emotionScore);
        faceEmotionScores.Add(keyValuePair);
    }

    public void AddAudioEmotionToDict(string emotion, double emotionScore)
    {
        var keyValuePair = new KeyValuePair<string, double>(emotion, emotionScore);
        audioEmotionsScores.Add(keyValuePair);
    }
}
