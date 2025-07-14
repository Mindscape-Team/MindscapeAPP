using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Notifications : MonoBehaviour
{
    public static UI_Notifications Instance { get; private set; }

    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private Transform notificationListContent;
    [SerializeField] private GameObject notificationItemPrefab;

    private readonly List<GameObject> notificationItems = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (notificationPanel != null)
            notificationPanel.SetActive(false);
    }

    public void AddNotification(string title, string message)
    {
        if (notificationItemPrefab == null || notificationListContent == null)
        {
            Debug.LogWarning("Notification prefab or content root is missing");
            return;
        }

        GameObject newItem = Instantiate(notificationItemPrefab, notificationListContent);
        newItem.transform.SetAsFirstSibling();

        TextMeshProUGUI[] texts = newItem.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length >= 2)
        {
            texts[0].text = title;
            texts[1].text = message;
        }
        else
        {
            Debug.LogWarning("Could not find both title and message TMP components.");
        }

        notificationItems.Add(newItem);

    }

    public void ShowNotificationPanel()
    {
        if (notificationPanel != null)
            notificationPanel.SetActive(true);
    }

    public void HideNotificationPanel()
    {
        if (notificationPanel != null)
            notificationPanel.SetActive(false);
    }
}
