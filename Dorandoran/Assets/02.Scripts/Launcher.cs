using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Com.DoranDoran.ReadingRoom
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;
        #region Private Serializable Fields

        #endregion

        #region Private Fields

        string gameVersion = "1";

        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            // PhotonNetwork.AutomaticallySyncScene이 true일 경우 - MasterClient는  PhotonNetwork.LoadLevel()을 호출 가능
            // 및 플레이어들은 동일한 레벨을 자동으로 로드
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        private void Start()
        {
            //Connect();
        }

        #endregion

        #region Public Methods

        // Photon Cloud에 연결되는 시작 지점
        public void Connect()
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        #endregion

        #region MonoBehaviourPunCallbacks Callbacks

        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMater() was called by PUN");

            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        }
        #endregion

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("PUN Bascis Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one. \nCalling: PhotonNetwork.CreateRoom");

            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        }

    }
}
