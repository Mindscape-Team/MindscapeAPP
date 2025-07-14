using System;

[Serializable]
public class AuthResponse
{
    public string message;
    public bool isValid;
    public string[] roles;

    public string token;
    public string expireOn;
    public string userName;
    public string email;
    public string fullName;
    //public bool hasProfilePicture;
    public bool hasAnxietyResponse;
    public bool hasDepressionResponse;
}