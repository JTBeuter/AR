using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using UnityEngine.Android;

public class Position : MonoBehaviour, IState
{
    public IState DoState(StateManager sm)
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            sm.PermissionsNotAllowed.SetActive(true);
        }
        else
        {
            sm.PermissionsNotAllowed.SetActive(false);
        }

        if (sm.hasPlayed == 0 && !sm.PlayerPlacementTippShown && Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            sm.PlayerPlacementTipp.SetActive(true);
            sm.PlayerPlacementTippShown = true;
        }

        if (sm.stateEnabled && !sm.PlayerPlacementTipp.activeSelf)
        {
            sm.visual.SetActive(true);
            sm.PositionButton.GetComponent<Button>().interactable = false;

            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            sm.raycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes);

            bool IsPointerOverUIObject()
            {
                PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
                eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
                return results.Count > 0;
            }

            if (!Physics.Raycast(sm.cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0))) && Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                sm.ScanEnvironmentPopup.SetActive(true);
                sm.PlayerPlaceable = false;
                sm.visual.SetActive(false);
            }
            else if (Physics.Raycast(sm.cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0))))
            {
                sm.ScanEnvironmentPopup.SetActive(false);
                sm.PlayerPlaceable = true;
                sm.visual.SetActive(true);
            }

            if (hits.Count > 0)
            {
                sm.pos = hits[0].pose.position;
                sm.visual.transform.position = sm.pos;
            }

            if (!IsPointerOverUIObject() && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && sm.PlayerPlaceable)
            {
                if (sm.clone == null)
                {
                    sm.clone = Instantiate(sm.Character, sm.pos, Quaternion.LookRotation(sm.cam.transform.position));
                    sm.UIDrawer.SetActive(true);
                    sm.takePictureButton.SetActive(true);
                    sm.UIDrawer.GetComponent<UIDrawer>().ToggleDrawer();
                    sm.clickedClear = false;
                    sm.clickedPosition = true;

                    if (sm.hasPlayed == 0 && !sm.SwitchModeTippShown)
                    {
                        sm.SwitchModeTipp.SetActive(true);
                        sm.SwitchModeTippShown = true;
                    }
                }
                else
                {
                    sm.clone.transform.position = sm.pos;
                }

                sm.lookRotation = Quaternion.LookRotation(sm.cam.transform.position - sm.clone.transform.position).eulerAngles;
                sm.clone.transform.rotation = Quaternion.Euler(Vector3.Scale(sm.lookRotation, sm.rotationMask));
            }

            if (sm.clickedRotation)
            {
                sm.PositionButton.GetComponent<Button>().interactable = true;
                sm.clickedPosition = false;
                return sm.rotation;
            }

            if (sm.clickedScale)
            {
                sm.PositionButton.GetComponent<Button>().interactable = true;
                sm.clickedPosition = false;
                return sm.scale;
            }
        }
        else
        {
            return this;
        }
        
        return this;
    }
}