using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneUIManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI orderText;
    public static SceneUIManager instance;

    public TextMeshProUGUI timeText;
    private double startTime;
    public double timeDuration = 120.0f;
    bool isRunning = false;

    int minute = 0;
    int second = 0;

    float maxTime;

    int speakerOrder = 0;
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
        if (isRunning && startTime > 0)
        {
            double elapsed = PhotonNetwork.Time - startTime;
            double remainingTime = timeDuration - elapsed;

            timeText.text = (int)(remainingTime / 60) + "�� " + (int)(remainingTime % 60) + "�� / 2�� ���ѽð�";

            if (remainingTime < 0)
            {
                NextOrderMessage(); // ���� ���ʷ�
                TriggerReset();
            }
        }
    }

    void NextOrderMessage()
    {
        TriggerReset();
        RPCNextOrder();
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
        if (speakerOrder + 1 > playerN) // ��ǥ�ں��� ũ��
        {
            panel_Timer.SetActive(false);
            orderText.text = "AI ��ȸ�� �ð��Դϴ�.";
            speakerOrder = 0;

        }
        else
        {
            panel_Timer.SetActive(true);
            orderText.text = (speakerOrder + 1).ToString() + "��° ��ǥ�� �߾� �ð��Դϴ�.";
            speakerOrder++; 
        }
    }

    public void RPCNextOrder()
    {
        if(photonView.IsMine)
        {
            photonView.RPC("NextOrder", RpcTarget.All);
        }
    }
}
