using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clear : IState
{
    public IState DoState(StateManager sm)
    {
        sm.visual.SetActive(false);
        sm.ScanEnvironmentPopup.SetActive(false);

        if (sm.clickedPosition)
        {
            sm.clickedClear = false;
            return sm.position;
        }

        if (sm.clickedScale)
        {
            sm.clickedClear = false;
            return sm.scale;
        }

        if (sm.clickedRotation)
        {
            sm.clickedClear = false;
            return sm.rotation;
        }

        return this;
    }
}
