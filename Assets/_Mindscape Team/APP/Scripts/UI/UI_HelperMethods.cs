using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class UI_HelperMethods : MonoBehaviour 
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject screen;

    [SerializeField] private float waitingTime = 1f;

    public static void simpleAnimation(GameObject go, float animationTime, float delay)
    {
        go.transform.localPosition = new Vector2(-Screen.width, 0);
        go.LeanMoveLocalX(0, animationTime).setEaseOutExpo().delay = delay;
    }

    public void LoadingToScreen()
    {
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.transform.localPosition = new Vector2(-Screen.width, 0);
        loadingScreen.LeanMoveLocalX(0, 0.5f).setEaseOutExpo().delay = 0.1f;

        StartCoroutine(WaitAndHideLoading());
    }
    private IEnumerator WaitAndHideLoading()
    {
        yield return new WaitForSeconds(waitingTime);

        loadingScreen.SetActive(false);

        screen.SetActive(true);
        screen.transform.localPosition = new Vector2(-Screen.width, 0);
        screen.LeanMoveLocalX(0, 0.5f).setEaseOutExpo().delay = 0.1f;
    }

    public static Texture2D CropToCircle(Texture2D sourceTexture)
    {
        int size = Mathf.Min(sourceTexture.width, sourceTexture.height);
        Texture2D result = new Texture2D(size, size);
        int centerX = sourceTexture.width / 2;
        int centerY = sourceTexture.height / 2;
        int radius = size / 2;

        Color clear = new Color(0, 0, 0, 0);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - radius;
                float dy = y - radius;
                if (dx * dx + dy * dy <= radius * radius)
                {
                    result.SetPixel(x, y, sourceTexture.GetPixel(centerX - radius + x, centerY - radius + y));
                }
                else
                {
                    result.SetPixel(x, y, clear);
                }
            }
        }

        result.Apply();
        return result;
    }
    public static Sprite ImageLoader(byte[] img)
    {
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(img);

        Texture2D circularTexture = CropToCircle(texture);

        Sprite sprite = Sprite.Create(circularTexture, new Rect(0, 0, circularTexture.width, circularTexture.height), new Vector2(0.5f, 0.5f));

        return sprite;
    }
    public static byte[] ImageUnloader(Sprite sprite)
    {
        Texture2D texture = sprite.texture;
        RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height);

        Graphics.Blit(texture, renderTexture);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTexture;

        Texture2D readableTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        readableTexture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        readableTexture.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTexture);

        byte[] imageData = readableTexture.EncodeToPNG();

        return imageData;
    }
}
