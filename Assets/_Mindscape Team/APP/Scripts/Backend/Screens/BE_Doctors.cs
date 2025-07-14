using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public static class BE_Doctors
{
    public static IEnumerator GetDoctors(Action<bool, List<Doctor>, string> callback)
    {
        string doctorsUrl = "https://localhost:44314/api/Doctor";

        using (UnityWebRequest request = UnityWebRequest.Get(doctorsUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("API Response: " + request.downloadHandler.text);

                try
                {
                    var doctors = BE_HelperMethods.FromJson<Doctor>(request.downloadHandler.text);
                    callback(true, doctors, request.downloadHandler.text);
                }
                catch (Exception e)
                {
                    Debug.LogError("JSON Parsing Error: " + e.Message);
                    callback(false, null, "JSON Parsing Error");
                }
            }
            else
            {
                callback(false, null, request.error);
            }
        }
    }

    public static IEnumerator AddAppointment(Appointment appointment, System.Action<bool, string, Appointment> callback)
    {
        string addAppointmentUrl = "https://localhost:44314/api/Appointment";
        string token = PlayerPrefs.GetString("Token");

        if (string.IsNullOrEmpty(token))
        {
            Debug.LogError("No token found. User needs to log in.");
            callback?.Invoke(false, "No token found.", null);
            yield break;
        }
        DateTime parsedDate;
        if (DateTime.TryParse(appointment.date, out parsedDate))
        {
            appointment.date = parsedDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }
        else
        {
            Debug.LogError("Invalid date format. Please check the date value.");
            callback?.Invoke(false, "Invalid date format.", null);
            yield break;
        }

        string jsonData = JsonUtility.ToJson(appointment);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(addAppointmentUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + token);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Appointment added successfully!");
                callback?.Invoke(true, request.downloadHandler.text, appointment);
            }
            else
            {
                Debug.LogError($"Failed to add appointment. Error: {request.error} (HTTP {request.responseCode})");
                callback?.Invoke(false, request.error, null);
            }
        }
    }

    public static IEnumerator GetAppointmentsByDoctorId(int doctorId, Action<bool, List<Appointment>, string> callback)
    {
        string doctorAppointmentsUrl = $"https://localhost:44314/api/Appointment/Doctor/{doctorId}";

        using (UnityWebRequest request = UnityWebRequest.Get(doctorAppointmentsUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("API Response: " + request.downloadHandler.text);

                try
                {
                    var appointments = BE_HelperMethods.FromJson<Appointment>(request.downloadHandler.text);
                    callback(true, appointments, request.downloadHandler.text);
                }
                catch (Exception e)
                {
                    Debug.LogError("JSON Parsing Error: " + e.Message);
                    callback(false, null, "JSON Parsing Error");
                }
            }
            else
            {
                callback(false, null, request.error);
            }
        }
    }
}
