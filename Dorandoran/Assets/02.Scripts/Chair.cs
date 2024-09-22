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
        // 만일, 데이터를 서버에 전송(PhotonView.IsMine == true)하는 상태라면...
        if (stream.IsWriting)
        {
            stream.SendNext(sitting);
            stream.SendNext(playerName);
            stream.SendNext(id);
        }
        // 그렇지 않고, 만일 데이터를 서버로부터 읽어야하는 상태라면...
        else if (stream.IsReading)
        {
            sitting = (bool)stream.ReceiveNext();
            playerName = (string)stream.ReceiveNext();
            id = (int)stream.ReceiveNext();
        }
    }
}
