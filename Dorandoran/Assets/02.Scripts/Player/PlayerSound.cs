using Photon.Pun;
using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using Photon.Voice.PUN;
using Photon.Realtime;

public class PlayerSound : MonoBehaviourPunCallbacks, IPunObservable
{
    Photon.Voice.Unity.Recorder recorder;
    PhotonView pv;

    List<string> allowedUserIds = new List<string> { };

    int playerID = 0;

    // 마스터 클라이언트
    // 현재 모든 클라이언트 아이디를 보관
    // 스피크 중인 아이디 정보 보관
    // 스피크 아이디를 버튼을 누르면 변경

    // Start is called before the first frame update
    void Start()
    {
        // 현재 유저의 ID 가져오기
        string currentUserId = PhotonNetwork.LocalPlayer.UserId;

        // 유저 ID 비교
        if (allowedUserIds.Contains(currentUserId))
        {
            // 허용된 유저라면 특정 기능 활성화
            
        }
        else
        {

        }

        recorder = GetComponentInChildren<Photon.Voice.Unity.Recorder>();
        pv = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            SpeakPlayer();
        }
    }

    void SpeakPlayer()
    {
        if(PhotonNetwork.IsMasterClient) // 방장만 설정 가능하도록 
        {
            // RPC_UpdateSound(speakId++);
        }
    }


    void UpdateSound(int SoundPlayerid)
    {
        //// 넣고
        //Dictionary<int, Player> playerDict = PhotonNetwork.CurrentRoom.Players;
        
        //foreach (KeyValuePair<int, Player> player in playerDict)
        //{
        //    userIds.Add(player.Value.UserId);
        //}

        //// 확인
        //if (userIds[SoundPlayerid] == pv.Owner.UserId) // 끌 플레이어 id
        //{
        //    recorder.TransmitEnabled = true; // 말한 내용 전달
        //}
        //else // 킬 플레이어 id
        //{
        //    recorder.TransmitEnabled = false; // 말한 내용 전달x
        //}

        //// 지운다.
        //userIds.Clear();
    }

    public void RPC_UpdateSound(int SoundPlayerid)
    {
        pv.RPC("UpdateSound", RpcTarget.All, SoundPlayerid);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
