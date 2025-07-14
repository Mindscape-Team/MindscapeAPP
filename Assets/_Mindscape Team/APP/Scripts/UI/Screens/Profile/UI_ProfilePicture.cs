using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UI_ProfilePicture : MonoBehaviour
{
    [SerializeField] private Image profileImage;
    [SerializeField] private Sprite defaultSprite;

    [SerializeField] private Button cameraButton;
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button takePhotoButton;

    public static byte[] imageData;

    //private Sprite defaultSprite;

    //private void Start()
    //{
    //    if (PlayerPrefs.HasKey("Profile Picture"))
    //    {
    //        string profilePicture = PlayerPrefs.GetString("Profile Picture");
    //        byte[] Image = Convert.FromBase64String(profilePicture);

    //        profileImage.sprite = UIHelperMethods.ImageLoader(Image);
    //    }
    //    else
    //    {
    //        defaultSprite = profileImage.sprite;
    //    }
    //}

    public void TakePhoto()
    {
        Debug.Log("Camera function is not implemented yet.");
    }

    public void ChangeProfilePhoto()
    {
        string path = UnityEditor.EditorUtility.OpenFilePanel("Select a photo", "", "png,jpg,jpeg");
        if (!string.IsNullOrEmpty(path))
        {
            imageData = File.ReadAllBytes(path);

            Sprite sprite = UI_HelperMethods.ImageLoader(imageData);

            profileImage.sprite = sprite;
            profileImage.preserveAspect = true;
        }
    }

    public void DeleteProfilePhoto()
    {
        imageData = UI_HelperMethods.ImageUnloader(defaultSprite);

        profileImage.sprite = defaultSprite;
    }
}
