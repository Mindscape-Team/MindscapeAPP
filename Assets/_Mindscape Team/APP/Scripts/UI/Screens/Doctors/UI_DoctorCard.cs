using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_DoctorCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI specializationTxt;
    [SerializeField] private TextMeshProUGUI ratingTxt;
    [SerializeField] private TextMeshProUGUI locationTxt;
    [SerializeField] private TextMeshProUGUI timeTxt;
    [SerializeField] private TextMeshProUGUI priceTxt;

    [SerializeField] private Button bookBtn;

    private GameObject doctorBookingGroup;
    private UI_Booking bookingUI;
    private UI_DateTimeHandler dateTimeHandler;

    private int doctorId;

    public void Initialize(Doctor doctor, GameObject group, UI_Booking ui, UI_DateTimeHandler handler)
    {
        doctorBookingGroup = group;
        bookingUI = ui;
        dateTimeHandler = handler;

        doctorId = doctor.id;

        nameTxt.text = doctor.name;
        specializationTxt.text = doctor.specialization;
        ratingTxt.text = doctor.rating.ToString("0.0");
        locationTxt.text = doctor.location;
        timeTxt.text = doctor.availableTime;
        priceTxt.text = $"${doctor.price:F2}";

        bookBtn.onClick.AddListener(OnSelectDoctor);
    }

    private void OnSelectDoctor()
    {
        if (doctorBookingGroup != null)
        {
            doctorBookingGroup.SetActive(true);

            UI_HelperMethods.simpleAnimation(doctorBookingGroup, 0.5f, 0.1f);
        }

        if (bookingUI != null)
            dateTimeHandler.SetDoctorId(doctorId);

        //if (bookingUI != null)
        //{
        //    //UI_DateTimeHandler.Instance.SeedDoctorAppointments(doctorId);
        //}

        //if (dateTimeHandler != null)
        //{
        //    dateTimeHandler.SeedDoctorAppointments(doctorId);
        //}
    }
}
