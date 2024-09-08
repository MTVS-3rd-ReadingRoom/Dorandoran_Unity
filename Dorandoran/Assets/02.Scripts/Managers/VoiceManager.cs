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
    public TextMeshProUGUI logText; // Inspector���� Text ������Ʈ�� �Ҵ�
    public Button button;

    //Dictionary<int, string> playerInfo = new Dictionary<int, string>();
    //List<string> playerNameList = new List<string>();
    List<int> playerActorNum = new List<int>();
    int playerIndex = -1;

    // �÷��̾� ����� ActorNumber �������� �����ϴ� �Լ�
    private Player[] GetSortedPlayers()
    {
        // Players.Values�� �迭�� ��ȯ
        Player[] playersArray = PhotonNetwork.CurrentRoom.Players.Values.ToArray();

        Debug.Log("���� �÷��̾� ����(������): " + PhotonNetwork.CurrentRoom.Players.Count);
        // �迭�� ActorNumber �������� ����
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
        photonView.RPC("TogglePlayerVoice", PhotonNetwork.CurrentRoom.GetPlayer(actorNum), mute); // ActorNumber�� �������� ��Ʈ

        //playerActorNum[playerIndex] == 
        //foreach (var item in playerInfo)
        //{
        //    if (item.Value == userId) // �÷��̾� ���� �߿� �ش� id�� ��ġ�ϴ� ���� �ִٸ�
        //    {
        //        photonView.RPC("TogglePlayerVoice", PhotonNetwork.CurrentRoom.GetPlayer(item.Key), mute); // ActorNumber�� �������� ��Ʈ
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
        //if (recorder.TransmitEnabled) // �Ҹ� ���� ��
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
        photonView.RPC("NextPlayerSpeak", RpcTarget.MasterClient, order); // �÷��̾� ��ǥ ������ ������ Ŭ���̾�Ʈ���� ����
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
        if (Input.GetKeyDown(KeyCode.V) && PhotonNetwork.IsMasterClient) // ������ ��쿡��
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

    // �α׸� ȭ�鿡 �߰��ϴ� �Լ�
    public void SetLog(string message)
    {
        if (logText != null)
        {
            logText.text += message;
        }
    }
}
