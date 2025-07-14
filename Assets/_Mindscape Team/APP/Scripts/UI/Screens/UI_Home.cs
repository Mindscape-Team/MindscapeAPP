using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Home : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI welcomeHeaderTxt;
    [SerializeField] private TextMeshProUGUI flowHeaderTxt;

    [SerializeField] private Image profileImage;

    [SerializeField] private GameObject navbar;
    [SerializeField] private UI_Navbar navbarUI;

    private void OnEnable()
    {
        UpdateUI();
    }

    //private void OnEnable()
    //{
    //    StartCoroutine(FetchUserProfileAndUpdateUI());
    //}

    //private IEnumerator FetchUserProfileAndUpdateUI()
    //{
    //    yield return StartCoroutine(BE_HelperMethods.FetchUserProfile());
    //    UpdateUI();
    //}

    private void UpdateUI()
    {
        string fullName = PlayerPrefs.GetString("User Full Name");

        welcomeHeaderTxt.text = $"Hello, {fullName}";
        flowHeaderTxt.text = $"{fullName}'s Flow";

        string profilePicture = PlayerPrefs.GetString("User Profile Picture");

        if (!string.IsNullOrEmpty(profilePicture))
        {
            byte[] image = Convert.FromBase64String(profilePicture);

            Sprite sprite = UI_HelperMethods.ImageLoader(image);

            profileImage.sprite = sprite;
            profileImage.preserveAspect = true;
        }

        navbarUI.ChangeGUI(0); 
        navbar.SetActive(true);
    }
}
