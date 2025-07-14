using UnityEngine;
using UnityEngine.UI;

public class UI_GettingReady : MonoBehaviour
{
    [SerializeField] private Toggle toggle1;
    [SerializeField] private Toggle toggle2;
    [SerializeField] private Toggle toggle3;
    [SerializeField] private Button readyButton;

    public void CheckAllToggles()
    {
        if (toggle1.isOn && toggle2.isOn && toggle3.isOn)
        {
            toggle1.interactable = false;
            toggle2.interactable = false;
            toggle3.interactable = false;

            readyButton.interactable = true;
        }
    }
}