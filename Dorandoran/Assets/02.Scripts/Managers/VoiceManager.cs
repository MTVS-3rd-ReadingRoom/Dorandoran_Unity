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

public class VoiceManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI logText; // Inspector에서 Text 컴포넌트를 할당
    public Button button;

    //Dictionary<int, string> playerInfo = new Dictionary<int, string>();
    //List<string> playerNameList = new List<string>();
    List<int> playerActorNum = new List<int>();
    int playerIndex = -1;

    // 플레이어 목록을 ActorNumber 기준으로 정렬하는 함수
    private Player[] GetSortedPlayers()
    {
        // Players.Values를 배열로 변환
        Player[] playersArray = PhotonNetwork.CurrentRoom.Players.Values.ToArray();

        Debug.Log("현재 플레이어 개수(정렬중): " + PhotonNetwork.CurrentRoom.Players.Count);
        // 배열을 ActorNumber 기준으로 정렬
        Array.Sort(playersArray, (p1, p2) => p1.ActorNumber.CompareTo(p2.ActorNumber));

        return playersArray;
    }

    public void UpdatePlayerList()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            playerActorNum.Clear();
            foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values.ToArray())
            {
                playerActorNum.Add(player.ActorNumber);
            };

            playerActorNum.Sort();
            //playerInfo.Clear();
            //playerNameList.Clear();
            //foreach (Player player in GetSortedPlayers())
            //{
            //    string playerId = "Player_" + player.ActorNumber;
            //    playerInfo[player.ActorNumber] = playerId;
            //    playerNameList.Add(playerId);
            //}
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {

    }

    public void MutePlayerVoice(int actorNum, bool mute)
    {
        photonView.RPC("TogglePlayerVoice", PhotonNetwork.CurrentRoom.GetPlayer(actorNum), mute); // ActorNumber를 기준으로 뮤트

        //playerActorNum[playerIndex] == 
        //foreach (var item in playerInfo)
        //{
        //    if (item.Value == userId) // 플레이어 정보 중에 해당 id와 일치하는 것이 있다면
        //    {
        //        photonView.RPC("TogglePlayerVoice", PhotonNetwork.CurrentRoom.GetPlayer(item.Key), mute); // ActorNumber를 기준으로 뮤트
        //    }
        //}

    }

    [PunRPC]
    public void TogglePlayerVoice(bool mute)
    {
        Recorder recorder = GetComponent<Recorder>();

        if (recorder)
            recorder.TransmitEnabled = !mute;

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
        logText = GameObject.Find("Canvas/LogData").GetComponentInChildren<TextMeshProUGUI>();
        button = GameObject.Find("Canvas/Button").GetComponentInChildren<Button>();
        button.onClick.AddListener(ChangeSpeak);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V) && PhotonNetwork.IsMasterClient) // 방장일 경우에만
        {


        }
    }

    public void ChangeSpeak()
    {
        if (PhotonNetwork.IsMasterClient && photonView.IsMine)
        {
            playerIndex = (++playerIndex) % (PhotonNetwork.CurrentRoom.Players.Count);
            NextPlayerSpeakData(playerIndex);
            UpdatePlayerList();
            MutePlayerVoice(playerActorNum[playerIndex], false);

            for (int i = 0; i < PhotonNetwork.CurrentRoom.Players.Count; i++)
            {
                if (playerIndex != i)
                {
                    MutePlayerVoice(playerActorNum[i], true);
                }
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
