using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Reflection;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;
using UnityEngine.Rendering.LookDev;
using UnityEngine.SceneManagement;
using Photon.Voice.PUN;
using Photon.Voice.Unity;

public class SceneNetworkManager : MonoBehaviourPunCallbacks
{
    public static SceneNetworkManager instance;

    public TMP_Text[] tmpList;
    public int[] actorList = new int[4];
    PhotonView pv;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    void Update()
    {

    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            DisconnectPlayer();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) // 입장 - 전체적으로 갱신
    {
        UpdatePlayerList();
        SetPlayerNickNameRPC();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        JoinRoom();
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        JoinRoom();
    }

    public void JoinRoom()
    {
        UpdatePlayerList();
        SetPlayerNickNameRPC();
    }

    public void UpdatePlayerList()
    {
        int i = 0;
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            tmpList[i].text = player.Value.NickName;
            actorList[i] = player.Value.ActorNumber;
            i++;
        }

        for (int j = i; j < 4; j++)
        {
            actorList[j] = -1;
        }
}


[PunRPC]
    public void SetPlayerNickName(string text1, string text2, string text3, string text4, int actor1, int actor2, int actor3, int actor4)
    {
        tmpList[0].text = text1;
        tmpList[1].text = text2;
        tmpList[2].text = text3;
        tmpList[3].text = text4;

        actorList[0] = actor1;
        actorList[1] = actor2;
        actorList[2] = actor3;
        actorList[3] = actor4;
    }

    public void SetPlayerNickNameRPC()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            pv.RPC("SetPlayerNickName", RpcTarget.All, tmpList[0].text, tmpList[1].text, tmpList[2].text, tmpList[3].text, actorList[0], actorList[1], actorList[2], actorList[3]);
        }
    }

    public override void OnLeftRoom()
    {
        SceneReloadManager.instance.NextScene();
        PhotonNetwork.LoadLevel(0);
    }         

    public void OnCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void DisconnectPlayer()
    {
        OnCursor();
        PhotonNetwork.LeaveRoom();
    }

}