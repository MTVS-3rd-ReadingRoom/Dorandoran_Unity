using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void CallBackAction();

public class EffectTrigger : MonoBehaviour
{
    public GameObject[] Effects;
    public CallBackAction callBackAction;

    public void ActiveSound(string soundEnum)
    {
        SoundManager.instance.ActiveOnShotSFXSound(SoundManager.instance.enumToString[soundEnum], transform, Vector3.zero);
    }

    public void ActvieEffect(int index)
    {
        Effects[index].SetActive(false);
        Effects[index].SetActive(true);
        if(callBackAction != null)
        {
            callBackAction();
        }
    }
}
