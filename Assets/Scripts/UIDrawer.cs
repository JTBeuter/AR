using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDrawer : MonoBehaviour
{
    [SerializeField] bool drawerOpen;
    [SerializeField] RectTransform drawer;
    [SerializeField] RectTransform toggle;
    [SerializeField] GameObject StateManager;
    [SerializeField] GameObject visual;

    private void Awake()
    {
        if (drawer == null)
        {
            Debug.LogError(new System.Exception("Field 'drawer' not set int the editor!"));
        }
    }

    public void ToggleDrawer()
    {
        StateManager.SetActive(false);

        if (drawerOpen)
        { // Close drawer
            drawer.localScale = new Vector3(0f, 1f, 1f);
            toggle.anchorMin = new Vector2(0f, 0f);
            toggle.anchorMax = new Vector2(0f, 1f);

            StateManager.GetComponent<StateManager>().stateEnabled = false;
            StateManager.GetComponent<StateManager>().visual.SetActive(false);
            StateManager.GetComponent<StateManager>().ScanEnvironmentPopup.SetActive(false);
            StateManager.GetComponent<StateManager>().PlayerPlaceable = false;
        }
        else
        { // Open drawer
            drawer.localScale = new Vector3(1f, 1f, 1f);
            toggle.anchorMin = new Vector2(1f, 0f);
            toggle.anchorMax = new Vector2(1f, 1f);

            StateManager.GetComponent<StateManager>().stateEnabled = true;
        }

        toggle.GetChild(0).Rotate(Vector3.forward, 180f);
        drawerOpen = !drawerOpen;

        StateManager.SetActive(true);
    }
}
