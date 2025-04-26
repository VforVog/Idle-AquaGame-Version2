using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriticalClick : MonoBehaviour
{  
    public GameObject PopUp;
    public Animator Ani;

    public void Play()
    {
        Ani.Play("CriticalClickEffect");
        Destroy(gameObject, 0.5f);
    }

}