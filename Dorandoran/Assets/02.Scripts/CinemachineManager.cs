using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CinemachineManager : MonoBehaviour
{
    private CinemachineBlendListCamera cinemachine;
    public CinemachineBlendListCamera.Instruction[] instructions;

    private void Awake()
    {
        cinemachine = GetComponent<CinemachineBlendListCamera>();
    }
    public bool test = false;

    private void Update()
    {
        if (test)
        {
            test = false;
        }   
    }

    public void AddList(CinemachineVirtualCamera virtualCamera, float hold, CinemachineBlendDefinition.Style style)
    {
        
    }
}
