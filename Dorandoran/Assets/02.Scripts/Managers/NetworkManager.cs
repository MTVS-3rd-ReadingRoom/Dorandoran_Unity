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
        // ������ ���� ����

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

        //  ������ ������ ���� �Ϸ�

        print(MethodInfo.GetCurrentMethod().Name + "is Call!");

        // ������ �κ�� ����
        PhotonNetwork.JoinLobby();

        // PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
    }



    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("�������");
    }



    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        // ���� �κ� ������ �˷��ش�.
        print(MethodInfo.GetCurrentMethod().Name + "is Call!");
        // LoginUIController.LoginUI.ShowMakeRoomPanel();
    }



    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(roomInput.text, new RoomOptions { MaxPlayers = roomInputN });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");

        // ù ��° �÷��̾��� ��쿡�� ����
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("We load the 'Room for 1' ");

            CheckDropdownBox();
            // PhotonNetwork.LoadLevel(roomName);
        }

        print("�������Ϸ�");
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

    public override void OnCreatedRoom() => print("�游���Ϸ�");

    // public override void OnJoinedRoom() => print("�������Ϸ�");

    public override void OnCreateRoomFailed(short returnCode, string message) => print("�游������");

    public override void OnJoinRoomFailed(short returnCode, string message) => print("����������");

    public override void OnJoinRandomFailed(short returnCode, string message) => print("�淣����������");



    [ContextMenu("����")]
    void Info()
    {
        if (PhotonNetwork.InRoom)
        {
            print("���� �� �̸� : " + PhotonNetwork.CurrentRoom.Name);
            print("���� �� �ο��� : " + PhotonNetwork.CurrentRoom.PlayerCount);
            print("���� �� �ִ��ο��� : " + PhotonNetwork.CurrentRoom.MaxPlayers);

            string playerStr = "�濡 �ִ� �÷��̾� ��� : ";
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) playerStr += PhotonNetwork.PlayerList[i].NickName + ", ";
            print(playerStr);
        }
        else
        {
            print("������ �ο� �� : " + PhotonNetwork.CountOfPlayers);
            print("�� ���� : " + PhotonNetwork.CountOfRooms);
            print("��� �濡 �ִ� �ο� �� : " + PhotonNetwork.CountOfPlayersInRooms);
            print("�κ� �ִ���? : " + PhotonNetwork.InLobby);
            print("����ƴ���? : " + PhotonNetwork.IsConnected);
        }
    }
}
