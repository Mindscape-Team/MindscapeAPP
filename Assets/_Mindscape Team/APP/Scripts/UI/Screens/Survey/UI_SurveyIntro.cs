using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SurveyIntro : MonoBehaviour
{
    [SerializeField] private Button depressionSurveyBtn;
    [SerializeField] private Button anxietySurveyBtn;

    [SerializeField] private GameObject caseOneGroup;
    [SerializeField] private GameObject caseTwoGroup;

    private void OnEnable()
    {
        string depressionSurvey = PlayerPrefs.GetString("User Depression Survey");
        string anxietySurvey = PlayerPrefs.GetString("User Anxiety Survey");

        if (depressionSurvey == "1")
        {
            depressionSurveyBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Already Done";
            depressionSurveyBtn.interactable = false;
        }

        if (anxietySurvey == "1")
        {
            anxietySurveyBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Already Done";
            anxietySurveyBtn.interactable = false;
        }

        if (depressionSurvey == "1" && anxietySurvey == "1")
        {
            caseOneGroup.SetActive(false);
            caseTwoGroup.SetActive(true);
        }
    }
}
