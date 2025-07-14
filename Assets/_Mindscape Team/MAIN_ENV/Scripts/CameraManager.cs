using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Renderer cameraLiveRenderer; 
    [SerializeField] private Renderer cameraFrameRenderer; 
    private WebCamTexture webcamTexture;

    [SerializeField] private SessionManager sessionManager;

    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        
        //foreach(var device in devices)
        //{
        //    Debug.Log(device.name);
        //}

        if (devices.Length > 0)
        {
            webcamTexture = new WebCamTexture(devices[0].name);
            cameraLiveRenderer.material.mainTexture = webcamTexture;
            webcamTexture.Play();
        }
        else
        {
            Debug.LogWarning("No webcam found.");
        }
    }

    public void CaptureFrame()
    {
        Texture2D snap = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.RGB24, false);
        snap.SetPixels32(webcamTexture.GetPixels32());
        snap.Apply();

        byte[] imageBytes = snap.EncodeToPNG();

        //Debug.Log("Captured frame, size: " + imageBytes.Length);

        if (cameraFrameRenderer != null)
        {
            cameraFrameRenderer.material.mainTexture = snap;
        }

        StartCoroutine(SendImage(imageBytes));
    }

    private IEnumerator SendImage(byte[] imageData)
    {
        //Debug.Log("Sending image to emotion API...");

        string base64Image = Convert.ToBase64String(imageData);

        WWWForm form = new WWWForm();
        form.AddField("image_data", base64Image);

        using (UnityWebRequest request = UnityWebRequest.Post("https://nada013-face-emotions.hf.space/predict-base64", form))
        {
            request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("API error: " + request.error);
            }
            else
            {
                FaceDetectionResponse result = JsonUtility.FromJson<FaceDetectionResponse>(request.downloadHandler.text);

                if (result.success && result.predictions.Length > 0)
                {
                    string emotion = result.predictions[0].emotion;
                    double confidence = result.predictions[0].confidence;

                    string finalScore = $"{emotion} {confidence * 100}%";
                    Debug.Log($"FACE Emotion Detection: {finalScore}");

                    sessionManager.AddFaceEmotionToDict(emotion, confidence);
                }
                else
                {
                    Debug.Log("No emotion detected.");

                    sessionManager.AddFaceEmotionToDict("No Emotion", 0);
                }
            }
        }
    }
}
