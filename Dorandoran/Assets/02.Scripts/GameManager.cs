using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;

namespace Com.DoranDoran.ReadingRoom
{
    // ���� �Ŵ��� ������

    public class GameManager : MonoBehaviourPunCallbacks
    {

        #region Photon Callbacks

        public override void OnLeftRoom()
        {
            // Build Setting - 0�� �ε��� �κ� ���� Launcher�� �ε�
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

        // ���ϴ� ������ �ε��ϴ� ���
        void LoadArea()
        {
            // LoadLevel �Լ��� ������ Ŭ���̾�Ʈ�� �ƴ� ��쿡�� ȣ��Ǿ�� �ϱ� ������
            // IsMasterClient�� �̿��ؼ� ������ Ŭ���̾�Ʈ���� üũ
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
                return;
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);

            // ���ϴ� ������ �ε�
            PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);

            // automaticallySyncScene�� ����ϵ��� ������ ����
            // �� �ȿ� ������ ��� Ŭ���̾�Ʈ�� ���� ���� �ε带 ����Ƽ�� �ϴ� ��X, Photon�� �ϵ��� ����
        }

        #endregion


        // GameManager�� �÷��̾� ���� �� ������ Listen
        #region Photon Callbacks

        // �÷��̾���� �����Ҷ����� ����
        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName);

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);

                LoadArea();
            }
        }

        // �÷��̾���� ���������� ����
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