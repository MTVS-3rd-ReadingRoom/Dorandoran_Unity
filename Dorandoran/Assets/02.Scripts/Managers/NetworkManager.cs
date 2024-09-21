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
        // 접속을 위한 설정
        if (DataManager.instance.nickName.Length > 0)
        {
            PhotonNetwork.GameVersion = "1.0.0";
            PhotonNetwork.NickName = DataManager.instance.nickName;
            PhotonNetwork.AutomaticallySyncScene = true;

            // 접속을 서버에 요청하기
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnected()
    {
        base.OnConnected();

        // 네임 서버에 접속이 완료되었음을 알려준다.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);

        // 실패 원인을 출력한다.
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

        //  마스터 서버에 접속 완료

        print(MethodInfo.GetCurrentMethod().Name + "is Call!");

        // 서버의 로비로 접속
        PhotonNetwork.JoinLobby();

        // PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        // 서버 로비에 들어갔음을 알려준다.
        print(MethodInfo.GetCurrentMethod().Name + "is Call!");
        // LoginUIController.LoginUI.ShowMakeRoomPanel();
    }

    public void CreateRoom()
    {
        string roomName = roomInput.text;
        int maxPlayer = roomInputDropDown.value + 1;

        if (roomName.Length > 0 && maxPlayer >= 1)
        {
            // 나의 룸을 만든다.
            RoomOptions roomOpt = new RoomOptions();
            roomOpt.MaxPlayers = maxPlayer;
            roomOpt.IsOpen = true;
            roomOpt.IsVisible = true;

            // 룸의 커스텀 정보를 추가한다.
            // 키 값 등록하기
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

        PhotonNetwork.LoadLevel(1); // 1번 빌드 셋팅으로 고정 이동
        print("방 참가완료");
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

        // 룸에 입장이 실패한 이유를 출력한다.
        Debug.LogError(message);

        switch(message)
        {

        }
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();

        // 성공적으로 방이 개설되었음을 알려준다.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");

    }

    // 현재 로비에서 룸의 변경사항을 알려주는 콜백 함수
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        foreach (RoomInfo room in roomList)
        {
            // 만일, 갱신된 룸 정보가 제거 리스트에 있다면..
            if (room.RemovedFromList) // true일 경우 제거될 정보이다.
            {
                // cashedRoomList에서 해당 룸을 제거한다.
                cashedRoomList.Remove(room);
            }
            else
            {
                // 만일, 이미 cashedRoomList 에 있는 방이라면...
                if (cashedRoomList.Contains(room))
                {
                    // 기존 룸 정보를 제거한다.
                    cashedRoomList.Remove(room);
                }

                // 새 룸을 cachedRoomList에 추가한다.
                cashedRoomList.Add(room);
            }
        }

        // 기존의 모든 방 정보를 삭제한다.
        for (int i = 0; i < scrollContent.childCount; i++)
        {
            Destroy(scrollContent.GetChild(i).gameObject);
        }


        foreach (RoomInfo room in roomList)
        {
            // cachedRoomList에 있는 모든 방을 만들어서 스크롤뷰에 추가한다.
            GameObject go = Instantiate(roomPrefab, scrollContent);
            RoomPanel roomPanel = go.GetComponent<RoomPanel>();
            roomPanel.SetRoomInfo(room);
            //// 버튼에 방 입장 기능 연결하기
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



    // public override void OnJoinedRoom() => print("방참가완료");

    public override void OnCreateRoomFailed(short returnCode, string message) => print("방만들기실패");

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

    public void JoinRoom()
    {
        // Join 관련 패널을 활성화한다.
        ChangePanel(1, 2);
    }

    /// <summary>
    /// 패널의 변경을 하기 위한 함수
    /// </summary>
    /// <param name="offIndex">꺼여될 패널 인덱스</param>
    /// <param name="onIndex">켜야될 패널 인덱스</param>

    void ChangePanel(int offIndex, int onIndex)
    {
        panelList[offIndex].SetActive(false);
        panelList[onIndex].SetActive(true);
    }

    private void OnDestroy()
    {
        Debug.Log("네트워크 매니저 삭제");
    }
}
