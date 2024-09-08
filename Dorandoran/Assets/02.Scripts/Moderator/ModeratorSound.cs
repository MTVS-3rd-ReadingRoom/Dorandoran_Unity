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

    // 마스터 클라이언트
    // 현재 모든 클라이언트 아이디를 보관
    // 스피크 중인 아이디 정보 보관
    // 스피크 아이디를 버튼을 누르면 변경

    // Start is called before the first frame update
    void Start()
    {
        // 현재 유저의 ID 가져오기
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
        if(PhotonNetwork.IsMasterClient) // 방장만 설정 가능하도록 
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
