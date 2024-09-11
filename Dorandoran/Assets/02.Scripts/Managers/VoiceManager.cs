using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice.Unity;
using Photon.Voice.PUN;
using Photon.Pun;

public class VoiceManager : MonoBehaviourPun, IPunObservable
{
    Recorder recorder;

    int playerIndex = -1;

    PhotonVoiceView voiceView;

    private void Start()
    {
        recorder = GetComponent<Recorder>();
        recorder.VoiceDetection = false;
    }

    private void Update()
    {
        //// 만약, V 키를 누르면 음소거
        //if (Input.GetKeyDown(KeyCode.V))
        //{
        //    recorder.TransmitEnabled = !recorder.TransmitEnabled;
        //}
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}