using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;

public class PlayerProsAndCons : MonoBehaviour, IPunObservable
{
    public TMP_Text text_ProsAndCons;
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
        curDebatePosition = (int)DebatePosition.DebatePositionEnd;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDebatePosition();
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
            
        }
        // �׷��� �ʰ�, ���� �����͸� �����κ��� �о���ϴ� ���¶��...
        else if (stream.IsReading)
        {
            curDebatePosition = (int)stream.ReceiveNext();
            bOnDebatePosition = (bool)stream.ReceiveNext();
        }
    }
}
