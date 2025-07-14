using System.Collections.Generic;
using UnityEngine;

public class UI_Doctors : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject doctorPrefab;

    [SerializeField] private GameObject doctorBookingGroup;
    [SerializeField] private UI_Booking bookingUI;
    [SerializeField] private UI_DateTimeHandler dateTimeHandler;

    [SerializeField] private UI_Navbar navbarUI;

    private void OnEnable()
    {
        navbarUI.ChangeGUI(1);
        StartCoroutine(BE_Doctors.GetDoctors(HandleGetDoctorsResponse));
    }

    private void HandleGetDoctorsResponse(bool success, List<Doctor> doctors, string response)
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        if (success && doctors != null && doctors.Count > 0)
        {
            foreach (var doctor in doctors)
            {
                GameObject doctorCard = Instantiate(doctorPrefab, contentParent);
                UI_DoctorCard doctorCardUI = doctorCard.GetComponent<UI_DoctorCard>();
                doctorCardUI.Initialize(doctor, doctorBookingGroup, bookingUI, dateTimeHandler);
            }
        }
        else
        {
            Debug.LogError("Failed to load doctors: " + response);
        }
    }
}
