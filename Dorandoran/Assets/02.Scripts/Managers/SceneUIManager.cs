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
    int playerIndex = 0;
    int playerN = 0;

    bool bStartTime = false;

    public TextMeshProUGUI logText;

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
        playerIndex = 0;
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
        logText.text = Order + "��° ��ǥ �����Դϴ�.\n + ���� ActorNumber: " + PhotonNetwork.LocalPlayer.ActorNumber;
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
        Debug.Log("playerSpeak: " + Order + "ActorNumber: " + actorNumber);
        if (actorNumber == Order || Order <= 0)
            NextOrderPlayer();
    }

    public void NextOrderPlayer()
    {
        TriggerReset();
        RPCNextOrder();
        AllMuteTransmit();
        RPCSetTransmit();
    }

    // ��ư ����ȭ - ��ư�� �������� isMine�� ����ڿ��� ��� �ش� �Լ��� �����ϵ��� ����
    [PunRPC] // all�� �Ѹ��� isMine���� 1���� ����ǵ��� ����
    public void NextOrder()
    {
        ++Order;
        Order = (Order % (playerN + 1)); // 0 ~ 2
        if (!isRunning) // ���� ���� ���� �ƴϸ� ���� ������ ����
        {
            isRunning = true;
            Order = 0;
        }
        // ���⼭ ��� Ŭ���̾�Ʈ ��ǻ�Ϳ��� �ش� �Լ��� ȣ��������Ѵ�.
        playerN = PhotonNetwork.CurrentRoom.Players.Count;
        if (0 == Order) // 1���� ��� ��ȸ��
        {
            panel_Timer.SetActive(false);
            orderText.text = "AI ��ȸ�� �ð��Դϴ�.";
        }
        else
        {
            panel_Timer.SetActive(true);
            orderText.text = (Order).ToString() + "��° ��ǥ�� �߾� �ð��Դϴ�.";
        }

        Debug.Log(Order + "���� ��ǥ ����");
        Debug.Log(playerN + "�� ����");
    }

    public void RPCNextOrder()
    {
        photonView.RPC("NextOrder", RpcTarget.All);
    }

    [PunRPC]
    public void Toggle(bool toggle)
    {
        recorder.TransmitEnabled = toggle;
    }

    public void AllMuteTransmit()
    {
        playerN = PhotonNetwork.CurrentRoom.Players.Count;

        for (int i = 1; i < playerN + 1; i++)
        {
            photonView.RPC("Toggle", PhotonNetwork.CurrentRoom.GetPlayer(i), false);
        }
    }

    public void RPCSetTransmit()
    {
        photonView.RPC("Toggle", PhotonNetwork.CurrentRoom.GetPlayer(Order), true);
    }
}
