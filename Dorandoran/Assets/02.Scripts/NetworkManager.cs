using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Reflection;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public enum RoomType
    {
        Room0,
        Room1,
        Room2,
        RoomType_End
    };

    public RoomType RoomTypeData;
    public Text StatusText;
    public InputField roomInput, NickNameInput;
    public Dropdown filterDropDown;
    public Dropdown mapDropDown;
    public Dropdown roomInputDropDown;

    int roomInputN = 0;

    string roomName;
    void Awake()
    {
        Screen.SetResolution(960, 540, false);
    }

    public void StartLogin()
    {
        // 접속을 위한 설정

    }
    void Update()
    {
        StatusText.text = PhotonNetwork.NetworkClientState.ToString();
    }

    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        //  마스터 서버에 접속 완료

        print(MethodInfo.GetCurrentMethod().Name + "is Call!");

        // 서버의 로비로 접속
        PhotonNetwork.JoinLobby();

        // PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
    }



    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("연결끊김");
    }



    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        // 서버 로비에 들어갔음을 알려준다.
        print(MethodInfo.GetCurrentMethod().Name + "is Call!");
        LoginUIController.LoginUI.ShowMakeRoomPanel();
    }



    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(roomInput.text, new RoomOptions { MaxPlayers = roomInputN });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");

        // 첫 번째 플레이어일 경우에만 실행
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("We load the 'Room for 1' ");

            CheckDropdownBox();
            PhotonNetwork.LoadLevel(roomName);
        }

        print("방참가완료");
    }

    void CheckDropdownBox()
    {
        RoomTypeData = (RoomType)mapDropDown.value;
        roomInputN = roomInputDropDown.value;

        switch (RoomTypeData)
        {
            case RoomType.Room0:
                roomName = "Room For 1";
                break;
            case RoomType.Room1:
                roomName = "Room For 2";
                break;
            case RoomType.Room2:
                roomName = "Room For 3";
                break;

        }
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(roomInput.text);

        OnJoinedRoom();
    }

    public void JoinOrCreateRoom() => PhotonNetwork.JoinOrCreateRoom(roomInput.text, new RoomOptions { MaxPlayers = 2 }, null);

    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();

    public void LeaveRoom() => PhotonNetwork.LeaveRoom();

    public override void OnCreatedRoom() => print("방만들기완료");

    // public override void OnJoinedRoom() => print("방참가완료");

    public override void OnCreateRoomFailed(short returnCode, string message) => print("방만들기실패");

    public override void OnJoinRoomFailed(short returnCode, string message) => print("방참가실패");

    public override void OnJoinRandomFailed(short returnCode, string message) => print("방랜덤참가실패");



    [ContextMenu("정보")]
    void Info()
    {
        if (PhotonNetwork.InRoom)
        {
            print("현재 방 이름 : " + PhotonNetwork.CurrentRoom.Name);
            print("현재 방 인원수 : " + PhotonNetwork.CurrentRoom.PlayerCount);
            print("현재 방 최대인원수 : " + PhotonNetwork.CurrentRoom.MaxPlayers);

            string playerStr = "방에 있는 플레이어 목록 : ";
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) playerStr += PhotonNetwork.PlayerList[i].NickName + ", ";
            print(playerStr);
        }
        else
        {
            print("접속한 인원 수 : " + PhotonNetwork.CountOfPlayers);
            print("방 개수 : " + PhotonNetwork.CountOfRooms);
            print("모든 방에 있는 인원 수 : " + PhotonNetwork.CountOfPlayersInRooms);
            print("로비에 있는지? : " + PhotonNetwork.InLobby);
            print("연결됐는지? : " + PhotonNetwork.IsConnected);
        }
    }
}
