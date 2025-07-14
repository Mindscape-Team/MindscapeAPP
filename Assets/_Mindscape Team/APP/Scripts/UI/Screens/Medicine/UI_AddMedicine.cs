using TMPro;
using UnityEngine;

public class UI_AddMedicine : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameTxtField;
    [SerializeField] private TMP_InputField dosageFreqTxtField;
    [SerializeField] private TMP_InputField dosageTxtField;
    [SerializeField] private TMP_InputField timeTxtField;

    [SerializeField] private GameObject medicineCardPrefab;
    [SerializeField] private GameObject addScreen;
    [SerializeField] private GameObject reminderScreen;

    private GameObject medicineScrollView;
    private string selectedDate = "";


    public void SetSelectedDate(string date) => selectedDate = date;
    public void SetParent(GameObject p) => medicineScrollView = p;

    public void OnAddMedicine()
    {
        if (string.IsNullOrEmpty(selectedDate) || medicineScrollView == null)
        {
            Debug.LogError("No date selected – click a day first (auto‑selected at launch).");
            return;
        }

        if (string.IsNullOrEmpty(nameTxtField.text) ||
            string.IsNullOrEmpty(dosageFreqTxtField.text) ||
            string.IsNullOrEmpty(dosageTxtField.text) ||
            string.IsNullOrEmpty(timeTxtField.text))
        {
            Debug.LogError("All fields must be filled.");
            return;
        }

        Medicine med = new Medicine
        {
            name = nameTxtField.text,
            dosageFrequency = dosageFreqTxtField.text,
            dosage = dosageTxtField.text,
            medicineTime = timeTxtField.text,
            date = selectedDate
        };

        StartCoroutine(BE_Medicine.AddMedicine(med, HandleAddMedicineResponse));
    }

    private void HandleAddMedicineResponse(bool success, string response, Medicine med)
    {
        if (!success)
        {
            Debug.LogError($"Failed to add medicine. Response: {response}");
            return;
        }

        nameTxtField.text = "";
        dosageFreqTxtField.text = "";
        dosageTxtField.text = "";
        timeTxtField.text = "";

        AddMedicineToUI(med);

        addScreen.SetActive(false);
        reminderScreen.SetActive(true);
    }

    private void AddMedicineToUI(Medicine med)
    {
        if (medicineCardPrefab == null || medicineScrollView == null)
        {
            Debug.LogError("Prefab or scroll parent not assigned.");
            return;
        }

        if (medicineScrollView.transform.Find(med.id.ToString()) != null) return;

        GameObject card = Instantiate(medicineCardPrefab, medicineScrollView.transform);
        card.name = med.id.ToString();
        card.SetActive(true);

        TextMeshProUGUI[] t = card.GetComponentsInChildren<TextMeshProUGUI>();
        t[0].text = med.name;
        t[1].text = med.dosageFrequency;
        t[2].text = med.dosage;
        t[3].text = med.medicineTime;

        Debug.Log("Medicine entry added to UI.");
    }
}