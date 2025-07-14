using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioDetectionResponse
{
    public string predicted_emotion { get; set; }
    public float confidence { get; set; }
    public Dictionary<string, float> all_probabilities { get; set; }
}
