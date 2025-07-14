using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_TogglePasswordModeHandler : MonoBehaviour
{
    [SerializeField] private Toggle visibleToggle;
    [SerializeField] private TMP_InputField passwordInputField;

    void Start()
    {
        visibleToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnToggleChanged(bool isOn)
    {
        if (isOn)
        {
            passwordInputField.contentType = TMP_InputField.ContentType.Password;
        }
        else
        {
            passwordInputField.contentType = TMP_InputField.ContentType.Standard;
        }

        passwordInputField.ForceLabelUpdate();  
    }
}
