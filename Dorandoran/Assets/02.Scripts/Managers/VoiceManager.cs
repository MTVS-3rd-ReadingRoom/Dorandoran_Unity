using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.Services.Authentication;
using Photon.Voice.Unity;
using System.Threading;
using System.Linq;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering.UI;
using UnityEngine.Profiling;
using Photon.Voice.PUN;

public class VoiceManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI logText;
    public Button button;

    //Dictionary<int, string> playerInfo = new Dictionary<int, string>();
    //List<string> playerNameList = new List<string>();
    List<int> playerActorNum = new List<int>();
    int playerIndex = -1;

    // 플레이어 목록을 ActorNumber 기준으로 정렬하는 함수
    //private Player[] GetSortedPlayers()
    //{
    //    // Players.Values를 배열로 변환
    //    Player[] playersArray = PhotonNetwork.CurrentRoom.Players.Values.ToArray();

    //    Debug.Log("현재 플레이어 개수(정렬중): " + PhotonNetwork.CurrentRoom.Players.Count);
    //    // 배열을 ActorNumber 기준으로 정렬
    //    Array.Sort(playersArray, (p1, p2) => p1.ActorNumber.CompareTo(p2.ActorNumber));

    //    return playersArray;
    //}

    //public void UpdatePlayerList()
    //{
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        playerActorNum.Clear();
    //        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values.ToArray())
    //        {
    //            playerActorNum.Add(player.ActorNumber);
    //        };

    //        playerActorNum.Sort();
    //    }
    //}

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // 마스터 클라이언트일 경우
        if (PhotonNetwork.IsMasterClient) // 마스터 클라이언트만 플레이어 정보를 받도록
        {
            playerActorNum.Add(newPlayer.ActorNumber);
        }
    }

    public override void OnJoinedRoom() // 방에 입장했을때
    {
        // 마스터 클라이언트가 입장했을 때
        if (PhotonNetwork.IsMasterClient)
        {
            playerActorNum.Add(PhotonNetwork.MasterClient.ActorNumber);
        }
    }
 


    // 해당 actorNum의 플레이어 보이스 수정
    public void RecordPlayerVoice(int actorNum, bool on)
    {
        // 해당 actorNum를 가진 오브젝트에게 보이스 설정(on, off)
        photonView.RPC("TogglePlayerVoice", PhotonNetwork.CurrentRoom.GetPlayer(actorNum), on); // ActorNumber를 기준으로 뮤트
    }

    // 특정 플레이어 보이스 키고 끄기
    [PunRPC]
    public void TogglePlayerVoice(bool on)
    {
        Photon.Voice.Unity.Recorder recorder = GetComponent< Photon.Voice.Unity.Recorder> ();

        if (recorder)
            recorder.TransmitEnabled = on;
    }

    [PunRPC]
    public void NextPlayerSpeak(int order)
    {
        playerIndex = order;
    }

    public void NextPlayerSpeakData(int order)
    {
        photonView.RPC("NextPlayerSpeak", RpcTarget.MasterClient, order); // 플레이어 발표 순서를 마스터 클라이언트에게 전달
    }

    void Start()
    {
        logText = GameObject.Find("Canvas_UIManager/LogData").GetComponentInChildren<TextMeshProUGUI>();
        //button = GameObject.Find("Canvas/Button").GetComponentInChildren<Button>();
        //button.onClick.AddListener(ChangeSpeaker);

        Photon.Voice.Unity.Recorder recorder = GetComponent<Photon.Voice.Unity.Recorder>();
        recorder.VoiceDetectionThreshold = 0.00001f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 발언자를 변경하라는 함수 마스터 클라이언트로 보내기
    public void ChangeSpeaker()
    {
        photonView.RPC("ChangeSpeak", PhotonNetwork.MasterClient); // ActorNumber를 기준으로 뮤트
    }

    // 마스터 클라이언트에서 발언자 변경
    [PunRPC]
    public void ChangeSpeak()
    {
        playerIndex = (++playerIndex) % (PhotonNetwork.CurrentRoom.Players.Count);
        NextPlayerSpeakData(playerIndex);
        RecordPlayerVoice(playerActorNum[playerIndex], true); // 발표자 TransmitEnabled on

        for (int i = 0; i < PhotonNetwork.CurrentRoom.Players.Count; i++)
        {
            if (playerIndex != i)
            {
                RecordPlayerVoice(playerActorNum[i], true); // // 경청하는 사람 TransmitEnabled false
            }
        }
    }

    // 로그를 화면에 추가하는 함수
    public void SetLog(string message)
    {
        if (logText != null)
        {
            logText.text += message;
        }
    }
}


//PhotonView localPhotonView = PhotonView.Get(this);

//if (!localPhotonView.IsMine)
//    return;
//GameObject localObject = localPhotonView.gameObject;
//Transform childTransform = localObject.transform.Find("SpeakerObject");
//if (recorder.TransmitEnabled) // 소리 전송 중
//{
//    childTransform.gameObject.GetComponentInChildren<MeshRenderer>().enabled = true;
//}
//else
//{
//    childTransform.gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
//}