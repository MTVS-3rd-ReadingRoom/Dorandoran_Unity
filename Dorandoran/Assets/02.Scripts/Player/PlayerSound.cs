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

    // ������ Ŭ���̾�Ʈ
    // ���� ��� Ŭ���̾�Ʈ ���̵� ����
    // ����ũ ���� ���̵� ���� ����
    // ����ũ ���̵� ��ư�� ������ ����

    // Start is called before the first frame update
    void Start()
    {
        // ���� ������ ID ��������
        string currentUserId = PhotonNetwork.LocalPlayer.UserId;

        // ���� ID ��
        if (allowedUserIds.Contains(currentUserId))
        {
            // ���� ������� Ư�� ��� Ȱ��ȭ
            
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
        if(PhotonNetwork.IsMasterClient) // ���常 ���� �����ϵ��� 
        {
            // RPC_UpdateSound(speakId++);
        }
    }


    void UpdateSound(int SoundPlayerid)
    {
        //// �ְ�
        //Dictionary<int, Player> playerDict = PhotonNetwork.CurrentRoom.Players;
        
        //foreach (KeyValuePair<int, Player> player in playerDict)
        //{
        //    userIds.Add(player.Value.UserId);
        //}

        //// Ȯ��
        //if (userIds[SoundPlayerid] == pv.Owner.UserId) // �� �÷��̾� id
        //{
        //    recorder.TransmitEnabled = true; // ���� ���� ����
        //}
        //else // ų �÷��̾� id
        //{
        //    recorder.TransmitEnabled = false; // ���� ���� ����x
        //}

        //// �����.
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
