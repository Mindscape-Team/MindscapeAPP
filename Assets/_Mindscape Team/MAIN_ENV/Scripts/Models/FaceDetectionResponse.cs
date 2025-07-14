using UnityEngine;


[System.Serializable]
public class Prediction
{
    public string emotion;
    public double confidence;
}

[System.Serializable]
public class FaceDetectionResponse
{
    public bool success;
    public Prediction[] predictions;
    public int faces_detected;
}
