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
        // ����, �����͸� ������ ����(PhotonView.IsMine == true)�ϴ� ���¶��...
        if (stream.IsWriting)
        {
            stream.SendNext(sitting);
            stream.SendNext(playerName);
        }
        // �׷��� �ʰ�, ���� �����͸� �����κ��� �о���ϴ� ���¶��...
        else if (stream.IsReading)
        {
            sitting = (bool)stream.ReceiveNext();
            playerName = (string)stream.ReceiveNext();
        }
    }
}
