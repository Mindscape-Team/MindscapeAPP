using UnityEngine;
using UnityEngine.UI;

public class UI_LoadingScreenAnimation : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private float delayTime = 1.0f;

    void Start()
    {
        fillImage.fillAmount = 0f;

        LeanTween.value(gameObject, 0f, 1f, delayTime).setOnUpdate((float val) =>
        {
            fillImage.fillAmount = val;
        }).setEase(LeanTweenType.easeInOutQuad).setLoopClamp();
    }
}
