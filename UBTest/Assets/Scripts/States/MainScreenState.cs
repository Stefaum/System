﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScreenState : MonoBehaviour, IFSMState
{

    public IEnumerator Enter()
    {
        yield return new WaitForEndOfFrame();
    }

    public IEnumerator Exit()
    {
        yield return new WaitForEndOfFrame();
    }

    public void FSMUpdate(){

    }
}
