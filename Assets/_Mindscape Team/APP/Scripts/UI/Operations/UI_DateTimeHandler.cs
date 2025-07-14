using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_DateTimeHandler : MonoBehaviour
{
    [Header("Day Elements")]
    private GameObject selectedDayBtn;

    [SerializeField] private GameObject dayBtnPrefab;
    [SerializeField] private GameObject dayScrollView;

    [SerializeField] private Sprite dayDefaultSprite;
    [SerializeField] private Sprite daySelectedSprite;

    [Header("Time Elements")]
    private GameObject selectedTimeBtn;

    [SerializeField] private GameObject timeBtnPrefab;
    [SerializeField] private GameObject timeScrollView;

    [SerializeField] private Sprite timeDefaultSprite;
    [SerializeField] private Sprite timeSelectedSprite;

    private string[] timeSlots = {
        "10:00 AM", "11:00 AM", "12:00 PM",
        "1:00 PM", "2:00 PM", "3:00 PM",
        "4:00 PM", "5:00 PM", "6:00 PM",
        "7:00 PM", "8:00 PM"
    };

    private int doctorId = 0;
    private string packageType = "";
    private Dictionary<string, List<string>> appoinmentsDict;
    private Dictionary<string, Button> timeSlotButtons = new Dictionary<string, Button>();

    public void SetDoctorId(int id)
    {
        doctorId = id;
    }

    public void SetPackageType(string pt)
    {
        packageType = pt;
    }

    private void OnEnable()
    {
        StartCoroutine(BE_Doctors.GetAppointmentsByDoctorId(doctorId, HandleAppointmentsResponse));
    }

    private void HandleAppointmentsResponse(bool success, List<Appointment> appointments, string response)
    {
        if (success && appointments != null && appointments.Count > 0)
        {
            var doctorAppointments = appointments
                                    .Where(a => a.doctorId == doctorId)
                                    .ToList();

            appoinmentsDict = doctorAppointments
                                  .GroupBy(a => a.date.Substring(0, 10)) 
                                  .ToDictionary(
                                        g => g.Key, 
                                        g => g.Select(a => a.time).ToList() 
                                   );

            InitializeDays();
        }
        else
        {
            InitializeDays();
        }
    }

    private void InitializeDays()
    {
        DateTime today = DateTime.Today;

        for (int i = 0; i < 7; i++)
        {
            GameObject dayBtn = Instantiate(dayBtnPrefab, dayScrollView.transform);
            dayBtn.name = "Day " + (i + 1);

            TextMeshProUGUI[] btnTxts = dayBtn.GetComponentsInChildren<TextMeshProUGUI>();
            btnTxts[0].name = "Day " + (i + 1) + " Txt";
            btnTxts[1].name = "Day Number " + (i + 1) + " Txt";

            DateTime date = today.AddDays(i);

            btnTxts[0].text = date.ToString("ddd");
            btnTxts[1].text = date.Day.ToString();

            if (date.DayOfWeek == DayOfWeek.Friday)
                dayBtn.GetComponent<Button>().interactable = false;

            int index = i;
            dayBtn.GetComponent<Button>().onClick.AddListener(() => SelectButton(dayBtn));
            dayBtn.GetComponent<Button>().onClick.AddListener(() => InitializeTime(index, date.ToString("yyyy-MM-dd")));
        }
    }

    private void InitializeTime(int dayIndex, string day)
    {
        if (timeScrollView.transform.childCount > 0)
        {
            foreach (Transform child in timeScrollView.transform)
            {
                Destroy(child.gameObject);
            }
        }

        for (int i = 0; i < timeSlots.Length; i++)
        {
            GameObject timeBtn = Instantiate(timeBtnPrefab, timeScrollView.transform);
            timeBtn.name = "Time " + (i + 1);

            TextMeshProUGUI btnTxt = timeBtn.GetComponentInChildren<TextMeshProUGUI>();
            btnTxt.name = "Time " + (i + 1) + " Txt";

            btnTxt.text = timeSlots[i];

            int index = i;
            timeBtn.GetComponent<Button>().onClick.AddListener(() => SelectButton(timeBtn, dayIndex, index));

            timeSlotButtons[timeSlots[i]] = timeBtn.GetComponent<Button>();
        }

        if (appoinmentsDict != null)
        {
            if (appoinmentsDict.ContainsKey(day))
            {
                var times = appoinmentsDict[day];

                foreach (var time in times)
                {
                    if (timeSlotButtons.ContainsKey(time))
                    {
                        timeSlotButtons[time].GetComponent<Image>().sprite = timeSelectedSprite;
                        timeSlotButtons[time].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                        timeSlotButtons[time].interactable = false;
                    }
                }
            }
        }
    }

    private void SelectButton(GameObject btn, int dayIndex = -1, int timeIndex = -1)
    {
        TextMeshProUGUI[] txts = btn.GetComponentsInChildren<TextMeshProUGUI>();

        if (txts.Length > 1)
        {
            if (selectedDayBtn != null && selectedDayBtn != btn)
            {
                selectedDayBtn.GetComponent<Image>().sprite = dayDefaultSprite;
                var prevTxts = selectedDayBtn.GetComponentsInChildren<TextMeshProUGUI>();
                prevTxts[0].color = Color.black;
                prevTxts[1].color = Color.black;
            }

            selectedDayBtn = btn;
            btn.GetComponent<Image>().sprite = daySelectedSprite;
            txts[0].color = Color.white;
            txts[1].color = Color.white;
        }
        else
        {
            if (selectedTimeBtn != null && selectedTimeBtn != btn)
            {
                selectedTimeBtn.GetComponent<Image>().sprite = timeDefaultSprite;
                var prevTxt = selectedTimeBtn.GetComponentInChildren<TextMeshProUGUI>();
                prevTxt.color = Color.black;
            }

            selectedTimeBtn = btn;
            btn.GetComponent<Image>().sprite = timeSelectedSprite;
            txts[0].color = Color.white;
        }

        if (dayIndex != -1 && timeIndex != -1)
        {
            AddAppointment(dayIndex, timeIndex);
        }
    }

    private void AddAppointment(int dayIndex, int timeIndex)
    {
        DateTime selectedDay = DateTime.Today.AddDays(dayIndex);
        string formattedDate = selectedDay.ToString("yyyy-MM-dd");

        string selectedTime = timeSlots[timeIndex];

        UI_Booking addAppointment = FindObjectOfType<UI_Booking>();

        if (addAppointment != null)
        {
            addAppointment.SetDoctorId(doctorId);
            addAppointment.SetPackageType(packageType);
            addAppointment.SetSelectedDate(formattedDate);
            addAppointment.SetSelectedTime(selectedTime);

            Debug.Log("Doctor Id: " + doctorId);
            Debug.Log("Package Type: " + packageType);
            Debug.Log("Date selected: " + formattedDate);
            Debug.Log("Time selected: " + selectedTime);
        }
        else
        {
            Debug.LogError("UI_Booking not found in the scene.");
        }
    }
}