using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;

public class PlayerProsAndCons : MonoBehaviour, IPunObservable
{
    PhotonView pv;

    public TMP_Text text_ProsAndCons;
    public TMP_Text text_NickName;

    string playerNickName;
    bool bOnDebatePosition = false;
    public enum DebatePosition
    {
        Pro, // ����
        Con, // �ݴ�
        DebatePositionEnd
    }

    // ���� ��� ���� ����
    int curDebatePosition;

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        if (pv.IsMine)
        {
            curDebatePosition = (int)DebatePosition.DebatePositionEnd;

        }

            //PhotonNetwork.LocalPlayer.NickName;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDebatePosition();
        UpdatePlayerNickName();
    }

    void UpdateDebatePosition()
    {
        if (bOnDebatePosition)
        {
            switch ((DebatePosition)curDebatePosition)
            {
                case DebatePosition.Pro:
                    text_ProsAndCons.text = "����";
                    text_ProsAndCons.color = new Color(0.0f, 0.0f, 255.0f);
                    break;
                case DebatePosition.Con:
                    text_ProsAndCons.text = "�ݴ�";
                    text_ProsAndCons.color = new Color(255.0f, 0.0f, 0.0f);
                    break;

            }
        }
        else
        {
            text_ProsAndCons.text = "";
        }
    }


    public void OnDebatePosition(bool bDrawDebatePosition)
    {
        bOnDebatePosition = bDrawDebatePosition;
    }

    public void UpdatePlayerNickName()
    {
        if (pv.IsMine)
        {
            playerNickName = PhotonNetwork.LocalPlayer.NickName;
        }

        text_NickName.text = playerNickName;
    }

    // text_ProsAndCons
    // ���� �÷��̾��� ���� ���ϱ�
    public void SetCurPlayerProsAndConsData(int debatePosition)
    {
        if (curDebatePosition == debatePosition)
            return;
        curDebatePosition = debatePosition;
        ChatManager.chatManager.SetCurChatProsAndConsData((DebatePosition)debatePosition);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // ����, �����͸� ������ ����(PhotonView.IsMine == true)�ϴ� ���¶��...
        if (stream.IsWriting)
        {
            // iterable �����͸� ������.
            stream.SendNext((int)curDebatePosition);
            stream.SendNext(bOnDebatePosition);
            stream.SendNext(playerNickName);
            
        }
        // �׷��� �ʰ�, ���� �����͸� �����κ��� �о���ϴ� ���¶��...
        else if (stream.IsReading)
        {
            curDebatePosition = (int)stream.ReceiveNext();
            bOnDebatePosition = (bool)stream.ReceiveNext();
            playerNickName = (string)stream.ReceiveNext();
        }
    }

    public int GetcurDebatePosition()
    {
        return curDebatePosition;
    }
}
