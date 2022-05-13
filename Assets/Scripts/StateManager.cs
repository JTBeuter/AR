using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public interface IState
{
    public IState DoState(StateManager sm);
}

public class StateManager : MonoBehaviour
{
    #region Variables
    public GameObject Character;
    public GameObject visual;
    public Camera cam;
    public GameObject ARSessionOrigin;
    public GameObject NoPlayerPlacedPopup;
    public GameObject ScanEnvironmentPopup;
    public GameObject UI;
    public Vector3 rotationMask;
    public float minScale;
    public float maxScale;
    [HideInInspector]
    public GameObject clone;
    [HideInInspector]
    public Vector3 pos;
    [HideInInspector]
    public Vector3 lookRotation;
    [HideInInspector]
    public Vector3 oldCloneScale;
    [HideInInspector]
    public Vector3 oldVisualScale;
    [HideInInspector]
    public Vector3 oldRotation;
    [HideInInspector]
    public float YSwipe;
    [HideInInspector]
    public float XSwipe;
    [HideInInspector]
    public bool noUIElementSelected = true;
    public GameObject PositionButton;
    public GameObject RotationButton;
    public GameObject ScaleButton;
    public bool clickedPosition { get; set; } = false;
    public bool clickedScale { get; set; } = false;
    public bool clickedRotation { get; set; } = false;
    public bool clickedClear { get; set; } = false;
    public GameObject UIDrawer;
    public Image PopupImage;
    public bool PlayerPlaceable = true;
    public bool stateEnabled = true;
    public GameObject takePictureButton;
    public GameObject PermissionsNotAllowed;

    [SerializeField]
    public ARCameraManager cameraManager;
    [SerializeField]
    public ARRaycastManager raycastManager;

    [HideInInspector]
    public int hasPlayed;
    public GameObject PlayerPlacementTipp;
    [HideInInspector]
    public bool PlayerPlacementTippShown = false;
    public GameObject SwitchModeTipp;
    [HideInInspector]
    public bool SwitchModeTippShown = false;
    public GameObject TakePictureTipp;
    [HideInInspector]
    public bool TakePictureTippShown = false;
    #endregion

    public IState position = new Position();
    public IState rotation = new Rotatation();
    public IState scale = new Scale();
    public IState currentState;

    public void Start()
    {
        clickedPosition = true;
        currentState = position;
        
        hasPlayed = PlayerPrefs.GetInt("HasPlayed");

        if (hasPlayed == 0)
        {
            PlayerPrefs.SetInt("HasPlayed", 1);
        }
    }

    public void Update()
    {
        currentState = currentState.DoState(this);
    }

    public void RequestPermission()
    {
        Permission.RequestUserPermission(Permission.Camera);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void HidePopup()
    {
        EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false);
    }
}