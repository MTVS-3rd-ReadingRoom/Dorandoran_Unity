using Photon.Pun;
using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using Photon.Voice.PUN;
using Photon.Realtime;
using Unity.VisualScripting;

public class ModeratorSound : MonoBehaviourPunCallbacks, IPunObservable
{
    public static ModeratorSound instance;

    Photon.Voice.Unity.Recorder recorder;
    PhotonView pv;

    List<string> allowedUserIds = new List<string> { };

    int playerID = 0;

    // ������ Ŭ���̾�Ʈ
    // ���� ��� Ŭ���̾�Ʈ ���̵� ����
    // ����ũ ���� ���̵� ���� ����
    // ����ũ ���̵� ��ư�� ������ ����


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // ���� ������ ID ��������
        string currentUserId = PhotonNetwork.LocalPlayer.UserId;

        recorder = GetComponentInChildren<Photon.Voice.Unity.Recorder>();
        pv = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SpeakPlayer(AudioClip audioClip)
    {
        if(PhotonNetwork.IsMasterClient) // ���常 ���� �����ϵ��� 
        {
            RPC_UpdateSound(true, audioClip);
        }
    }

    [PunRPC]
    public void UpdateSound(bool play)
    {
        if (recorder.AudioClip == null)
            return;
        recorder.enabled = true;
        recorder.LoopAudioClip = false;  
        recorder.TransmitEnabled = true; // ������ ��
        recorder.RecordWhenJoined = true;

        recorder.RestartRecording();
    }

    public void RPC_UpdateSound(bool play, AudioClip audioClip)
    {
        recorder.AudioClip = audioClip;
        recorder.enabled = true;
        recorder.RecordingEnabled = true;
        recorder.LoopAudioClip = false;
        recorder.TransmitEnabled = true; // ������ ��
        recorder.RecordWhenJoined = true;

        recorder.RestartRecording();
        //pv.RPC("UpdateSound", RpcTarget.All, play);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
