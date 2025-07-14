[System.Serializable]
public class AppointmentResponse
{
	public Appointment Appointment;
}

[System.Serializable]
public class Appointment
{
    public int id;
    public string userName;
    public string phoneNumber;
    public string packageType;
    public string date;
    public string time;
    public int doctorId;
}