using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using static UnityEditor.PlayerSettings;

public class Chair : MonoBehaviour, IPunObservable
{
    public bool sitting;
    public string playerName;

    public bool Sitting(string playerName)
    {
        if (!sitting)
        {
            sitting = true;
            this.playerName = playerName;
            return true;
        }

        return false;
    }

    public void Eixt()
    {
        sitting = false;
        playerName = null;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 만일, 데이터를 서버에 전송(PhotonView.IsMine == true)하는 상태라면...
        if (stream.IsWriting)
        {
            stream.SendNext(sitting);
            stream.SendNext(playerName);
        }
        // 그렇지 않고, 만일 데이터를 서버로부터 읽어야하는 상태라면...
        else if (stream.IsReading)
        {
            sitting = (bool)stream.ReceiveNext();
            playerName = (string)stream.ReceiveNext();
        }
    }
}
