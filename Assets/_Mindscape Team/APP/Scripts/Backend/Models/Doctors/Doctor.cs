using System.Collections.Generic;

[System.Serializable]
public class Doctor
{
    public int id;
    public string name;
    public string specialization;
    public string location;
    public float rating;
    public string availableTime;
    public float price;
    public object Appointments { get; set; }
}

[System.Serializable]
public class DoctorListWrapper
{
    public List<Doctor> doctors;
}
