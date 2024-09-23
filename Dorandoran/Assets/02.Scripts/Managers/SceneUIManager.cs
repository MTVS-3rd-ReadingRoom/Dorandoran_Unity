using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Voice.Unity;
using Photon.Voice.PUN;
using Photon.Pun.Demo.PunBasics;
using DG.Tweening.Core.Easing;


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

    public Chair announcer_Chair;
    public Chair[] propositionSide_Chair;
    public Chair[] oppositionSide_Chair;
    public List<Chair> propositionSide = new List<Chair>();
    public List<Chair> oppositionSide = new List<Chair>();

    public static SceneUIManager instance;

    public TextMeshProUGUI timeText;
    private double startTime;
    public double timeDuration = 120.0f;
    bool isRunning = false;

    int minute = 0;
    int second = 0;

    float maxTime;

    //int PlayN;
    int timelineIndex = 0;
    float[] times;
    public Chair[] speakerIdList;

    int playerIndex = 0;

    bool bStartTime = false;

    public TextMeshProUGUI logText;

    public TextMeshProUGUI recorder_InterestText;
    CharacterTurn m_eCurCharacterTurn;

    GameManager gameManager;

    bool checkSittingPlayer;
    public TMP_Text[] tmpProsAndConsList;

    SceneNetworkManager sceneNetworkManager;
    #region Panel
    [Header("����UI")]
    public GameObject panel_Order;
    public GameObject panel_Timer;
    public Button button_Next;
    #endregion

    // Ŭ������ ������ ����
    void NextTurn() // ������ ��
    {
        CinemachineManager.instance.AddInstructions();
        if (timelineIndex < times.Length) // �� �÷��̾� ���ڸ�ŭ �����ߴٸ�
        {
            print(timelineIndex);
            // orderText.text = StageUIManager.instance.PrintCurrentIndex(timelineIndex);
            if (timelineIndex == 1 || timelineIndex == 4 || timelineIndex == 7 || timelineIndex == 10)
            {
                m_eCurCharacterTurn = CharacterTurn.CharacterDebateTurn;
                DebatePlayer();
                CinemachineManager.instance.AddInstructions();
            }
            else
            {
                m_eCurCharacterTurn = CharacterTurn.CharacterPlayerTurn;
                AllMuteTransmit();
                if (speakerIdList[timelineIndex] != announcer_Chair && speakerIdList[timelineIndex] != null)
                {
                    RPCSetTransmit();
                }
                if (speakerIdList[timelineIndex] != null)
                {
                    CinemachineManager.instance.AddInstructions(speakerIdList[timelineIndex].virtualCameraIndex);
                }
                else
                {
                    CinemachineManager.instance.AddInstructions();
                }
                SetSameSpeakGroup();
            }
            timeDuration = times[timelineIndex];
            print(timelineIndex);
            timelineIndex++;
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
        timelineIndex = 0;
        playerIndex = 0;
        checkSittingPlayer = false;

        sceneNetworkManager = GameObject.Find("SceneNetworkManager").GetComponentInChildren<SceneNetworkManager>();
        gameManager = GameObject.Find("GameManager").GetComponentInChildren<GameManager>();
        InitUI();

        times = new float[]
            {
                60, 
                180, 120, 120,
                180, 120, 120,
                180, 120, 120,
                180, 120, 120,
            };
        speakerIdList = new Chair[]
            {
                announcer_Chair,
                null, null, null,
                null, null, null,
                null, null, null,
                null, null, null,
            };
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
                gameManager.SetPlayerProsAndConsText();
                checkSittingPlayer = true;
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        // �ɾ��ִ� �÷��̾� üũ
        CheckSittingPlayer();
        // �÷��̾� ���� ������ üũ
        UpdatePlayerProsAndConsList();

        recorder_InterestText.text = "Recorder Interest Group: " + recorder.InterestGroup;

        //logText.text = PlayN + "��° ��ǥ �����Դϴ�.\n + ���� ActorNumber: " + PhotonNetwork.LocalPlayer.ActorNumber + "\n"
        //    + "���ڴ��� �׷�: " + recorder.InterestGroup;
        if (isRunning && startTime > 0)
        {
            double elapsed = PhotonNetwork.Time - startTime;
            double remainingTime = timeDuration - elapsed;

            timeText.text = (int)(remainingTime / 60) + "�� " + (int)(remainingTime % 60) + $"�� / {timeDuration}�� ���ѽð�";

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
    
    IEnumerator Coroutine_InitPlayerData()
    {
        gameManager.StartCamera();
        yield return new WaitForSeconds(2.5f);
        for (int i = 0; i < propositionSide_Chair.Length; i++)
        {
            if (propositionSide_Chair[i].sitting)
            {
                propositionSide.Add(propositionSide_Chair[i]);
            }
        }
        if (propositionSide.Count > 1)
        {
            speakerIdList[2] = propositionSide[0];
            speakerIdList[6] = propositionSide[1];
            speakerIdList[8] = propositionSide[0];
            speakerIdList[12] = propositionSide[1];
        }
        else if (propositionSide.Count == 1)
        {
            speakerIdList[2] = propositionSide[0];
            speakerIdList[6] = propositionSide[0];
            speakerIdList[8] = propositionSide[0];
            speakerIdList[12] = propositionSide[0];
        }

        for (int i = 0; i < oppositionSide_Chair.Length; i++)
        {
            if (oppositionSide_Chair[i].sitting)
            {
                oppositionSide.Add(oppositionSide_Chair[i]);
            }
        }
        if (oppositionSide.Count > 1)
        {
            speakerIdList[3] = oppositionSide[0];
            speakerIdList[5] = oppositionSide[1];
            speakerIdList[9] = oppositionSide[0];
            speakerIdList[11] = oppositionSide[1];
        }
        else if (oppositionSide.Count == 1)
        {
            speakerIdList[3] = oppositionSide[0];
            speakerIdList[5] = oppositionSide[0];
            speakerIdList[9] = oppositionSide[0];
            speakerIdList[11] = oppositionSide[0];
        }
        ModeratorSound.instance.SpeakPlayer(DataManager.instance.topicClip);
    }
    private void InitPlayerData()
    {
        StartCoroutine(Coroutine_InitPlayerData());
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
        // orderText.text = "AI ��ȸ�� �ð��Դϴ�.";
        m_eCurCharacterTurn = CharacterTurn.CharacterPlayerTurn;
        InitPlayerData();

    }

    // ��ư ����ȭ - ��ư�� �������� isMine�� ����ڿ��� ��� �ش� �Լ��� �����ϵ��� ����
    [PunRPC] // all�� �Ѹ��� isMine���� 1���� ����ǵ��� ����
    public void NextOrder()
    {
        NextTurn();
        panel_Timer.SetActive(true);

        //Debug.Log(PlayN + "���� ��ǥ ����");
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
        photonView.RPC("Toggle", PhotonNetwork.CurrentRoom.GetPlayer(speakerIdList[timelineIndex].id), true);
    }

    public void UpdatePlayerProsAndConsList()
    {
        int[] actorList = sceneNetworkManager.actorList;
        for (int i = 0; i < propositionSide_Chair.Length; i++)
        {
            if (propositionSide_Chair[i].sitting)
            {
                for(int t = 0; t < 4; t++)
                {
                    if (propositionSide_Chair[i].id == actorList[t])
                        tmpProsAndConsList[t].text = "���� ������:\n ����";
                }
                 
            }
        }

        for (int i = 0; i < oppositionSide_Chair.Length; i++)
        {
            if (oppositionSide_Chair[i].sitting)
            {
                for (int t = 0; t < 4; t++)
                {
                    if (oppositionSide_Chair[i].id == actorList[t])
                        tmpProsAndConsList[t].text = "���� ������:\n �ݴ�";
                }
            }
        }
    }
}
