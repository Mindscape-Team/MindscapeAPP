using UnityEngine;
using UnityEngine.UI;

public class PackageSelector : MonoBehaviour
{
    [SerializeField] private Toggle videoCallToggle;
    [SerializeField] private Toggle inPersonToggle;

    public string selectedPackageType = "Video Call";

    public void OnVideoCallToggled(bool isOn)
    {
        if (isOn)
            selectedPackageType = "Video Call";
    }

    public void OnInPersonToggled(bool isOn)
    {
        if (isOn)
            selectedPackageType = "In Person";
    }

    public void SelectPackageType()
    {
        UI_DateTimeHandler dateTimeHandler = FindObjectOfType<UI_DateTimeHandler>();

        if (dateTimeHandler != null)
        {
            dateTimeHandler.SetPackageType(selectedPackageType);
            Debug.Log($"selected package type: {selectedPackageType}");
        }
    }
}