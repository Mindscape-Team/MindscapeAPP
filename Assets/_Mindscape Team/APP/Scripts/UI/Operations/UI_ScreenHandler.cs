using UnityEngine;

public class UI_ScreenHandler : MonoBehaviour
{
    //[SerializeField] private GameObject oldScreensGroup;
    //[SerializeField] private GameObject newScreensGroup;
    [SerializeField] private GameObject[] screens;

    public void ShowScreen(int screenIndex)
    {
        for (int i = 0; i < screens.Length; i++)
        {
            screens[i].SetActive(i == screenIndex);

            UI_HelperMethods.simpleAnimation(screens[i], 0.5f, 0.1f);
        }
    }

    public void ClosePopUp(int screenIndex)
    {
        for (int i = 0; i < screens.Length; i++)
        {
            if (i == screenIndex)
            {
                screens[i].SetActive(false);
            }
        }
    }

    //public void SwitchScreensGroup(int screenIndex)
    //{
    //    newScreensGroup.SetActive(true);
    //    oldScreensGroup.SetActive(false);

    //    ShowScreen(screenIndex);
    //}
}
