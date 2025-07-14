using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Survey : MonoBehaviour
{
    [SerializeField] private GameObject questionsPanel;
    [SerializeField] private GameObject questionFrame;
    [SerializeField] private TextMeshProUGUI questionTxt;
    [SerializeField] private TextMeshProUGUI stepTxt;

    [SerializeField] private Button nextBtn;
    [SerializeField] private TextMeshProUGUI nextBtnTxt;
    [SerializeField] private Button backBtn;

    [SerializeField] private ToggleGroup toggleGroup;

    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI resultTxt;
    [SerializeField] private TextMeshProUGUI totalTxt;

    [SerializeField] private TextMeshProUGUI headerTxt;
    [SerializeField] private TextMeshProUGUI paragraphTxt;

    private int surveyType = 0;

    private int currentQuestionIndex;
    private int[] selectedScores;
    private Toggle[] toggles;

    private string[] anxietyQuestions = new string[]
    {
        "Do you feel tense or anxious?",
        "Do you get easily fatigued?",
        "Do you fear darkness or crowds?",
        "Do you have trouble sleeping?",
        "Do you have memory issues?",
        "Do you feel emotionally numb?",
        "Do you fidget or shake during conversations?",
        "Do you feel physical sensations like dizziness or hot flashes?",
        "Do your muscles feel tense or sore?",
        "Do you feel chest pain or palpitations?",
        "Do you feel breathless or suffocated?",
        "Do you have stomach or digestion issues?",
        "Do you have urinary or sexual problems?",
        "Do you experience headaches or sweating?"
    };

    private string[] depressionQuestions = new string[]
    {
        "Do you feel sadness, depression, or pessimism about the future?",
        "Do you feel guilt or blame yourself for your condition or actions?",
        "Do you have thoughts that life isn’t worth living?",
        "Do you have difficulty performing tasks or lost interest in activities?",
        "Do you feel slowed down in thinking, speaking, or moving?",
        "Do you feel tense, easily startled, or expect something bad to happen?",
        "Do you experience physical symptoms such as palpitations, headaches, or indigestion?",
        "Are you excessively worried about your health or convinced you're ill?",
        "Do you have trouble falling asleep at the beginning of the night?",
        "Do you wake up frequently in the middle of the night?",
        "Do you wake up earlier than usual and can’t go back to sleep?",
        "Do you experience physical restlessness such as hand wringing or pacing?",
        "Do you have symptoms like constipation, loss of appetite, or a heavy stomach?",
        "Do you feel fatigue, generalized pain, or heaviness in limbs, back, or head?",
        "Do you have reduced sexual interest or menstrual disturbances?",
        "Are you aware that your condition is psychological and not purely physical?",
        "Have you experienced any noticeable weight loss recently?"
    };

    //private void OnEnable()
    //{
    //    surveyType = 0;
    //    currentQuestionIndex = 0;
    //    selectedScores = new int[0];
    //    toggles = new Toggle[0];
    //}

    public void SetSurveyTypeToDepression(int st)
    {
        selectedScores = new int[depressionQuestions.Length];
        surveyType = st;

        headerTxt.text = "Depression Survey";
        paragraphTxt.text = "Answer the following questions to help us understand your depression levels";

        SetUpQuestionAndOptions();
    }

    public void SetSurveyTypeToAnxiety(int st)
    {
        selectedScores = new int[anxietyQuestions.Length];
        surveyType = st;

        headerTxt.text = "Anxiety Survey";
        paragraphTxt.text = "Answer the following questions to help us understand your anxiety levels";

        SetUpQuestionAndOptions();
    }

    public void SetUpQuestionAndOptions()
    {
        for (int i = 0; i < selectedScores.Length; i++)
        {
            selectedScores[i] = -1;
        }

        toggles = toggleGroup.GetComponentsInChildren<Toggle>();

        for (int i = 0; i < toggles.Length; i++)
        {
            int index = i;
            toggles[i].onValueChanged.AddListener((isOn) => OnToggleChanged(isOn, index));
        }

        ShowQuestion();
    }

    public void Animation(GameObject gameObject)
    {
        Vector2 originalPosition = gameObject.transform.localPosition;

        gameObject.transform.localPosition = originalPosition + new Vector2(-Screen.width, 0);

        gameObject.LeanMoveLocalX(0, 0.5f).setEaseOutExpo().setDelay(0.1f);
    }

    public void ShowQuestion()
    {
        Animation(stepTxt.gameObject);
        Animation(questionFrame);

        if (currentQuestionIndex == 0)
        {
            backBtn.interactable = false;
        }
        else
        {
            backBtn.interactable = true;
        }

        if (surveyType == 1)
        {
            questionTxt.text = depressionQuestions[currentQuestionIndex];
            stepTxt.text = $"Step {currentQuestionIndex + 1} of {depressionQuestions.Length}";
        }
        else if (surveyType == 2)
        {
            questionTxt.text = anxietyQuestions[currentQuestionIndex];
            stepTxt.text = $"Step {currentQuestionIndex + 1} of {anxietyQuestions.Length}";
        }

        if (currentQuestionIndex != 0)
        {
            toggleGroup.allowSwitchOff = true;
            toggleGroup.SetAllTogglesOff();
            toggleGroup.allowSwitchOff = false;
        }

        int savedScore = selectedScores[currentQuestionIndex];

        if (savedScore >= 0 && savedScore < toggles.Length)
        {
            toggles[savedScore].isOn = true;
            nextBtn.interactable = true;
        }
        else
        {
            nextBtn.interactable = false;
        }

        if (surveyType == 1)
        {
            nextBtnTxt.text = (currentQuestionIndex == depressionQuestions.Length - 1) ? "Finish" : "Next";
        }
        else if (surveyType == 2)
        {
            nextBtnTxt.text = (currentQuestionIndex == anxietyQuestions.Length - 1) ? "Finish" : "Next";
        }
    }

    public void OnToggleChanged(bool isOn, int score)
    {
        if (isOn)
            OnOptionChosen(score);
    }

    public void OnOptionChosen(int score)
    {
        selectedScores[currentQuestionIndex] = score;
        nextBtn.interactable = true;
        Debug.Log($"Selected score {score} for question {currentQuestionIndex + 1}");
    }

    public void Next()
    {
        if (surveyType == 1)
        {
            if (currentQuestionIndex == depressionQuestions.Length - 1)
            {
                string submitUrl = "https://localhost:44314/api/Survey/SubmitDepressionSurvey";
                SubmitSurvey(submitUrl);

                //surveyType = 0;
                currentQuestionIndex = 0;
                selectedScores = new int[0];
                toggles = new Toggle[0];

                return;
            }
        }
        else if (surveyType == 2)
        {
            if (currentQuestionIndex == anxietyQuestions.Length - 1)
            {
                string submitUrl = "https://localhost:44314/api/Survey/SubmitAnxietySurvey";
                SubmitSurvey(submitUrl);

                //surveyType = 0;
                currentQuestionIndex = 0;
                selectedScores = new int[0];
                toggles = new Toggle[0];

                return;
            }
        }

        currentQuestionIndex++;
        ShowQuestion();
    }

    public void Back()
    {
        if (currentQuestionIndex > 0)
        {
            currentQuestionIndex--;
            ShowQuestion();
        }
    }

    public void SubmitSurvey(string url)
    {
        for (int i = 0; i < selectedScores.Length; i++)
        {
            if (selectedScores[i] == -1)
            {
                resultTxt.text = $"Please answer question {i + 1} before submitting.";
                //resultPanel.SetActive(true);
                return;
            }
        }

        List<SurveyAnswer> answers = new List<SurveyAnswer>();
        for (int i = 0; i < selectedScores.Length; i++)
        {
            answers.Add(new SurveyAnswer
            {
                QuestionNumber = i + 1,
                Score = selectedScores[i]
            });
        }

        StartCoroutine(BE_Survey.SendSurvey(url, answers, (result) =>
        {
            if (result != null)
            {
                if (surveyType == 1)
                    PlayerPrefs.SetString("User Depression Survey", "1");
                else if (surveyType == 2)
                    PlayerPrefs.SetString("User Anxiety Survey", "1");

                ShowResult(result.totalScore, result.severityLevel);
            }
            else
            {
                resultTxt.text = "Submission failed.";
                //resultPanel.SetActive(true);
            }
        }));

        questionsPanel.SetActive(false);
    }


    public void ShowResult(int totalScore, string level)
    {
        resultPanel.SetActive(true);
        Animation(resultPanel);

        if (surveyType == 1)
            totalTxt.text = "/68";
        else if (surveyType == 2)
            totalTxt.text = "/56";

        resultTxt.text = $"{totalScore}";
    }
}
