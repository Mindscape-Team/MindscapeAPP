using System;
using UnityEngine;

public class UI_TokenCheckHandler : MonoBehaviour
{
    [SerializeField] private GameObject introScreensGroup;

    [SerializeField] private GameObject homeScreen;
    [SerializeField] private GameObject navbar;

    void Start()
    {
        if (PlayerPrefs.HasKey("Token"))
        {
            string expireOnStr = PlayerPrefs.GetString("Expires On");
            DateTime expireOn = DateTime.Parse(expireOnStr);

            if (DateTime.UtcNow < expireOn)
            {
                Invoke("HomeScreenUI", 1.75f);
            }
            else
            {
                PlayerPrefs.DeleteAll();
                Invoke("BoardingScreensUI", 1.75f);
            }
        }
    }

    void HomeScreenUI()
    {
        introScreensGroup.SetActive(false);
        homeScreen.SetActive(true);
        navbar.SetActive(true);
    }
}
