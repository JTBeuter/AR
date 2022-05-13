using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TakePicture : MonoBehaviour
{
    [SerializeField]
    GameObject UI;
    [SerializeField]
    GameObject Visual;
    [SerializeField]
    GameObject StateManager;

    public void TakeScreenshot()
    {
        UI.SetActive(false);
        Visual.SetActive(false);
        StateManager.SetActive(false);
        StartCoroutine(CRSaveScreenshot());
    }

    IEnumerator CRSaveScreenshot()
    {
        yield return new WaitForEndOfFrame();
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        string name = "Tag_der_Fans" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";

        NativeGallery.SaveImageToGallery(texture, "Tag der Fans", name);

        Destroy(texture);
        UI.SetActive(true);
        Visual.SetActive(true);
        StateManager.SetActive(true);
        StateManager.GetComponent<ShowPopup>()._ShowPopup();
    }
}
