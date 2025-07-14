using UnityEngine;

public class UI_SwipeAnimation : MonoBehaviour
{
    private GameObject animatedObject;
    [SerializeField] private int animationLeftDirection = 0;
    [SerializeField] private int animationUpDirection = 0;

    [SerializeField] private float animationTime = 0.5f;
    [SerializeField] private float delayTime = 0.1f;

    private Vector2 originalPosition;

    private void Awake()
    {
        animatedObject = this.gameObject;
    }

    void Start()
    {
        originalPosition = animatedObject.transform.localPosition;

        int leftDirection = animationLeftDirection * Screen.width;
        int upDirection = animationUpDirection * Screen.height;

        animatedObject.transform.localPosition = originalPosition + new Vector2(leftDirection, upDirection);

        if (leftDirection != 0 && upDirection == 0)
        {
            animatedObject.LeanMoveLocalX(0, animationTime).setEaseOutExpo().setDelay(delayTime);
        }
        else if (leftDirection == 0 && upDirection != 0)
        {
            animatedObject.LeanMoveLocalY(originalPosition.y, animationTime).setEaseOutExpo().setDelay(delayTime);
        }
        else if (leftDirection != 0 && upDirection != 0)
        {
            animatedObject.LeanMoveLocal(originalPosition, animationTime).setEaseOutExpo().setDelay(delayTime);
        }
    }
}
