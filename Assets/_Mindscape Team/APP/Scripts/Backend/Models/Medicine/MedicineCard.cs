using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MedicineCard : MonoBehaviour
{
    [SerializeField] private GameObject reminderScreen;
    [SerializeField] private GameObject deletePopUp;

    [SerializeField] private Image oldBackground;
    [SerializeField] private Sprite newBackground;

    [SerializeField] private Button doneBtn;
    [SerializeField] private Button deleteBtn;


    private void Start()
    {
        reminderScreen = GameObject.Find("Reminder");
        deletePopUp = GameObject.Find("Delete Medicine");
    }

    public void DoneMedicine()
    {
        oldBackground.sprite = newBackground;

        doneBtn.interactable = false;
        deleteBtn.interactable = false;

        StartCoroutine(BE_Medicine.DeleteMedicine(int.Parse(gameObject.name), (success, message) =>
        {
            if (success)
            {
                foreach (Transform child in deletePopUp.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }));
    }

    public void DeleteMedicine()
    {
        //reminderScreen.gameObject.SetActive(false);

        foreach (Transform child in deletePopUp.transform)
        {
            child.gameObject.SetActive(true);
        }

        Button[] buttons = deletePopUp.GetComponentsInChildren<Button>();
        buttons[0].onClick.AddListener(() =>
        {
            StartCoroutine(BE_Medicine.DeleteMedicine(int.Parse(gameObject.name), (success, message) =>
            {
                if (success)
                {
                    foreach (Transform child in deletePopUp.transform)
                    {
                        child.gameObject.SetActive(false);
                    }

                    Destroy(gameObject);
                }
            }));
        });
    }

    
}
