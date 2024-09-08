using Photon.Pun;
using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using Photon.Voice.PUN;
using Photon.Realtime;

public class ModeratorSound : MonoBehaviourPunCallbacks, IPunObservable
{
    Photon.Voice.Unity.Recorder recorder;
    PhotonView pv;

    List<string> allowedUserIds = new List<string> { };

    int playerID = 0;

    public AudioClip audioclipdata;

    // ������ Ŭ���̾�Ʈ
    // ���� ��� Ŭ���̾�Ʈ ���̵� ����
    // ����ũ ���� ���̵� ���� ����
    // ����ũ ���̵� ��ư�� ������ ����

    // Start is called before the first frame update
    void Start()
    {
        // ���� ������ ID ��������
        string currentUserId = PhotonNetwork.LocalPlayer.UserId;

        recorder = GetComponentInChildren<Photon.Voice.Unity.Recorder>();
        pv = GetComponent<PhotonView>();

        SpeakPlayer();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void SpeakPlayer()
    {
        if(PhotonNetwork.IsMasterClient) // ���常 ���� �����ϵ��� 
        {
            RPC_UpdateSound(true);
        }
    }

    [PunRPC]
    public void UpdateSound(bool play)
    {
        recorder.AudioClip = audioclipdata;
        recorder.LoopAudioClip = true;  
        recorder.TransmitEnabled = true;
        recorder.RestartRecording();
    }

    public void RPC_UpdateSound(bool play)
    {
        pv.RPC("UpdateSound", RpcTarget.All, play);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
