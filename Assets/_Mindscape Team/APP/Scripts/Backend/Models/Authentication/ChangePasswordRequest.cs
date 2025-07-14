using UnityEngine;

[System.Serializable]
public class ChangePasswordRequest
{
    public string currentPassword;
    public string newPassword;
    public string confirmPassword;
}
