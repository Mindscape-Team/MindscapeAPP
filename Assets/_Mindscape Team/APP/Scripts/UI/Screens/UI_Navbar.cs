using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Navbar : MonoBehaviour
{
    [SerializeField] private Button[] btns;

    [SerializeField] private Sprite[] activeSprites;
    [SerializeField] private Sprite[] inActiveSprites;

    //int index = 0;
    //public void ChangeIndex(int i)
    //{
    //    index = i;
    //}

    public void ChangeGUI(int index)
    {
        for (int i = 0; i < btns.Length; i++)
        {
            if (i == index)
            {
                btns[i].image.sprite = activeSprites[i];
                TMP_Text btnActiveTxt = btns[i].GetComponentInChildren<TMP_Text>();
                btnActiveTxt.color = new Color(0.423f, 0.294f, 0.980f);
            }
            else
            {
                btns[i].image.sprite = inActiveSprites[i];
                TMP_Text btnInActiveTxt = btns[i].GetComponentInChildren<TMP_Text>();
                btnInActiveTxt.color = Color.black;
            }
        }
    }

    //private void Start()
    //{
    //    btns[0].onClick.AddListener(() => ChangeIndex(0));
    //    btns[1].onClick.AddListener(() => ChangeIndex(1));
    //    btns[2].onClick.AddListener(() => ChangeIndex(2));
    //    btns[3].onClick.AddListener(() => ChangeIndex(3));
    //}
    //private void Update()
    //{
    //    if (index == 0)
    //    {
    //        ChangeGUI(index);
    //    }

    //    if (index == 1)
    //    {
    //        ChangeGUI(index);
    //    }

    //    if (index == 2)
    //    {
    //        ChangeGUI(index);
    //    }

    //    if (index == 3)
    //    {
    //        ChangeGUI(index);
    //    }
    //}
}
