using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using static UnityEditor.PlayerSettings;

public class Chair : MonoBehaviourPun, IPunObservable
{
    public bool sitting;
    public string playerName;
    public int id;
    public int virtualCameraIndex;

    public bool Sitting(PhotonView pv, string playerName, int id)
    {
        if (!sitting)
        {
            photonView.TransferOwnership(pv.Owner);
            sitting = true;
            this.playerName = playerName;
            this.id = id;
            return true;
        }

        return false;
    }

    public void Eixt()
    {
        sitting = false;
        playerName = null;
        id = int.MinValue;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // ����, �����͸� ������ ����(PhotonView.IsMine == true)�ϴ� ���¶��...
        if (stream.IsWriting)
        {
            stream.SendNext(sitting);
            stream.SendNext(playerName);
            stream.SendNext(id);
        }
        // �׷��� �ʰ�, ���� �����͸� �����κ��� �о���ϴ� ���¶��...
        else if (stream.IsReading)
        {
            sitting = (bool)stream.ReceiveNext();
            playerName = (string)stream.ReceiveNext();
            id = (int)stream.ReceiveNext();
        }
    }
}
