using UnityEngine;

public class UI_Progress : MonoBehaviour
{
    [SerializeField] private UI_Navbar navbarUI;

    private void OnEnable()
    {
        navbarUI.ChangeGUI(2);
    }
}
