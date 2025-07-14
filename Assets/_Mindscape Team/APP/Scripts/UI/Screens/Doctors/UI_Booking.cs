using TMPro;
using UnityEngine;

public class UI_Booking : MonoBehaviour
{
    [SerializeField] private TMP_InputField userNameTxtField;
    [SerializeField] private TMP_InputField phoneNumberTxtField;
    [SerializeField] private GameObject dayScrollView;
    [SerializeField] private GameObject timeScrollView;

    [SerializeField] private GameObject payBtn;

    private string selectedDate = "";
    private string selectedTime = "";
    private string selectedPackageType = "";
    private int selectedDoctorId = -1;

    public void SetSelectedDate(string date)
    {
        selectedDate = date;
        //Debug.Log("Date set in AppointmentUI: " + selectedDate);
    }

    public void SetSelectedTime(string time)
    {
        selectedTime = time;
        //Debug.Log("Time set in AppointmentUI: " + selectedTime);
    }

    public void SetPackageType(string packageType)
    {
        selectedPackageType = packageType;
        //Debug.Log("Package Type: " + selectedPackageType);
    }

    public void SetDoctorId(int doctorId)
    {
        selectedDoctorId = doctorId;
        //Debug.Log("Doctor ID set: " + selectedDoctorId);
    }

    public void OnAddAppointment()
    {
        if (string.IsNullOrEmpty(selectedDate))
        {
            Debug.LogError("No date selected.");
            return;
        }

        if (string.IsNullOrEmpty(selectedTime))
        {
            Debug.LogError("No time selected.");
            return;
        }

        if (selectedDoctorId == -1)
        {
            Debug.LogError("No doctor selected.");
            return;
        }

        if (string.IsNullOrEmpty(userNameTxtField.text) || string.IsNullOrEmpty(phoneNumberTxtField.text))
        {
            Debug.LogError("All fields must be filled.");
            return;
        }

        Debug.Log("Sending appointment data for date: " + selectedDate);

        Appointment appointment = new Appointment
        {
            userName = userNameTxtField.text,
            phoneNumber = phoneNumberTxtField.text,
            time = selectedTime,
            packageType = selectedPackageType,
            date = selectedDate,
            doctorId = selectedDoctorId
        };

        StartCoroutine(BE_Doctors.AddAppointment(appointment, HandleAddAppointmentResponse));
    }

    private void HandleAddAppointmentResponse(bool success, string response, Appointment appointment)
    {
        if (success)
        {
            Debug.Log("Appointment successfully added to backend!");
        }
        else
        {
            Debug.LogError($"Failed to add appointment. Response: {response}");
        }
    }
}
