﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectTrackVRState : MonoBehaviour, IFSMState
{
    public IEnumerator Enter()
    {
        EventManager.StartVR();
        yield return new WaitForEndOfFrame();
    }

    public IEnumerator Exit()
    {
        yield return new WaitForEndOfFrame();
    }

    public void FSMUpdate(){

    }
}