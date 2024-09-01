using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;

namespace Com.DoranDoran.ReadingRoom
{
    // 게임 매니저 프리팹

    public class GameManager : MonoBehaviourPunCallbacks
    {

        #region Photon Callbacks

        public override void OnLeftRoom()
        {
            // Build Setting - 0번 인덱스 로비 씬인 Launcher를 로드
            SceneManager.LoadScene(0);
        }
         
        #endregion

        #region Public Methods

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        #endregion

        #region Private Methods

        // 원하는 레벨을 로드하는 기능
        void LoadArea()
        {
            // LoadLevel 함수는 마스터 클라이언트가 아닐 경우에만 호출되어야 하기 때문에
            // IsMasterClient를 이용해서 마스터 클라이언트인지 체크
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
                return;
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);

            // 원하는 레벨을 로드
            PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);

            // automaticallySyncScene를 사용하도록 이전에 설정
            // 룸 안에 접속한 모든 클라이언트에 대해 레벨 로드를 유니티가 하는 것X, Photon이 하도록 구현
        }

        #endregion


        // GameManager가 플레이어 연결 및 해제를 Listen
        #region Photon Callbacks

        // 플레이어들이 참여할때마다 통지
        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName);

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);

                LoadArea();
            }
        }

        // 플레이어들이 떠날때마다 통지
        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName);

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);

                LoadArea();
            }
        }

        #endregion
    }
}