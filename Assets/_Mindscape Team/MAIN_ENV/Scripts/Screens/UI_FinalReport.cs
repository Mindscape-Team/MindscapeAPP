using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UI_FinalReport : MonoBehaviour
{
    [Header("Panel Components")]
    [SerializeField] private GameObject neutralPanel;
    [SerializeField] private GameObject sadnessPanel;
    [SerializeField] private GameObject stressedPanel;

    [Header("Text Components")]
    [SerializeField] private TextMeshProUGUI dateTxt;
    [SerializeField] private TextMeshProUGUI durationTxt;
    [SerializeField] private TextMeshProUGUI primaryEmotionsTxt;
    [SerializeField] private TextMeshProUGUI emotionProgressTxt;
    [SerializeField] private TextMeshProUGUI summaryTxt;
    [SerializeField] private TextMeshProUGUI faceEmotionTxt;
    [SerializeField] private TextMeshProUGUI voiceEmotionTxt;

    private GameObject selectedPanel;

    public void ChangeUI(SessionSummaryResponse respone, List<KeyValuePair<string, double>> faceEmotionScores, List<KeyValuePair<string, double>> audioEmotionsScores)
    {
        if (ClassifyEmotion(respone.primary_emotions[0]) == "Neutral")
            selectedPanel = neutralPanel;
        else if (ClassifyEmotion(respone.primary_emotions[0]) == "Sadness")
            selectedPanel = sadnessPanel;
        else if (ClassifyEmotion(respone.primary_emotions[0]) == "Stressed")
            selectedPanel = stressedPanel;

        selectedPanel.GetComponentInChildren<TextMeshProUGUI>().text = respone.primary_emotions[0].ToUpper();
        selectedPanel.SetActive(true);

        dateTxt.text = respone.start_time.ToString("dd/MM/yyyy");

        TimeSpan time = TimeSpan.FromMinutes(respone.duration_minutes);
        durationTxt.text = time.ToString(@"hh\:mm\:ss");

        primaryEmotionsTxt.text = string.Join(", ", respone.primary_emotions);

        emotionProgressTxt.text = string.Join("=> ", respone.emotion_progression);

        summaryTxt.text = respone.summary;

        var f_result = ComputeScoresAverage(faceEmotionScores);
        faceEmotionTxt.text = $"{f_result.Key} {f_result.Value * 100}%";

        var a_result = ComputeScoresAverage(audioEmotionsScores);
        voiceEmotionTxt.text = $"{a_result.Key} {a_result.Value * 100}%";
    }

    private string ClassifyEmotion(string emotion)
    {
        emotion = emotion.Trim().ToLower();

        HashSet<string> neutralEmotions = new HashSet<string>
    {
        "admiration", "amusement", "approval", "caring", "curiosity",
        "desire", "excitement", "gratitude", "joy", "love", "optimism",
        "pride", "realization", "relief", "surprise", "neutral"
    };

        HashSet<string> sadnessEmotions = new HashSet<string>
    {
        "disappointment", "disapproval", "grief", "remorse", "sadness",
        "embarrassment"
    };

        HashSet<string> stressedEmotions = new HashSet<string>
    {
        "anger", "annoyance", "confusion", "disgust", "fear", "nervousness"
    };

        if (neutralEmotions.Contains(emotion))
            return "Neutral";
        else if (sadnessEmotions.Contains(emotion))
            return "Sadness";
        else if (stressedEmotions.Contains(emotion))
            return "Stressed";
        else
            return "Unknown";
    }
    private KeyValuePair<string, double> ComputeScoresAverage(List<KeyValuePair<string, double>> emotionsScores)
    {
        var grouped = emotionsScores
            .GroupBy(e => e.Key)
            .ToDictionary(g => g.Key, g => g.Select(e => e.Value).ToList());

        bool hasDuplicates = grouped.Any(g => g.Value.Count > 1);

        if (hasDuplicates)
        {
            var averages = grouped
                .ToDictionary(g => g.Key, g => g.Value.Average());

            var top = averages
                .OrderByDescending(e => e.Value)
                .First();

            return top;
        }
        else
        {
            var top = emotionsScores
                .OrderByDescending(e => e.Value)
                .First();

            return top;
        }
    }
}
