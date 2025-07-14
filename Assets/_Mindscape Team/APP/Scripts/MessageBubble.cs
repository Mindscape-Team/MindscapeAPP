using Newtonsoft.Json;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using static Unity.Burst.Intrinsics.X86.Avx;

public class MessageBubble : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform msgBackground;          
    [SerializeField] private TextMeshProUGUI msgTxt;       

    public void SetMessage(string message)
    {
        if (msgTxt == null || msgBackground == null)
        {
            Debug.LogError("MessageBubble: Missing references.");
            return;
        }

        msgTxt.text = message;

        Canvas.ForceUpdateCanvases();

        Vector2 textSize = msgTxt.GetPreferredValues(message, 600f, Mathf.Infinity);

        RectTransform rt = msgTxt.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(textSize.x, textSize.y);

        msgBackground.sizeDelta = new Vector2(textSize.x + 40f, textSize.y + 40f);
    }
}
