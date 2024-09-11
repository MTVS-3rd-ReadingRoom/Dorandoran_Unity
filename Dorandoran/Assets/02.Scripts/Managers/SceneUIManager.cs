using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Voice.Unity;
using Photon.Voice.PUN;

public class SceneUIManager : MonoBehaviourPunCallbacks
{
    public Photon.Voice.Unity.Recorder recorder;

    public TextMeshProUGUI orderText;
    public static SceneUIManager instance;

    public TextMeshProUGUI timeText;
    private double startTime;
    public double timeDuration = 120.0f;
    bool isRunning = false;

    int minute = 0;
    int second = 0;

    float maxTime;

    int Order;
    int playerN = 0;

    bool bStartTime = false;

    #region Panel
    [Header("����UI")]
    public GameObject panel_Order;
    public GameObject panel_Timer;
    public Button button_Next;
    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // �� Ŭ���̾�Ʈ����
    // isMine�� �ؼ� ��������.
    // time�� speakerOrder�� �����ָ� �ȴ�.

    // Start is called before the first frame update
    void Start()
    {
        Order = 0;
        InitUI();
    }

    [PunRPC]
    void StartTimer()
    {
        startTime = PhotonNetwork.Time;
    }

    [PunRPC]
    public void ResetTimer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // ���� �ð� �������� �ٽ� ����
            photonView.RPC("StartTimer", RpcTarget.AllBuffered);
            // ��� Ŭ���̾�Ʈ���� �ٽ� ���Ӱ� Ÿ�̸� ���� �ð� ����
        }
    }

    // �ܺο��� ���� ȣ��
    public void TriggerReset()
    {
        // ������ Ŭ���̾�Ʈ���� ResetTimer ȣ��
        photonView.RPC("ResetTimer", RpcTarget.MasterClient);
    }

    public void InitUI()
    {
        button_Next.onClick.AddListener(NextOrderMessage);
    }

    public void UpdateTime()
    {


    }

    // Update is called once per frame
    void Update()
    {
        orderText.text = Order + "��° ��ǥ �����Դϴ�.\n + ActorNumber: " + PhotonNetwork.LocalPlayer.ActorNumber;
        if (isRunning && startTime > 0)
        {
            double elapsed = PhotonNetwork.Time - startTime;
            double remainingTime = timeDuration - elapsed;

            timeText.text = (int)(remainingTime / 60) + "�� " + (int)(remainingTime % 60) + "�� / 2�� ���ѽð�";

            // �ð��� ���� or (Ű ���� && ���� ������ �÷��̾ ������ ���) 0, ��ȸ��, 2, 3 ~ �÷��̾� ��
            if (remainingTime < 0)
            {
                NextOrderPlayer();
            }
        }
    }

    void NextOrderMessage()
    {
        // ��ư�� ������
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        Debug.Log("playerSpeak: " + (Order + 1) + "ActorNumber: " + actorNumber);
        if (actorNumber == (Order + 1) && photonView.IsMine) // ��ǥ�� �ϰ� �ִ� ����� && ���� �� �ڽŸ�
        {
            NextOrderPlayer();
        }
    }

    public void NextOrderPlayer()
    {
        TriggerReset();
        RPCNextOrder();
        RPCSetTransmit();
    }

    // ��ư ����ȭ - ��ư�� �������� isMine�� ����ڿ��� ��� �ش� �Լ��� �����ϵ��� ����
    [PunRPC] // all�� �Ѹ��� isMine���� 1���� ����ǵ��� ����
    public void NextOrder()
    {
        if(!isRunning) // ���� ���� ���� �ƴϸ� ���� ������ ����
        {
            isRunning = true;
        }

        // ���⼭ ��� Ŭ���̾�Ʈ ��ǻ�Ϳ��� �ش� �Լ��� ȣ��������Ѵ�.
        playerN = PhotonNetwork.CurrentRoom.Players.Count;
        if (0 == Order) // 1���� ��� ��ȸ��
        {
            panel_Timer.SetActive(false);
            // orderText.text = "AI ��ȸ�� �ð��Դϴ�.";
        }
        else // speakerOrder: 0, 2, 3, 
        {
            panel_Timer.SetActive(true);
            // orderText.text = (Order).ToString() + "��° ��ǥ�� �߾� �ð��Դϴ�.";
        }
        Order++;
        if (Order != 0)
            Order = (Order % (playerN));

        Debug.Log(Order + "���� ��ǥ ����");
        Debug.Log(playerN + "�� ����");
    }

    public void RPCNextOrder()
    {
        if(photonView.IsMine) // ����������?
        {
            photonView.RPC("NextOrder", RpcTarget.All);
        }
    }

    [PunRPC]
    public void Toggle(bool toggle)
    {
        recorder.TransmitEnabled = toggle;
    }

    public void RPCSetTransmit()
    {
        if (photonView.IsMine) // ����������?
        {
            playerN = PhotonNetwork.CurrentRoom.Players.Count;

            photonView.RPC("Toggle", PhotonNetwork.CurrentRoom.GetPlayer(Order + 1), true);
            for(int i = 0; i < playerN; i++)
            {
                if(i != (Order + 1))
                {
                    photonView.RPC("Toggle", PhotonNetwork.CurrentRoom.GetPlayer(i), false);
                }
            }
        }
    }
}
