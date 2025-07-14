using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class UI_DateHandler : MonoBehaviour
{
    [SerializeField] private UI_AddMedicine addMedicine;

    [Header("Day Elements")]
    private GameObject selectedDayBtn;
    [SerializeField] private GameObject dayBtnPrefab;
    [SerializeField] private GameObject dayScrollView;
    [SerializeField] private Sprite dayDefaultSprite;
    [SerializeField] private Sprite daySelectedSprite;

    [Header("Medicine Elements")]
    [SerializeField] private GameObject medicineScrollView;
    [SerializeField] private GameObject medicineCardPrefab;

    private void OnEnable() => InitializeDays();

    private void InitializeDays()
    {
        DateTime d = DateTime.Today;
        foreach (Transform child in medicineScrollView.transform)
        {
            child.gameObject.name = d.ToString("yyyy-MM-dd");
            d = d.AddDays(1);
        }

        for (int i = 0; i < 7; i++)
        {
            GameObject dayBtn = Instantiate(dayBtnPrefab, dayScrollView.transform, false);
            dayBtn.name = $"Day {i + 1}";

            TextMeshProUGUI[] txt = dayBtn.GetComponentsInChildren<TextMeshProUGUI>();
            txt[0].name = $"Day {i + 1} Txt";
            txt[1].name = $"Day Number {i + 1} Txt";

            DateTime day = DateTime.Today.AddDays(i);
            txt[0].text = day.ToString("ddd");
            txt[1].text = day.Day.ToString();

            int captured = i;
            dayBtn.GetComponent<Button>().onClick.AddListener(() => SelectButton(dayBtn, captured));

            Transform scrollView = medicineScrollView.transform.GetChild(captured);
            scrollView.gameObject.SetActive(i == 0);

            InitializeMedicine(scrollView);
        }

        SelectButton(dayScrollView.transform.GetChild(0).gameObject, 0);
    }

    private void SelectButton(GameObject btn, int index)
    {
        if (selectedDayBtn != null && selectedDayBtn != btn)
            SetButtonVisual(selectedDayBtn, false);

        selectedDayBtn = btn;
        SetButtonVisual(btn, true);

        for (int i = 0; i < medicineScrollView.transform.childCount; i++)
            medicineScrollView.transform.GetChild(i).gameObject.SetActive(i == index);

        DateTime date = DateTime.Today.AddDays(index);
        ScrollRect currentRect = medicineScrollView.transform.GetChild(index).GetComponent<ScrollRect>();

        addMedicine.SetSelectedDate(date.ToString("yyyy-MM-dd"));
        addMedicine.SetParent(currentRect.content.gameObject);
    }

    private void SetButtonVisual(GameObject btn, bool selected)
    {
        btn.GetComponent<Image>().sprite = selected ? daySelectedSprite : dayDefaultSprite;
        foreach (var t in btn.GetComponentsInChildren<TextMeshProUGUI>())
            t.color = selected ? Color.white : Color.black;
    }

    private void InitializeMedicine(Transform scrollViewObj)
    {
        ScrollRect rect = scrollViewObj.GetComponent<ScrollRect>();
        ClearContent(rect);

        StartCoroutine(BE_Medicine.GetMedicinesByDate(
            scrollViewObj.gameObject.name,
            (success, list, msg) =>
            {
                if (!success || list == null || list.Count == 0) return;

                ClearContent(rect);

                foreach (var med in list)
                {
                    if (CardExists(med.id.ToString(), rect.content)) continue;

                    GameObject card = Instantiate(medicineCardPrefab, rect.content);
                    card.name = med.id.ToString();
                    card.SetActive(true);

                    TextMeshProUGUI[] t = card.GetComponentsInChildren<TextMeshProUGUI>();
                    t[0].text = med.name;
                    t[1].text = med.dosageFrequency;
                    t[2].text = med.dosage;
                    t[3].text = med.medicineTime;
                }
            }));
    }

    private void ClearContent(ScrollRect rect)
    {
        foreach (Transform c in rect.content) Destroy(c.gameObject);
    }

    private bool CardExists(string id, Transform parent) => parent.Find(id) != null;
}