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

    // �÷��̾� ����� ActorNumber �������� �����ϴ� �Լ�
    //private Player[] GetSortedPlayers()
    //{
    //    // Players.Values�� �迭�� ��ȯ
    //    Player[] playersArray = PhotonNetwork.CurrentRoom.Players.Values.ToArray();

    //    Debug.Log("���� �÷��̾� ����(������): " + PhotonNetwork.CurrentRoom.Players.Count);
    //    // �迭�� ActorNumber �������� ����
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
        // ������ Ŭ���̾�Ʈ�� ���
        if (PhotonNetwork.IsMasterClient) // ������ Ŭ���̾�Ʈ�� �÷��̾� ������ �޵���
        {
            playerActorNum.Add(newPlayer.ActorNumber);
        }
    }

    public override void OnJoinedRoom() // �濡 ����������
    {
        // ������ Ŭ���̾�Ʈ�� �������� ��
        if (PhotonNetwork.IsMasterClient)
        {
            playerActorNum.Add(PhotonNetwork.MasterClient.ActorNumber);
        }
    }
 


    // �ش� actorNum�� �÷��̾� ���̽� ����
    public void RecordPlayerVoice(int actorNum, bool on)
    {
        // �ش� actorNum�� ���� ������Ʈ���� ���̽� ����(on, off)
        photonView.RPC("TogglePlayerVoice", PhotonNetwork.CurrentRoom.GetPlayer(actorNum), on); // ActorNumber�� �������� ��Ʈ
    }

    // Ư�� �÷��̾� ���̽� Ű�� ����
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
        photonView.RPC("NextPlayerSpeak", RpcTarget.MasterClient, order); // �÷��̾� ��ǥ ������ ������ Ŭ���̾�Ʈ���� ����
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

    // �߾��ڸ� �����϶�� �Լ� ������ Ŭ���̾�Ʈ�� ������
    public void ChangeSpeaker()
    {
        photonView.RPC("ChangeSpeak", PhotonNetwork.MasterClient); // ActorNumber�� �������� ��Ʈ
    }

    // ������ Ŭ���̾�Ʈ���� �߾��� ����
    [PunRPC]
    public void ChangeSpeak()
    {
        playerIndex = (++playerIndex) % (PhotonNetwork.CurrentRoom.Players.Count);
        NextPlayerSpeakData(playerIndex);
        RecordPlayerVoice(playerActorNum[playerIndex], true); // ��ǥ�� TransmitEnabled on

        for (int i = 0; i < PhotonNetwork.CurrentRoom.Players.Count; i++)
        {
            if (playerIndex != i)
            {
                RecordPlayerVoice(playerActorNum[i], true); // // ��û�ϴ� ��� TransmitEnabled false
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