using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_SceneLoader : MonoBehaviour
{
    //[SerializeField] private GameObject application;
    //[SerializeField] private GameObject mainEnvironment;

    //public void LoadScreen()
    //{
    //    application.SetActive(false);
    //    mainEnvironment.SetActive(true);
    //}

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Image loadingSlider;

    public void LoadScene(string sceneName)
    {
        StartCoroutine(ShowLoadingAndLoadScene(sceneName));
    }

    IEnumerator ShowLoadingAndLoadScene(string sceneName)
    {
        loadingScreen.SetActive(true);

        yield return null;

        yield return StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        loadingOperation.allowSceneActivation = false;

        while (loadingOperation.progress < 0.9f)
        {
            float progressValue = Mathf.Clamp01(loadingOperation.progress / 0.9f);
            loadingSlider.fillAmount = progressValue;
            yield return null;
        }

        loadingSlider.fillAmount = 1f;
        yield return new WaitForSeconds(0.5f);

        gameObject.SetActive(false);
        loadingOperation.allowSceneActivation = true;
    }
}
