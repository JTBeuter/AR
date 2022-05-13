using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Rotatation : IState
{
    public float XSwipe;
    private Vector2 startTouch;
    private Vector2 endTouch;
    private Vector2 oldRotation;

    public IState DoState(StateManager sm)
    {
        if (sm.stateEnabled)
        {
            sm.visual.SetActive(false);
            sm.RotationButton.GetComponent<Button>().interactable = false;
            sm.ScanEnvironmentPopup.SetActive(false);

            bool IsPointerOverUIObject()
            {
                PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
                eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
                return results.Count > 0;
            }

            if (!IsPointerOverUIObject())
            {
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    startTouch = Input.touches[0].position;
                }

                if (Input.touchCount > 0)
                {
                    endTouch = Input.touches[0].position;
                    XSwipe = endTouch.x -= startTouch.x;
                }

                if (Input.touchCount > 0)
                {
                    for (int i = 0; i < Input.touchCount; i++)
                    {
                        Touch currentTouch = Input.GetTouch(i);
                        if (currentTouch.fingerId == 0 && currentTouch.phase == TouchPhase.Ended && sm.hasPlayed == 0 && !sm.TakePictureTippShown)
                        {
                            sm.TakePictureTipp.SetActive(true);
                            sm.TakePictureTippShown = true;
                        }
                    }
                }
            }

            if (sm.clone != null)
            {
                if (Input.touchCount == 0)
                {
                    oldRotation = sm.clone.transform.eulerAngles;
                }

                if (Input.touchCount > 0 && !IsPointerOverUIObject())
                {
                    sm.clone.transform.eulerAngles = Vector3.up * (oldRotation.y + XSwipe / -3);
                }
            }
   
            if (sm.clickedPosition)
            {
                sm.RotationButton.GetComponent<Button>().interactable = true;
                sm.clickedRotation = false;
                return sm.position;
            }

            if (sm.clickedScale)
            {
                sm.RotationButton.GetComponent<Button>().interactable = true;
                sm.clickedRotation = false;
                return sm.scale;
            }
        }

        return this;
    }
}
