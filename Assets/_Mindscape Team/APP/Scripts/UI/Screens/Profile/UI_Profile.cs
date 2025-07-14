using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Profile : MonoBehaviour
{
    [SerializeField] private Image profileImage;
    [SerializeField] private Sprite defaultSprite;

    [SerializeField] private TextMeshProUGUI fullNameTxt;

    [SerializeField] private UI_Navbar navbarUI;

    private void OnEnable()
    {
        navbarUI.ChangeGUI(3);
        StartCoroutine(FetchUserProfileAndUpdateUI());
    }

    private IEnumerator FetchUserProfileAndUpdateUI()
    {
        yield return StartCoroutine(BE_Profile.FetchUserProfile());
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (PlayerPrefs.HasKey("Profile Picture"))
        {
            if (PlayerPrefs.GetString("Profile Picture") != "")
            {
                string profilePicture = PlayerPrefs.GetString("Profile Picture");
                byte[] Image = Convert.FromBase64String(profilePicture);

                profileImage.sprite = UI_HelperMethods.ImageLoader(Image);
            }
        }
        else
        {
            profileImage.sprite = defaultSprite;
        }

        fullNameTxt.text = PlayerPrefs.GetString("Full Name");
    }
}
