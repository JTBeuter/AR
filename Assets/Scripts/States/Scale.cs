using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Scale : IState
{
    public float YSwipe;
    public Vector2 Swipe0;
    public Vector2 Swipe1;
    private float scale;
    private Vector2 startTouch;
    private Vector2 currentTouch;
    private Vector2 startTouch1;
    private Vector2 endTouch1;
    private float oldScale;

    float touchDifference;

    public IState DoState(StateManager sm)
    {
        if (sm.stateEnabled)
        {
            sm.visual.SetActive(false);
            sm.ScaleButton.GetComponent<Button>().interactable = false;
            sm.ScanEnvironmentPopup.SetActive(false);

            bool IsPointerOverUIObject()
            {
                PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
                eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
                return results.Count > 0;
            }

            if (Input.touchCount != 1 && sm.clone != null)
            {
                oldScale = sm.clone.transform.localScale.y;
            }

            scale = Mathf.Clamp(scale, sm.minScale, sm.maxScale);

            if (!IsPointerOverUIObject() && sm.clone != null)
            {
                if (Input.touchCount == 2)
                {
                    Touch touch0 = Input.GetTouch(0);
                    Touch touch1 = Input.GetTouch(1);

                    Vector2 PrevTouchPos0 = touch0.position - touch0.deltaPosition;
                    Vector2 PrevTouchPos1 = touch1.position - touch1.deltaPosition;

                    float prevMagnitude = (PrevTouchPos0 - PrevTouchPos1).magnitude;
                    float currentMagnitude = (touch0.position - touch1.position).magnitude;

                    touchDifference = currentMagnitude - prevMagnitude;

                    scale += touchDifference / 300;
                    scale = Mathf.Clamp(scale, sm.minScale, sm.maxScale);
                    sm.clone.transform.localScale = Vector3.one * scale;
                }
                else if (Input.GetMouseButton(0))
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        startTouch = Input.touches[0].position;
                    }

                    currentTouch = Input.touches[0].position;

                    YSwipe = currentTouch.y -= startTouch.y;
                    scale = oldScale + (YSwipe / 300);

                    scale = Mathf.Clamp(scale, sm.minScale, sm.maxScale);
                    sm.clone.transform.localScale = Vector3.one * scale;
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

            if (sm.clickedPosition)
            {
                sm.ScaleButton.GetComponent<Button>().interactable = true;
                sm.clickedScale = false;
                return sm.position;
            }

            if (sm.clickedRotation)
            {
                sm.ScaleButton.GetComponent<Button>().interactable = true;
                sm.clickedScale = false;
                return sm.rotation;
            }
        }

        return this;
    }
}
