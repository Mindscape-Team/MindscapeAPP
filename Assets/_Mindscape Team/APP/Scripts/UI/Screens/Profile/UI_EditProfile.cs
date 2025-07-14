using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_EditProfile : MonoBehaviour
{
    [SerializeField] private TMP_InputField[] inputFields;

    [SerializeField] private TextMeshProUGUI[] placeholderFields;

    [SerializeField] private Button saveChangesBtn;

    [SerializeField] private GameObject changesSavedPopUpScreen;

    private void OnEnable()
    {
        StartCoroutine(FetchUserProfileAndUpdateUI());
        saveChangesBtn.onClick.AddListener(ChangesSavedBE_UI);
    }

    private void ChangesSavedBE_UI()
    {
        BE_Profile.UpdateProfile(inputFields);

        changesSavedPopUpScreen.gameObject.SetActive(true);
        Invoke("ChangesSavedUI", 1f);
    }

    private void ChangesSavedUI()
    {
        changesSavedPopUpScreen.gameObject.SetActive(false);
    }

    private IEnumerator FetchUserProfileAndUpdateUI()
    {
        yield return StartCoroutine(BE_Profile.FetchUserProfile());
        UpdateUI();
    }

    private void UpdateUI()
    {
        placeholderFields[0].text = PlayerPrefs.GetString("Full Name");
        placeholderFields[1].text = PlayerPrefs.GetString("Phone Number");
        placeholderFields[2].text = PlayerPrefs.GetString("Address");
    }
}
