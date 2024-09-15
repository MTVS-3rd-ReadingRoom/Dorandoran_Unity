using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CinemachineManager : MonoBehaviour
{
    private CinemachineBlendListCamera cinemachine;
    public CinemachineVirtualCamera[] virtualCameras;

    private void Awake()
    {
        cinemachine = GetComponent<CinemachineBlendListCamera>();

    }

    public void AddInstructions(int index)
    {
        List<CinemachineBlendListCamera.Instruction> instructions = new List<CinemachineBlendListCamera.Instruction>();
        instructions.AddRange(cinemachine.m_Instructions);
        CinemachineBlendListCamera.Instruction newInstruction = new CinemachineBlendListCamera.Instruction();
        newInstruction.m_VirtualCamera = virtualCameras[index];
        newInstruction.m_Hold = 0f;
        newInstruction.m_Blend.m_Style = CinemachineBlendDefinition.Style.EaseIn;
        newInstruction.m_Blend.m_Time = 1f;
        instructions.Add(newInstruction);
        
        cinemachine.m_Instructions = instructions.ToArray();
    }


}
