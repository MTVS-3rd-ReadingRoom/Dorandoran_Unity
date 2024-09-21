using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Reflection;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public enum RoomType
    {
        Room0,
        Room1,
        Room2,
        RoomType_End
    };

    public static NetworkManager instance;

    public RoomType RoomTypeData;
    public Text StatusText;
    public InputField roomInput, NickNameInput;
    public Dropdown filterDropDown;
    public Dropdown bookDropDown;
    public Dropdown mapDropDown;
    public Dropdown roomInputDropDown;

    int roomInputN = 0;
    int roomIndex = 0;
    string roomName;

    public GameObject roomPrefab;
    public Transform scrollContent;
    public GameObject[] panelList;

    List<RoomInfo> cashedRoomList = new List<RoomInfo>();

    int playerID = -1;


    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            Screen.SetResolution(1920, 1080, false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartLogin()
    {
        // ������ ���� ����
        if (DataManager.instance.nickName.Length > 0)
        {
            PhotonNetwork.GameVersion = "1.0.0";
            PhotonNetwork.NickName = DataManager.instance.nickName;
            PhotonNetwork.AutomaticallySyncScene = true;

            // ������ ������ ��û�ϱ�
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnected()
    {
        base.OnConnected();

        // ���� ������ ������ �Ϸ�Ǿ����� �˷��ش�.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);

        // ���� ������ ����Ѵ�.
        Debug.LogError("Disconnected from Server - " + cause);
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

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        // ���� �κ� ������ �˷��ش�.
        print(MethodInfo.GetCurrentMethod().Name + "is Call!");
        // LoginUIController.LoginUI.ShowMakeRoomPanel();
    }

    public void CreateRoom()
    {
        string roomName = roomInput.text;
        int maxPlayer = roomInputDropDown.value + 1;

        if (roomName.Length > 0 && maxPlayer >= 1)
        {
            // ���� ���� �����.
            RoomOptions roomOpt = new RoomOptions();
            roomOpt.MaxPlayers = maxPlayer;
            roomOpt.IsOpen = true;
            roomOpt.IsVisible = true;

            // ���� Ŀ���� ������ �߰��Ѵ�.
            // Ű �� ����ϱ�
            roomOpt.CustomRoomPropertiesForLobby = new string[] { "MASTER_NAME", "PASSWORD" };
            Hashtable roomTable = new Hashtable();
            roomTable.Add("MASTER_NAME", PhotonNetwork.NickName);
            roomTable.Add("PASSWORD", 1234);
            roomOpt.CustomRoomProperties = roomTable;

            PhotonNetwork.CreateRoom(roomName, roomOpt, TypedLobby.Default);
        }


        // PhotonNetwork.CreateRoom(roomInput.text, new RoomOptions { MaxPlayers = roomInputDropDown.value + 1 });
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }




    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");

        PhotonNetwork.LoadLevel(1); // 1�� ���� �������� ���� �̵�
        print("�� �����Ϸ�");
    }

    public void SetPlayerID()
    {

    }

    void CheckDropdownBox()
    {
        RoomTypeData = (RoomType)mapDropDown.value;

    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);

        // �뿡 ������ ������ ������ ����Ѵ�.
        Debug.LogError(message);

        switch(message)
        {

        }
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();

        // ���������� ���� �����Ǿ����� �˷��ش�.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");

    }

    // ���� �κ񿡼� ���� ��������� �˷��ִ� �ݹ� �Լ�
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        foreach (RoomInfo room in roomList)
        {
            // ����, ���ŵ� �� ������ ���� ����Ʈ�� �ִٸ�..
            if (room.RemovedFromList) // true�� ��� ���ŵ� �����̴�.
            {
                // cashedRoomList���� �ش� ���� �����Ѵ�.
                cashedRoomList.Remove(room);
            }
            else
            {
                // ����, �̹� cashedRoomList �� �ִ� ���̶��...
                if (cashedRoomList.Contains(room))
                {
                    // ���� �� ������ �����Ѵ�.
                    cashedRoomList.Remove(room);
                }

                // �� ���� cachedRoomList�� �߰��Ѵ�.
                cashedRoomList.Add(room);
            }
        }

        // ������ ��� �� ������ �����Ѵ�.
        for (int i = 0; i < scrollContent.childCount; i++)
        {
            Destroy(scrollContent.GetChild(i).gameObject);
        }


        foreach (RoomInfo room in roomList)
        {
            // cachedRoomList�� �ִ� ��� ���� ���� ��ũ�Ѻ信 �߰��Ѵ�.
            GameObject go = Instantiate(roomPrefab, scrollContent);
            RoomPanel roomPanel = go.GetComponent<RoomPanel>();
            roomPanel.SetRoomInfo(room);
            //// ��ư�� �� ���� ��� �����ϱ�
            roomPanel.btn_join.onClick.AddListener(() =>
            {
                PhotonNetwork.JoinRoom(room.Name);
            });

        }
    }

    public void SetBookList(List<BookUI> bookList)
    {
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        foreach (var item in bookList)
        {
            options.Add(new Dropdown.OptionData(item.name));
        }
        bookDropDown.options = options;
    }

    public string GetBook()
    {
        return bookDropDown.options[bookDropDown.value].text;
    }

    //public void JoinOrCreateRoom() => PhotonNetwork.JoinOrCreateRoom(roomInput.text, new RoomOptions { MaxPlayers = 2 }, null);

    //public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();

    //public void LeaveRoom() => PhotonNetwork.LeaveRoom();



    // public override void OnJoinedRoom() => print("�������Ϸ�");

    public override void OnCreateRoomFailed(short returnCode, string message) => print("�游������");

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

    public void JoinRoom()
    {
        // Join ���� �г��� Ȱ��ȭ�Ѵ�.
        ChangePanel(1, 2);
    }

    /// <summary>
    /// �г��� ������ �ϱ� ���� �Լ�
    /// </summary>
    /// <param name="offIndex">������ �г� �ε���</param>
    /// <param name="onIndex">�Ѿߵ� �г� �ε���</param>

    void ChangePanel(int offIndex, int onIndex)
    {
        panelList[offIndex].SetActive(false);
        panelList[onIndex].SetActive(true);
    }

    private void OnDestroy()
    {
        Debug.Log("��Ʈ��ũ �Ŵ��� ����");
    }
}
