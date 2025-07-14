using System.Collections.Generic;

[System.Serializable]
public class Medicine
{
    public int id;
    public string name;
    public string dosageFrequency;
    public string dosage;
    public string medicineTime;
    public string medicinePhoto;
    public string date;
}

[System.Serializable]
public class MedicineListWrapper
{
    public List<Medicine> medicines;
}

[System.Serializable]
public class MedicineResponseWrapper
{
    public string message;
    public Medicine medicine;
}