using System.Text;
using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class BE_Medicine
{
    public static IEnumerator GetMedicinesByDate(string date, Action<bool, List<Medicine>, string> callback)
    {
        string getMedicinesUrl = $"https://localhost:44314/api/Medicine/GetMedicinesByDate?date={date}";
        string token = PlayerPrefs.GetString("Token");

        if (string.IsNullOrEmpty(token))
        {
            Debug.LogError("No token found. User needs to log in.");
            callback?.Invoke(false, null, "No token found.");
            yield break;
        }

        using (UnityWebRequest request = UnityWebRequest.Get(getMedicinesUrl))
        {
            request.SetRequestHeader("Authorization", "Bearer " + token);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;

                List<Medicine> medicines = JsonConvert.DeserializeObject<List<Medicine>>(jsonResponse);

                if (medicines != null && medicines.Count > 0)
                {
                    callback?.Invoke(true, medicines, "");
                }
                else
                {
                    callback?.Invoke(false, null, "No medicines found!");
                }
            }
            else
            {
                Debug.LogError($"Failed to fetch medicines: {request.error} (HTTP {request.responseCode})");
                callback?.Invoke(false, null, request.downloadHandler.text);
            }
        }
    }

    public static IEnumerator AddMedicine(Medicine medicine, System.Action<bool, string, Medicine> callback)
    {
        string addMedicineUrl = "https://localhost:44314/api/Medicine/AddMedicine";
        string token = PlayerPrefs.GetString("Token");

        if (string.IsNullOrEmpty(token))
        {
            Debug.LogError("No token found. User needs to log in.");
            callback?.Invoke(false, "No token found.", null);
            yield break;
        }

        DateTime parsedDate;
        if (DateTime.TryParse(medicine.date, out parsedDate))
        {
            medicine.date = parsedDate.ToString("yyyy-MM-dd");
        }
        else
        {
            Debug.LogError("Invalid date format. Please check the date value.");
            callback?.Invoke(false, "Invalid date format.", null);
            yield break;
        }

        string jsonData = JsonUtility.ToJson(medicine);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(addMedicineUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + token);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Medicine added successfully!");
                Debug.Log("Raw response: " + request.downloadHandler.text);

                MedicineResponseWrapper responseWrapper = JsonUtility.FromJson<MedicineResponseWrapper>(request.downloadHandler.text);

                if (responseWrapper != null && responseWrapper.medicine != null)
                {
                    Debug.Log("Received Medicine ID: " + responseWrapper.medicine.id);
                    callback?.Invoke(true, request.downloadHandler.text, responseWrapper.medicine);
                }
                else
                {
                    Debug.LogError("Could not parse medicine from response.");
                    callback?.Invoke(false, "Failed to parse response", null);
                }
            }
            else
            {
                Debug.LogError($"Failed to add medicine. Error: {request.error} (HTTP {request.responseCode})");
                callback?.Invoke(false, request.error, null);
            }
        }
    }

    //public static IEnumerator EditMedicine(int medicineId, Medicine medicine, System.Action<bool, string, Medicine> callback)
    //{
    //    string addMedicineUrl = "https://localhost:44314/api/Medicine/UpdateMedicine";
    //    string token = PlayerPrefs.GetString("Token");

    //    if (string.IsNullOrEmpty(token))
    //    {
    //        Debug.LogError("No token found. User needs to log in.");
    //        callback?.Invoke(false, "No token found.", null);
    //        yield break;
    //    }

    //    DateTime parsedDate;
    //    if (DateTime.TryParse(medicine.date, out parsedDate))
    //    {
    //        medicine.date = parsedDate.ToString("yyyy-MM-dd");
    //    }
    //    else
    //    {
    //        Debug.LogError("Invalid date format. Please check the date value.");
    //        callback?.Invoke(false, "Invalid date format.", null);
    //        yield break;
    //    }

    //    string jsonData = JsonUtility.ToJson(medicine);
    //    byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

    //    using (UnityWebRequest request = new UnityWebRequest(addMedicineUrl, "POST"))
    //    {
    //        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
    //        request.downloadHandler = new DownloadHandlerBuffer();
    //        request.SetRequestHeader("Content-Type", "application/json");
    //        request.SetRequestHeader("Authorization", "Bearer " + token);

    //        yield return request.SendWebRequest();

    //        if (request.result == UnityWebRequest.Result.Success)
    //        {
    //            Debug.Log("Medicine added successfully!");
    //            Debug.Log("Raw response: " + request.downloadHandler.text);

    //            MedicineResponseWrapper responseWrapper = JsonUtility.FromJson<MedicineResponseWrapper>(request.downloadHandler.text);

    //            if (responseWrapper != null && responseWrapper.medicine != null)
    //            {
    //                Debug.Log("Received Medicine ID: " + responseWrapper.medicine.id);
    //                callback?.Invoke(true, request.downloadHandler.text, responseWrapper.medicine);
    //            }
    //            else
    //            {
    //                Debug.LogError("Could not parse medicine from response.");
    //                callback?.Invoke(false, "Failed to parse response", null);
    //            }
    //        }
    //        else
    //        {
    //            Debug.LogError($"Failed to add medicine. Error: {request.error} (HTTP {request.responseCode})");
    //            callback?.Invoke(false, request.error, null);
    //        }
    //    }
    //}

    public static IEnumerator DeleteMedicine(int medicineId, Action<bool, string> callback)
    {
        string url = $"https://localhost:44314/api/Medicine/DeleteMedicine/{medicineId}";
        string token = PlayerPrefs.GetString("Token");

        Debug.Log($"Trying to delete medicine with ID: {medicineId}");

        using (UnityWebRequest request = UnityWebRequest.Delete(url))
        {
            request.SetRequestHeader("Authorization", "Bearer " + token);
            yield return request.SendWebRequest();
            Console.WriteLine($"Deleting medicine with ID: {medicineId}");

            Debug.Log($"Status Code: {request.responseCode}");

            if (request.result == UnityWebRequest.Result.Success)
            {
                callback?.Invoke(true, " Medicine deleted.");
            }
            else
            {
                callback?.Invoke(false, $"Failed to delete medicine: {request.error}");
            }
        }
    }
}
