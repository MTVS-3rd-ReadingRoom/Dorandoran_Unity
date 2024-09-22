using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Voice.Unity;
using Photon.Voice.PUN;
using Photon.Pun.Demo.PunBasics;

public class SceneUIManager : MonoBehaviourPunCallbacks
{
    enum CharacterTurn
    {
        CharacterHostTurn,
        CharacterPlayerTurn,
        CharacterDebateTurn,
        CharacterTurnEnd
    }
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

    int PlayN;
    int playerIndex = 0;

    bool bStartTime = false;

    public TextMeshProUGUI logText;

    public TextMeshProUGUI recorder_InterestText;
    CharacterTurn m_eCurCharacterTurn;

    GameManager gameManager;

    bool checkSittingPlayer;
    #region Panel
    [Header("����UI")]
    public GameObject panel_Order;
    public GameObject panel_Timer;
    public Button button_Next;
    #endregion

    // Ŭ������ ������ ����
    void NextTurn() // ������ ��
    {
        ++PlayN;
        Debug.Log("���� PlayN: " + PlayN);
        if (PlayN <= PhotonNetwork.CurrentRoom.PlayerCount) // �� �÷��̾� ���ڸ�ŭ �����ߴٸ�
        {
            m_eCurCharacterTurn = CharacterTurn.CharacterPlayerTurn;
            orderText.text = StageUIManager.instance.PrintCurrentIndex();
            AllMuteTransmit();
            RPCSetTransmit();
            SetSameSpeakGroup();
        }
        else if (PlayN == PhotonNetwork.CurrentRoom.PlayerCount + 1) // �÷��̾� ������ 1��ŭ Ŀ���� ��� �ð�
        {
            m_eCurCharacterTurn = CharacterTurn.CharacterDebateTurn;
            orderText.text = "��� �ð��Դϴ�.";
            DebatePlayer();
            PlayN = 0;
        }
    }
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
        m_eCurCharacterTurn = CharacterTurn.CharacterHostTurn;
        PlayN = 0;
        playerIndex = 0;
        checkSittingPlayer = false;

        gameManager = GameObject.Find("GameManager").GetComponentInChildren<GameManager>();
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

    public void CheckSittingPlayer()
    {
        if (!checkSittingPlayer)
        {
            if(gameManager.CheckSittingPlayer())
            {
                NextOrderMessage();
                checkSittingPlayer = true;
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        // �ɾ��ִ� �÷��̾� üũ
        CheckSittingPlayer();

        recorder_InterestText.text = "Recorder Interest Group: " + recorder.InterestGroup;

        logText.text = PlayN + "��° ��ǥ �����Դϴ�.\n + ���� ActorNumber: " + PhotonNetwork.LocalPlayer.ActorNumber + "\n"
            + "���ڴ��� �׷�: " + recorder.InterestGroup;
        if (isRunning && startTime > 0)
        {
            double elapsed = PhotonNetwork.Time - startTime;
            double remainingTime = timeDuration - elapsed;

            timeText.text = (int)(remainingTime / 60) + "�� " + (int)(remainingTime % 60) + "�� / 2�� ���ѽð�";

            // �ð��� ���� or (Ű ���� && ���� ������ �÷��̾ ������ ���) 0, ��ȸ��, 2, 3 ~ �÷��̾� ��
            if (remainingTime < 0)
            {
                NextOrderRPC();
            }
        }
    }

    void NextOrderMessage()
    {
        switch(m_eCurCharacterTurn)
        {
            case CharacterTurn.CharacterHostTurn:
                HostTurnRPC();
                TriggerReset();
                break;
            case CharacterTurn.CharacterPlayerTurn:
                NextOrderRPC();
                break;
            case CharacterTurn.CharacterDebateTurn:
                NextOrderRPC();
                break;
        }
    }

    public void DiscussPlayer()
    {
        TriggerReset();
    }

    public void NextOrderRPC()
    {
        TriggerReset();
        RPCNextOrder();
    }

    public void DebatePlayer()
    {
        AllSpeakTransmit();
        DivideTwoSpeakGroup();
    }

    public void HostTurnRPC()
    {
        photonView.RPC("HostTurn", RpcTarget.All);
    }

    [PunRPC] // all�� �Ѹ��� isMine���� 1���� ����ǵ��� ����
    public void HostTurn()
    {
        if (!isRunning) // ���� ���� ���� �ƴϸ� ���� ������ ����
            isRunning = true;
        panel_Timer.SetActive(false);
        orderText.text = "AI ��ȸ�� �ð��Դϴ�.";
        m_eCurCharacterTurn = CharacterTurn.CharacterPlayerTurn;
    }

    // ��ư ����ȭ - ��ư�� �������� isMine�� ����ڿ��� ��� �ش� �Լ��� �����ϵ��� ����
    [PunRPC] // all�� �Ѹ��� isMine���� 1���� ����ǵ��� ����
    public void NextOrder()
    {
        NextTurn();
        panel_Timer.SetActive(true);

        Debug.Log(PlayN + "���� ��ǥ ����");
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

    [PunRPC]
    public void SpeakGroup(byte groupID)
    {
        recorder.InterestGroup = groupID;            
    }

    public void SetSameSpeakGroup()
    {
        for (int i = 1; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            RPCSetSpeakGroup(i, 0);
        }
    }

    public void DivideTwoSpeakGroup()
    {
        // 1 ~ 2 - 2��
        // 1 ~ 4 - 4��
        int PlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        for (int i = 1; i < PlayerCount / 2 + 1; i++)
        {
            RPCSetSpeakGroup(i, 1);
            RPCSetSpeakGroup((i + PlayerCount / 2), 2);
        }
    }

    public void RPCSetSpeakGroup(int playerActor, byte GroupID)
    {
        photonView.RPC("SpeakGroup", PhotonNetwork.CurrentRoom.GetPlayer(playerActor), GroupID);
    }

    public void AllMuteTransmit()
    {
        for (int i = 1; i < PhotonNetwork.CurrentRoom.Players.Count + 1; i++)
        {
            photonView.RPC("Toggle", PhotonNetwork.CurrentRoom.GetPlayer(i), false);
        }
    }

    public void AllSpeakTransmit()
    {
        for (int i = 1; i < PhotonNetwork.CurrentRoom.Players.Count + 1; i++)
        {
            photonView.RPC("Toggle", PhotonNetwork.CurrentRoom.GetPlayer(i), true);
        }
    }

    public void RPCSetTransmit()
    {
        photonView.RPC("Toggle", PhotonNetwork.CurrentRoom.GetPlayer(PlayN), true);
    }
}
