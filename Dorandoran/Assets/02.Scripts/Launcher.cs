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

        [Tooltip("The Ui Panel to let ther user enter name, connect and play")]
        [SerializeField]
        private GameObject controlPanel;
        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        [SerializeField]
        private GameObject progressLabel;

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            // PhotonNetwork.AutomaticallySyncScene�� true�� ��� - MasterClient��  PhotonNetwork.LoadLevel()�� ȣ�� ����
            // �� �÷��̾���� ������ ������ �ڵ����� �ε�
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        private void Start()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);

            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }

        #endregion

        #region Public Methods

        // Photon Cloud�� ����Ǵ� ���� ����
        public void Connect()
        {
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);
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
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        }
        #endregion

        // OnJoinedRoom �Լ��� ������ ��� CreateRoom �Լ� ȣ��
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("PUN Bascis Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one. \nCalling: PhotonNetwork.CreateRoom");

            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }


        public override void OnJoinedRoom()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");

            // ù ��° �÷��̾��� ��쿡�� ����
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("We load the 'Room for 1' ");

                PhotonNetwork.LoadLevel("Room For 1");
            }
        }

    }
}
