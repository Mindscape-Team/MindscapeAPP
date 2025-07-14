using Newtonsoft.Json;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class demo : MonoBehaviour
{
    [SerializeField] private Transform msgsContent;
    [SerializeField] private GameObject userMsgPrefab;
    [SerializeField] private GameObject botMsgPrefab;
    [SerializeField] private TMP_InputField txtField;
    [SerializeField] private Button sendBtn;

    private string cpuModel = "https://nada013-conversational-chat.hf.space";
    private string gpuModel = "https://nada013-chat-gpu.hf.space";

    private void Start()
    {
        GameObject botMsg = Instantiate(botMsgPrefab, msgsContent);
        //MessageBubble botBubble = botMsg.GetComponent<MessageBubble>();
        //botBubble.SetMessage("Hello there! How can I help you today?");
        botMsg.GetComponentInChildren<TextMeshProUGUI>().text = "Hello there! How can I help you today?";
    }

    public void OnSendMessage()
    {
        if (string.IsNullOrWhiteSpace(txtField.text)) return;

        GameObject userMsg = Instantiate(userMsgPrefab, msgsContent);
        //MessageBubble userBubble = userMsg.GetComponent<MessageBubble>();
        //userBubble.SetMessage(txtField.text);
        userMsg.GetComponentInChildren<TextMeshProUGUI>().text = txtField.text;

        txtField.text = "";
        txtField.interactable = false;
        sendBtn.interactable = false;

        GameObject botMsg = Instantiate(botMsgPrefab, msgsContent);
        botMsg.GetComponentInChildren<TextMeshProUGUI>().text = "...";

        StartCoroutine(SendMessage(txtField.text, (text) =>
        {
            //MessageBubble botBubble = botMsg.GetComponent<MessageBubble>();
            //botBubble.SetMessage(text);
            botMsg.GetComponentInChildren<TextMeshProUGUI>().text = text;
            txtField.interactable = true;
            sendBtn.interactable = true;
        }));
    }

    private IEnumerator SendMessage(string message, System.Action<string> onComplete)
    {
        string userId = PlayerPrefs.GetString("Token").Substring(0, 10);
        Debug.Log(userId);

        string m = "I’ve been feeling really overwhelmed lately, like everything is too much to handle. Even small tasks feel exhausting.\"";

        Debug.Log($"message recieved. processing message: '{m}'");

        //string sendMessageUrl = $"{cpuModel}/send_message";
        string sendMessageUrl = $"{gpuModel}/send_message";

        var payload = new
        {
            user_id = userId,
            message = m
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

            onComplete?.Invoke("");
        }
        else
        {
            ChatSessionResponse sessionResponse = JsonUtility.FromJson<ChatSessionResponse>(request.downloadHandler.text);
            string text = sessionResponse.response;

            Debug.Log("Chatbot: " + sessionResponse.response);
            Debug.Log("Session ID: " + sessionResponse.session_id);

            onComplete?.Invoke(text);
        }
    }
}
