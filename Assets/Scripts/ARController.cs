using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum CharacterInteraction
{
    POSITION, SCALE, ROTATION, CLEAR
}

public class ARController : MonoBehaviour
{
    #region Variables
    public GameObject Character;
    public GameObject visual;
    public Camera cam;
    public GameObject ARSessionOrigin;
    public GameObject NoPlayerPlacedPopup;
    public GameObject UI;
    public Vector3 rotationMask;
    public float minScale;
    public float maxScale;
    public CharacterInteraction characterInteraction;
    public Button PositionButton;

    private GameObject clone;
    private Vector3 pos;
    private Vector3 lookRotation;
    private Vector2 startTouch;
    private Vector2 endTouch;
    private Vector3 oldCloneScale;
    private Vector3 oldVisualScale;
    private Vector3 oldRotation;
    private float YSwipe;
    private float XSwipe;
    private bool noUIElementSelected = true;
    public Image PopupImage;

    [SerializeField]
    private ARCameraManager cameraManager;
    [SerializeField]
    private ARRaycastManager raycastManager;
    #endregion

    private void Start()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();

        PopupImage = NoPlayerPlacedPopup.GetComponent<Image>();
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            noUIElementSelected = true;
        }
        else
        {
            noUIElementSelected = false;
        }

        if (noUIElementSelected)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                startTouch = Input.touches[0].position;
            }

            if (Input.touchCount > 0)
            {
                endTouch = Input.touches[0].position;
                YSwipe = endTouch.y -= startTouch.y;
                XSwipe = endTouch.x -= startTouch.x;
            }
        }

        switch (characterInteraction)
        {
            case CharacterInteraction.CLEAR:
                break;

            case CharacterInteraction.POSITION:
                PlaceObj();
                break;

            case CharacterInteraction.SCALE:
                ScaleObj();
                break;

            case CharacterInteraction.ROTATION:
                RotateObj();
                break;

            default:
                break;
        }

        if (PopupImage.GetComponent<CanvasRenderer>().GetAlpha() == 0)
        {
            NoPlayerPlacedPopup.SetActive(false);
        }
    }

    public void ClearMode()
    {
        visual.SetActive(false);

        characterInteraction = CharacterInteraction.CLEAR;
    }

    public void PlaceMode()
    {
        visual.SetActive(true);

        characterInteraction = CharacterInteraction.POSITION;
    }

    public void ScaleMode()
    {
        visual.SetActive(false);

        if (clone != null)
        {
            characterInteraction = CharacterInteraction.SCALE;
        }
        else
        {
            ShowPopup();
        }
    }

    public void RotateMode()
    {
        visual.SetActive(false);

        if (clone != null)
        {
            characterInteraction = CharacterInteraction.ROTATION;
        }
        else
        {
            ShowPopup();
        }
    }

    private void PlaceObj()
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        raycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes);

        if (hits.Count > 0)
        {
            pos = hits[0].pose.position;
            visual.transform.position = pos;
        }

        if (noUIElementSelected && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (clone == null)
            {
                clone = Instantiate(Character, pos, Quaternion.LookRotation(cam.transform.position));
            }
            else
            {
                clone.transform.position = pos;
            }

            lookRotation = Quaternion.LookRotation(cam.transform.position - clone.transform.position).eulerAngles;
            clone.transform.rotation = Quaternion.Euler(Vector3.Scale(lookRotation, rotationMask));
        }
    }

    private void ScaleObj()
    {
        if (Input.touchCount == 0)
        {
            oldCloneScale = clone.transform.localScale;
            oldVisualScale = visual.transform.localScale;
        }

        if (noUIElementSelected && Input.touchCount > 0)
        {
            if (clone.transform.localScale.x >= minScale && clone.transform.localScale.x <= maxScale)
            {
                clone.transform.localScale = oldCloneScale + new Vector3(YSwipe / 300, YSwipe / 300, YSwipe / 300);
                visual.transform.localScale = oldVisualScale + new Vector3(YSwipe / 2500, YSwipe / 2500, YSwipe / 2500);
            }

            if (clone.transform.localScale.x < minScale)
            {
                clone.transform.localScale = new Vector3(minScale, minScale, minScale);
                visual.transform.localScale = new Vector3(minScale, minScale, minScale);
            }

            if (clone.transform.localScale.x > maxScale)
            {
                clone.transform.localScale = new Vector3(maxScale, maxScale, maxScale);
                visual.transform.localScale = new Vector3(maxScale, maxScale, maxScale);
            }
        }
    }

    private void RotateObj()
    {
        if (Input.touchCount == 0)
        {
            oldRotation = clone.transform.eulerAngles;
        }

        if (Input.touchCount > 0 && noUIElementSelected)
        {
            clone.transform.eulerAngles = new Vector3(0, oldRotation.y + XSwipe / -3, 0);
        }
    }

    public void TakeScreenshot()
    {
        UI.SetActive(false);
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
    }

    public void ShowPopup()
    {
        NoPlayerPlacedPopup.SetActive(true);
        PopupImage.GetComponent<CanvasRenderer>().SetAlpha(1);
        PopupImage.CrossFadeAlpha(0, 1.5f, false);
    }
}