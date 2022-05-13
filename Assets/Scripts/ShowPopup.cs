using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowPopup : MonoBehaviour
{
    [SerializeField]
    private GameObject ImageSaved;
    [SerializeField]
    private Image PopupImage;

    public void _ShowPopup()
    {
        ImageSaved.GetComponent<CanvasRenderer>().SetAlpha(1);
        ImageSaved.SetActive(true);
        PopupImage.CrossFadeAlpha(0, 1.5f, false);
        StartCoroutine(showPopup());
    }

    IEnumerator showPopup()
    {
        while (PopupImage.GetComponent<CanvasRenderer>().GetAlpha() != 0)
        {
            yield return null;
        }

        ImageSaved.SetActive(false);
    }
}
